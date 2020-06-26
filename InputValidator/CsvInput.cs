using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InputValidator
{
    public class CsvInput<T>
    {
        public List<Rule> ParserRuleSet { get; }
            = new List<Rule>();

        public List<Rule<string[]>> LineRuleSet { get; }
            = new List<Rule<string[]>>();

        public List<Rule<IEnumerable<T>>> GlobalRuleSet { get; }
            = new List<Rule<IEnumerable<T>>>();

        public Func<string[], T> Converter { get; set; }

        public char Separator { get; set; } = ';';

        public bool SkipFirstLine { get; set; }
        public bool SkipLastLine { get; set; }
        public bool HasTrailingSeparator { get; set; }

        public Results<T> ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            if (SkipFirstLine)
                lines = lines.Skip(1).ToArray();
            if (SkipLastLine)
                lines = lines.SkipLast(1).ToArray();

            return Parse(lines);
        }

        public Results<T> Parse(string[] lines)
        {
            if (Converter == null)
                return new Results<T>(new List<T>(), new[] { new Warning("No converter has been defined!", Extent.Configuration) }.ToList());

            if (ParserRuleSet.Count == 0)
                return new Results<T>(new List<T>(), new[] { new Warning("Parser ruleset lacks definitions!", Extent.Configuration) }.ToList());

            var converted = new ConcurrentBag<T>();
            var remarks = new ConcurrentBag<Warning>();

            try
            {
                Parallel.ForEach(lines, (line, _, idx) =>
                {
                    var elements = line.Split(Separator).SkipLast(HasTrailingSeparator ? 1 : 0).ToArray();

                    if (elements.Length == ParserRuleSet.Count)
                    {
                        var canParse = elements.Select((s, i) =>
                        {
                            var result = ParserRuleSet[i].Predicate(s);
                            if (!result)
                                remarks.Add(new Warning(idx, i + 1, ParserRuleSet[i].Explanation));
                            return result;
                        }).All(x => x);

                        if (canParse)
                        {
                            var faults = LineRuleSet.Where(rule => !rule.Predicate(elements)).ToArray();
                            if (faults.Length == 0)
                            {
                                var parsed = Converter(elements);
                                converted.Add(parsed);
                            }
                            else
                                foreach (var f in faults)
                                    remarks.Add(new Warning(idx, f.Explanation));
                        }
                    }
                    else
                        remarks.Add(new Warning(idx, "Line doesn't have the required number of elements."));
                });

                foreach (var gF in GlobalRuleSet.Where(g => !g.Predicate(converted)))
                    remarks.Add(new Warning(gF.Explanation, Extent.Global));

                return new Results<T>(converted.ToList(), remarks.ToList());
            }
            catch (Exception exc)
            {
                return new Results<T>(new List<T>(), new[] { new Warning($"An exception occurred while parsing data. Please try to use rules to prevent faulty conversion attempts. Exception: {exc.Message}", Extent.Configuration) }.ToList());
            }
        }
    }
}