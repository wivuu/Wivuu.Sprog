using System;
using System.Linq;

namespace csparser
{
    using static Fundamentals;
    static class Fundamentals
    {
        public static char[] Whitespace = 
            new [] { ' ', '\t', '\r', '\n' };
            
        public static char[] AThroughZ =
            (from c in "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
             select c).ToArray();

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

        public static ReadOnlySpan<char> Take(this ReadOnlySpan<char> input, char[] predicate, out ReadOnlySpan<char> match)
        {
            bool Predicate(char c) => Array.IndexOf(predicate, c) != -1;

            return Take(input, Predicate, out match);
        }

        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            Take(input, predicate, out _);

        public static ReadOnlySpan<char> SkipWhitespace(this ReadOnlySpan<char> input)
        {
            bool Predicate(char c) => Array.IndexOf(Whitespace, c) != -1;

            return Skip(input, Predicate);
        }

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
                .SkipWhitespace()
                .Take(AThroughZ, out var word)
                .SkipWhitespace();

            Console.WriteLine($"Word: {word.AsString()}");
            Console.WriteLine($"Remaining: {remaining.AsString()}");
        }
    }
}
