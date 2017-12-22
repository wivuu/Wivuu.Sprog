using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;

namespace Wivuu.Sprog
{
    public static class ParserExtensions
    {
        public static ReadOnlySpan<char> TakeIdentifier(this ReadOnlySpan<char> input, out string identifier) =>
            input.Skip(IsWhiteSpace)
                 .TakeOne(IsLetter, out var first)
                 .Take(IsLetterOrDigit, out var rest)
                 .Let(identifier = $"{first}{rest}");
    }

    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestParserContinuators()
        {
            var remaining = " This is a test ".AsSpan();

            while (remaining.Length > 0) 
            {
                remaining = remaining
                    .TakeIdentifier(out var identifier)
                    .Skip(IsWhiteSpace);

                Console.WriteLine(identifier);
            }
        }

        [TestMethod]
        public void TestParseIdents()
        {
            var remaining = 
                @"1  B3his is some string  "
                .AsSpan()
                .SkipOne(IsDigit)
                .Skip(IsWhiteSpace)
                .Peek(out var c)
                .TakeIdentifier(out var ident)
                .Skip(IsWhiteSpace);

            Console.WriteLine($"     Char: `{c}`");
            Console.WriteLine($"    Ident: `{ident}`");
            Console.WriteLine($"Remaining: `{remaining.AsString()}`");
        }
    }
}