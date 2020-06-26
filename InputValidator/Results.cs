using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InputValidator
{
    public class Results<T>
    {
        public ReadOnlyCollection<T> Parsed { get; }
        public ReadOnlyCollection<Warning> Warnings { get; }

        public Results(List<T> parsed, List<Warning> warnings)
        {
            Parsed = parsed.AsReadOnly();
            Warnings = warnings.AsReadOnly();
        }
    }
}