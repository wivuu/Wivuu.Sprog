using System;
using System.Text;

namespace csparser
{
    public static partial class Parser
    {
        static int MatchWhile(this ReadOnlySpan<char> input, Func<char, bool> predicate)
        {
            var i = 0;
            while (i < input.Length && predicate( input[i] ))
                ++i;

            return i;
        }
        
        static int MatchWhile(this ReadOnlySpan<char> input, Func<char, bool> predicate, int take)
        {
            var i = 0;
            while (i < input.Length && i < take && predicate( input[i] ))
                ++i;

            return i;
        }

        public static ReadOnlySpan<char> TakeOne(
            this ReadOnlySpan<char> input, Func<char, bool> predicate, out char match)
        {
            var i = MatchWhile(input, predicate, take: 1);
            match = input[0];

            return input.Slice(i);
        }

        public static ReadOnlySpan<char> Take(
            this ReadOnlySpan<char> input, Func<char, bool> predicate, out ReadOnlySpan<char> match)
        {
            var i = MatchWhile(input, predicate);
            match = input.Slice(0, i);

            return input.Slice(i);
        }

        public static bool Peek(this ReadOnlySpan<char> input, int take, out ReadOnlySpan<char> match)
        {
            if (input.Length < take)
                return false;

            match = input.Slice(0, take);

            return true;
        }

        public static bool Peek(this ReadOnlySpan<char> input, out char match)
        {        
            var result = Peek(input, 1, out var m);
            match      = m[0];

            return result;
        }

        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate, take: 1));

        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate));

        public static string AsString(this ReadOnlySpan<char> input) =>
            new string(input.ToArray());
            
        public static string Concat(this ReadOnlySpan<char> input, params ReadOnlySpan<char>[] rest)
        {
            var sb = new StringBuilder();

            sb.Append(input.AsString());
            for (var i = 0; i < rest.Length; ++i)
                sb.Append(rest[i].AsString());

            return sb.ToString();
        }

        public static (string id, ReadOnlySpan<char> rest) Result(this ReadOnlySpan<char> rest, string id) => 
            (id, rest);
    }
}
