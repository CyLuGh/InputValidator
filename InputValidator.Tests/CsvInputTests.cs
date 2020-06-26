using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InputValidator.Tests
{
    public class CsvInputTests
    {
        [Test]
        public void TestNoConverter()
        {
            var parser = new CsvInput<string>();

            var results = parser.Parse(new[] { "Lorem" });

            results.Parsed.Should().BeEmpty();
            results.Warnings.Should().HaveCount(1);
            results.Warnings[0].Explanation.Should().Be("No converter has been defined!");
        }

        [Test]
        public void NoLineRuleSet()
        {
            var parser = new CsvInput<string>
            {
                Converter = _ => string.Empty
            };

            var results = parser.Parse(new[] { "Lorem" });

            results.Parsed.Should().BeEmpty();
            results.Warnings.Should().HaveCount(1);
            results.Warnings[0].Explanation.Should().Be("Parser ruleset lacks definitions!");
        }

        [Test]
        public void CheckParserRuleSet()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1]
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.NoRule });

            var input = new[] {
                "1;Test;0",
                "2;Test;0.4",
                "3;Test;0,5",
                "4;;7",
                "5;Test;x"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(5);
            results.Warnings.Should().BeEmpty();
        }

        [Test]
        public void CheckParserRuleSetMissingElements()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1]
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.NoRule });

            var input = new[] {
                "1;Test;0",
                "2;0.4",
                "3;Test;0,5",
                "4;;7",
                "5;Test"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(3);
            results.Warnings.Should().HaveCount(2);
        }

        [Test]
        public void CheckParserRuleSetTrailingSeparator()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1]
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.NoRule });

            var input = new[] {
                "1;Test;0;",
                "2;Test;0.4;",
                "3;Test;0,5;",
                "4;;7;",
                "5;Test;x;"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().BeEmpty();
            results.Warnings.Should().HaveCount(5);

            parser.HasTrailingSeparator = true;
            results = parser.Parse(input);
            results.Parsed.Should().HaveCount(5);
            results.Warnings.Should().BeEmpty();
        }

        [Test]
        public void CheckParserRuleSetDouble()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1],
                    Factor = double.Parse(elements[2])
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.IsValidDouble });

            var input = new[] {
                "1;Test;0",
                "2;Test;0.4",
                "3;Test;0,5",
                "4;;7",
                "5;Test;x"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(4);
            results.Warnings.Should().HaveCount(1);
        }

        [Test]
        public void CheckParserRuleSetException()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1],
                    Factor = double.Parse(elements[2])
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.NoRule });

            var input = new[] {
                "1;Test;0",
                "2;Test;0.4",
                "3;Test;0,5",
                "4;;7",
                "5;Test;x"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().BeEmpty();
            results.Warnings.Should().HaveCount(1);
            results.Warnings[0].Explanation.Should().StartWith("An exception occurred while parsing data.");
        }

        [Test]
        public void CheckParserRuleSetValuesIn()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1],
                    Factor = double.Parse(elements[2])
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.HasValue(1, 2, 3, 4), CsvHelper.HasValue("Test"), CsvHelper.NoRule });

            var input = new[] {
                "1;Test;0",
                "2;Test;0.4",
                "3;Test;0,5",
                "4;;7",
                "5;Test;x"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(3);
            results.Warnings.Should().HaveCount(2);
        }

        [Test]
        public void CheckParserRuleSetValuesCustom()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1]
                }
            };

            parser.ParserRuleSet.AddRange(new[]
            {
                CsvHelper.IsValidInt,
                new Rule(s => s.StartsWith("Abc"), "Element should start with Abc"),
                CsvHelper.NoRule
            });

            var input = new[] {
                "1;XyZ;0",
                "2;Abc XyZ;4",
                "3;Abc Test;0,5",
                "4;;8",
                "3;Test Abc;6"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(2);
            results.Warnings.Should().HaveCount(3);
        }

        [Test]
        public void CheckLineRuleSet()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1],
                    Factor = double.Parse(elements[2])
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.IsValidDouble });

            Predicate<string[]> linePredicate =
                elements => int.TryParse(elements[0], out var id)
                && (
                    (id % 2 == 0 && double.TryParse(elements[2], out var factor) && factor == 2 * id)
                    || id % 2 == 1);
            parser.LineRuleSet.Add(new Rule<string[]>(linePredicate, "Even ids should have a factor = 2 * id"));

            var input = new[] {
                "1;Test;0",
                "2;Test;4",
                "3;Test;0,5",
                "4;;8",
                "6;Test;6"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(4);
            results.Warnings.Should().HaveCount(1);
        }

        [Test]
        public void CheckGlobalRuleSet()
        {
            var parser = new CsvInput<Output>
            {
                Converter = elements => new Output
                {
                    Id = int.Parse(elements[0]),
                    Description = elements[1]
                }
            };

            parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.NoRule });
            Predicate<IEnumerable<Output>> globalPredicate = outputs => outputs.GroupBy(o => o.Id).All(x => x.Count() == 1);

            parser.GlobalRuleSet.Add(new Rule<IEnumerable<Output>>(globalPredicate, "Ids should be unique"));

            var input = new[] {
                "1;Test;0",
                "2;Test;4",
                "3;Test;0,5",
                "4;;8",
                "3;Test;6"
            };

            var results = parser.Parse(input);
            results.Parsed.Should().HaveCount(5);
            results.Warnings.Should().HaveCount(1);
        }
    }
}