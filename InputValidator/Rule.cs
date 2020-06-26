using System;
using System.Collections.Generic;
using System.Text;

namespace InputValidator
{
    public class Rule<T>
    {
        public Predicate<T> Predicate { get; }
        public string Explanation { get; }

        public Rule(Predicate<T> predicate, string explanation)
        {
            Predicate = predicate;
            Explanation = explanation;
        }
    }

    public class Rule : Rule<string>
    {
        public Rule(Predicate<string> predicate, string explanation) : base(predicate, explanation)
        {
        }
    }
}