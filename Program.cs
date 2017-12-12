using System;
using System.Linq;

namespace csparser
{
    using static Fundamentals;
    static class Fundamentals
    {
        public static bool Whitespace(char c) =>
            c == ' ' || c == '\t' || c == '\r' || c == '\n';
            
        public static bool AThroughZ(char c) => 
            ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
            
        public static ReadOnlySpan<char> Take(
            this ReadOnlySpan<char> input, Func<char, bool> predicate, out ReadOnlySpan<char> match)
        {
            var matchIndex = 0;
            
            while (matchIndex < input.Length && predicate( input[matchIndex] ))
                ++matchIndex;

            match = matchIndex > 0
                ? input.Slice(0, matchIndex)
                : throw new Exception("Unable to parse");

            return input.Slice(matchIndex);
        }

        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            Take(input, predicate, out _);

        public static string AsString(this ReadOnlySpan<char> input) =>
            new String(input.ToArray());
    }

    class Program
    {
        static void Main(string[] args)
        {
            var remaining = 
                @"  This is some string  "
                .AsSpan()
                .Skip(Whitespace)
                .Take(AThroughZ, out var word)
                .Skip(Whitespace);

            Console.WriteLine($"Word: {word.AsString()}");
            Console.WriteLine($"Remaining: {remaining.AsString()}");
        }
    }
}
