using System;
using System.Text;
using System.Linq;

namespace csparser
{
    using static Parser;
    using static Char;

    static class Program
    {
        static ReadOnlySpan<char> ParseIdentifier(this ReadOnlySpan<char> input, out string identifier)
        {
            var result = input
                 .TakeOne(IsLetter, out var prefix)
                 .Take(IsLetterOrDigit, out var word)
                 .Skip(IsWhiteSpace);

            identifier = $"{prefix}{word.AsString()}";
            return result;
        }

        static void Main(string[] args)
        {
            var remaining = 
                @"1  B3his is some string  "
                .AsSpan()
                .SkipOne(IsDigit)
                .Skip(IsWhiteSpace)
                .ParseIdentifier(out var ident)
                .Skip(IsWhiteSpace);

            Console.WriteLine($"    Ident: {ident}");
            Console.WriteLine($"Remaining: {remaining.AsString()}");
        }
    }
}
