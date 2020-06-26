using System;
using System.Globalization;
using System.Linq;

namespace InputValidator
{
    public static class CsvHelper
    {
        public static Rule NoRule
            => new Rule(_ => true, string.Empty);

        public static Rule NotEmpty
            => new Rule(s => !string.IsNullOrEmpty(s), "Element isn't allowed to be empty");

        public static Rule NotWhiteSpace
            => new Rule(s => !string.IsNullOrWhiteSpace(s), "Element isn't allowed to be white spaces");

        public static Rule IsValidDouble
            => new Rule(s => double.TryParse(s, out var _), "Wrong number format");

        public static Rule IsValidDoubleInvariant
            => new Rule(s => double.TryParse(s, NumberStyles.Any, new CultureInfo(""), out var _), "Wrong number format");

        public static Rule IsValidInt
           => new Rule(s => int.TryParse(s, out var _), "Wrong number format");

        public static Rule IsValidIntInvariant
            => new Rule(s => int.TryParse(s, NumberStyles.Any, new CultureInfo(""), out var _), "Wrong number format");

        public static Rule HasValue(params string[] values)
            => new Rule(s => values.Contains(s), $"Element isn't found in expected values ({string.Join(',', values)})");

        public static Rule HasValue(params int[] values)
            => new Rule(s => int.TryParse(s, out var i) && values.Contains(i), $"Element isn't found in expected values ({string.Join(',', values)})");
    }
}