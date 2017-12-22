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
            var prevline = new Parser(fromText);

            // TODO: Also retrieve text 'surrounding' the error:
            // MyCodeFile.xml(24, 4) Expected end tag </li> at `some stuff</lli>`

            for (var line = 1; !prevline.IsEOF(); ++line)
            {
                var nextline = prevline
                    .Skip(c => c != '\n')
                    .SkipOne();

                // If we've passed the index, go back and
                // find the column
                if (nextline.Length <= Remaining) 
                {
                    var index     = fromText.Length - Remaining;
                    var lineStart = fromText.Length - prevline.Buffer.Length;

                    LineAndColumn = (line, 1 + index - lineStart);
                    break;
                }

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