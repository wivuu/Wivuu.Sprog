using System;
using System.Text;
using System.Linq;

namespace csparser
{
    static partial class Parser
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

        public static ReadOnlySpan<char> Peek(this ReadOnlySpan<char> input, int take = 1) =>
            input.Length >= take
            ? input.Slice(0, take)
            : throw new Exception($"Unable to peek {take} from length {input.Length}");

        public static char Peek(this ReadOnlySpan<char> input) =>
            Peek(input, 1)[0];

        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate, take: 1));

        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Func<char, bool> predicate) =>
            input.Slice(MatchWhile(input, predicate));

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
}
