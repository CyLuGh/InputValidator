using FluentAssertions;
using NUnit.Framework;
using System;

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
    }
}