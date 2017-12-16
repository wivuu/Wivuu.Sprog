using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace csparser
{
    public static partial class Parser
    {
        public delegate bool Predicate(char id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int MatchWhile(ref ReadOnlySpan<char> input, Predicate predicate)
        {
            var i = 0;
            while (i < input.Length && predicate( input[i] ))
                ++i;

            return i;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int MatchWhile(ref ReadOnlySpan<char> input, Predicate predicate, int take)
        {
            var i = 0;
            while (i < input.Length && i < take && predicate( input[i] ))
                ++i;

            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> TakeOne(
            this ReadOnlySpan<char> input, Predicate predicate, out char match)
        {
            var i = MatchWhile(ref input, predicate, take: 1);
            match = input[0];

            return input.Slice(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Take(
            this ReadOnlySpan<char> input, Predicate predicate, out ReadOnlySpan<char> match)
        {
            var i = MatchWhile(ref input, predicate);
            match = input.Slice(0, i);

            return input.Slice(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Peek(this ReadOnlySpan<char> input, int take, out ReadOnlySpan<char> match)
        {
            if (input.Length < take)
                return false;

            match = input.Slice(0, take);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Peek(this ReadOnlySpan<char> input, out char match)
        {        
            var result = Peek(input, 1, out var m);
            match      = m[0];

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, Predicate predicate) =>
            input.Slice(MatchWhile(ref input, predicate, take: 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Predicate predicate) =>
            input.Slice(MatchWhile(ref input, predicate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string AsString(this ReadOnlySpan<char> input)
        {
            fixed (char* buffer = &input.DangerousGetPinnableReference())
            {
                return new string(buffer, 0, input.Length);
            }
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(this ReadOnlySpan<char> input, ReadOnlySpan<char> rest) =>
            string.Concat(input.AsString(), rest.AsString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Let(this ReadOnlySpan<char> rest, string id) => 
            rest;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Let<T>(this ReadOnlySpan<char> rest, T id) => 
            rest;
    }
}