using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;

namespace csparser
{
    public static class ParserExtensions
    {
        public static ReadOnlySpan<char> ParseIdentifier(this ReadOnlySpan<char> input, out string identifier)
        {
            var result = input
                 .TakeOne(IsLetter, out var prefix)
                 .Take(IsLetterOrDigit, out var word);

            identifier = $"{prefix}{word.AsString()}";
            return result;
        }
    }

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestParserContinuators()
        {
            (string, ReadOnlySpan<char>) identifier(string input) =>
                input.AsSpan()
                     .Skip(IsWhiteSpace)
                     .TakeOne(IsLetter, out var first)
                     .Take(IsLetterOrDigit, out var rest)
                     .Skip(IsWhiteSpace)
                     .Result($"{first}{rest.AsString()}");

            var (output, _) = identifier(" tBis   ");
        }

        [TestMethod]
        public void TestParseIdents()
        {
            var remaining = 
                @"1  B3his is some string  "
                .AsSpan()
                .SkipOne(IsDigit)
                .Skip(IsWhiteSpace);

            if (remaining.Peek(out var c))
            {
                remaining
                    .ParseIdentifier(out var ident)
                    .Skip(IsWhiteSpace);

                Console.WriteLine($"     Char: `{c}`");
                Console.WriteLine($"    Ident: `{ident}`");
                Console.WriteLine($"Remaining: `{remaining.AsString()}`");
            }
        }
    }
}
