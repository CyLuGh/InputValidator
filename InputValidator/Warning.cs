namespace InputValidator
{
    public enum Extent
    {
        Configuration,
        Global,
        Line,
        Element
    }

    public class Warning
    {
        public long LineNumber { get; }
        public int ElementPosition { get; }
        public string Explanation { get; }
        public Extent Extent { get; }

        public Warning(string explanation, Extent extent)
        {
            Explanation = explanation;
            Extent = extent;
        }

        public Warning(long lineNumber, string explanation)
        {
            LineNumber = lineNumber;
            Explanation = explanation;
            Extent = Extent.Line;
        }

        public Warning(long lineNumber, int elementPosition, string explanation)
        {
            LineNumber = lineNumber;
            ElementPosition = elementPosition;
            Explanation = explanation;
            Extent = Extent.Element;
        }
    }
}