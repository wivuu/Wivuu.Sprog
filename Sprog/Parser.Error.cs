using System;

namespace Wivuu.Sprog
{
    public class ParserError
    {
        public int Remaining { get; }

        public string Expected { get; }

        public (int line, int col)? LineAndColumn { get; private set; }

        public ParserError(int remaining, string expected)
        {
            this.Remaining = remaining;
            this.Expected  = expected;
        }

        public ParserError CalculateLineAndCol(string fromText)
        {
            var prevline = new ParserContext(fromText);
            var line = 1;

            while (!prevline.IsEOF())
            {
                var nextline = prevline
                    .Skip(c => c != '\n')
                    .TakeOne(out char ln);

                // If we've passed the index, go back and
                // find the column
                if (nextline.Buffer.Length <= Remaining) 
                {
                    var index     = fromText.Length - Remaining;
                    var lineStart = fromText.Length - prevline.Buffer.Length;

                    LineAndColumn = (line, 1 + index - lineStart);
                    break;
                }

                if (ln == '\n') ++line;

                prevline = nextline;
            }

            return this;
        }

        public override string ToString()
        {
            if (LineAndColumn.HasValue)
            {
                var (line, col) = LineAndColumn.Value;
                return $"({line}, {col}) {Expected}'";
            }
            else
                return Expected;
        }
    }
}