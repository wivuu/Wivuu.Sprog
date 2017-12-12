using System;
using System.Linq;

namespace csparser
{
    using System.Globalization;
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

            while (matchIndex < input.Length)
            {
                if (predicate(input[matchIndex]))
                    ++matchIndex;
                else
                    break;
            }

            match = matchIndex > 0
                ? input.Slice(0, matchIndex)
                : throw new Exception("(Take) Unable to parse");

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
    }

    class Program
    {
        static void Main(string[] args)
        {
            @"  This is some string  "
                .AsSpan()
                .SkipWhitespace()
                .Take(AThroughZ, out var word);

            Console.WriteLine($"Word: {word}");
        }
    }
}
