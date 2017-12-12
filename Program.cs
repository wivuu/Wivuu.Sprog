using System;
using System.Text;
using System.Linq;

namespace csparser
{
    using static Fundamentals;
    static class Fundamentals
    {
        static int MatchWhile(this ReadOnlySpan<char> input, Func<char, bool> predicate, bool failOnMiss)
        {
            var i = 0;
            while (i < input.Length && predicate( input[i] ))
                ++i;

            if (failOnMiss && i == 0)
                throw new Exception($"Unable to parse: \"{input.AsString()}\"");

            return i;
        }
        
        static int MatchWhile(this ReadOnlySpan<char> input, Func<char, bool> predicate, bool failOnMiss, int take)
        {
            var i = 0;
            while (i < input.Length && i < take && predicate( input[i] ))
                ++i;

            if (failOnMiss && i == 0)
                throw new Exception("Unable to parse");

            return i;
        }

        public static ReadOnlySpan<char> TakeOne(
            this ReadOnlySpan<char> input, Func<char, bool> predicate, out char match)
        {
            var i = MatchWhile(input, predicate, failOnMiss: true, take: 1);
            match = input[0];

            return input.Slice(i);
        }

        public static ReadOnlySpan<char> Take(
            this ReadOnlySpan<char> input, Func<char, bool> predicate, out ReadOnlySpan<char> match)
        {
            var i = MatchWhile(input, predicate, failOnMiss: true);
            match = input.Slice(0, i);

            return input.Slice(i);
        }

        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate, failOnMiss: false, take: 1));

        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate, failOnMiss: false));

        public static string AsString(this ReadOnlySpan<char> input) =>
            new String(input.ToArray());
            
        public static string Concat(this ReadOnlySpan<char> input, params ReadOnlySpan<char>[] rest)
        {
            var sb = new StringBuilder();

            sb.Append(input.AsString());
            for (var i = 0; i < rest.Length; ++i)
                sb.Append(rest[i].AsString());

            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var remaining = 
                @"1  B3his is some string  "
                .AsSpan()
                .SkipOne(Char.IsDigit)
                .Skip(Char.IsWhiteSpace)
                .TakeOne(Char.IsLetter, out var prefix)
                .Take(Char.IsLetterOrDigit, out var word)
                .Skip(Char.IsWhiteSpace);

            Console.WriteLine($"     Word: {prefix}{word.AsString()}");
            Console.WriteLine($"Remaining: {remaining.AsString()}");
        }
    }
}
