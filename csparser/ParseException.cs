using System;

namespace csparser
{
    public class ParseException : Exception
    {
        public string Context { get; private set; }

        public string Expected { get; private set; }

        public int Line { get; }

        public int Column { get; }

        public string Rest { get; }

        public override string Message =>
            Context == null
                ? Expected == null
                ? $"({Line},{Column}), got: {Rest}..."
                : $"({Line},{Column}), Expected {Expected}, got: {Rest}..."
                : Expected == null
                ? $"{Context} ({Line},{Column}), got: {Rest}..."
                : $"{Context} ({Line},{Column}), Expected {Expected}, got: {Rest}...";

        public ParseException(int line, int col, string rest, string expected = null)
        {
            this.Line     = line;
            this.Column   = col;
            this.Rest     = rest;
            this.Expected = expected;
        }

        public ParseException AddDetail(string file = null, string expected = null)
        {
            this.Context  = file;
            this.Expected = expected;

            return this;
        }
    }
}