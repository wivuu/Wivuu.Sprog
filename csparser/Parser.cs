using System;
using System.Runtime.CompilerServices;

namespace csparser
{
    public static partial class Parser
    {
        /// <summary>
        /// Test if the input character matches
        /// </summary>
        /// <param name="c">Input character</param>
        /// <returns>True if input char matches</returns>
        public delegate bool Predicate(char c);

        /// <summary>
        /// Iterate through input until the predicate returns false
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <returns>Index match end</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int MatchWhile(ref ReadOnlySpan<char> input, Predicate predicate)
        {
            var i = 0;
            while (i < input.Length && predicate( input[i] ))
                ++i;

            return i;
        }

        /// <summary>
        /// Iterate through input until the predicate returns false or number of taken
        /// characters taken is met
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <param name="take">Number of characters to take</param>
        /// <returns>Index match end</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int MatchWhile(ref ReadOnlySpan<char> input, Predicate predicate, int take)
        {
            var i = 0;
            while (i < input.Length && i < take && predicate( input[i] ))
                ++i;

            return i;
        }

        /// <summary>
        /// Take one character, if matching
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> TakeOne(
            this ReadOnlySpan<char> input, Predicate predicate, out char match)
        {
            var i = MatchWhile(ref input, predicate, take: 1);
            match = input[0];

            return input.Slice(i);
        }

        /// <summary>
        /// Take multiple characters, while matching
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching characters</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Take(
            this ReadOnlySpan<char> input, Predicate predicate, out string match)
        {
            var i = MatchWhile(ref input, predicate);
            match = input.Slice(0, i).AsString();

            return input.Slice(i);
        }

        /// <summary>
        /// Peek single character
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Peek(this ReadOnlySpan<char> input, out char match)
        {        
            Peek(input, 1, out var m);
            match = m[0];

            return input;
        }

        /// <summary>
        /// Peek multiple characters
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching string</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Peek(this ReadOnlySpan<char> input, int take, out string match)
        {
            match = (input.Length < take)
                ? null
                : input.Slice(0, take).AsString();

            return input;
        }

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, Predicate predicate) =>
            input.Slice(MatchWhile(ref input, predicate, take: 1));

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, Predicate predicate) =>
            input.Slice(MatchWhile(ref input, predicate));

        /// <summary>
        /// Convert input span to string
        /// </summary>
        /// <param name="input">Input span</param>
        /// <returns>New string containing input characters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string AsString(this ReadOnlySpan<char> input)
        {
            fixed (char* buffer = &input.DangerousGetPinnableReference())
            {
                return new string(buffer, 0, input.Length);
            }
        }

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Let(this ReadOnlySpan<char> rest, string id) => 
            rest;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Let<T>(this ReadOnlySpan<char> rest, T id) => 
            rest;
    }
}