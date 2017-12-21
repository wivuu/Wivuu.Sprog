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
            ? $"{Context} ({Line},{Column}), Expected {Expected}, got: {Rest}..."
            : $"({Line},{Column}), got: {Rest}...";

        public ParseException(int line, int col, string rest)
        {
            this.Line   = line;
            this.Column = col;
            this.Rest   = rest;
        }

        public ParseException AddDetail(string file = null, string expected = null)
        {
            this.Context     = file;
            this.Expected = expected;

            return this;
        }
    }
}