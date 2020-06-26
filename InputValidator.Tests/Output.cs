using System;
using System.Collections.Generic;
using System.Text;

namespace InputValidator.Tests
{
    public class Output
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Factor { get; set; }

        public override string ToString()
        => $"{Id} - {Description} - {Factor}";
    }
}