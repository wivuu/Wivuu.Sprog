using System;

namespace Wivuu.Sprog;

public class ParserException : Exception
{
    internal ParserException(string assertion, int remaining)
        : base(assertion)
    {
        this.Assertion = assertion;
        this.Remaining = remaining;
    }

    public string Assertion { get; }

    public int Remaining { get; }
}