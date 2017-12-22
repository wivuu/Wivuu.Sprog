using System;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    /// <summary>
    /// Test if the input character matches
    /// </summary>
    /// <param name="c">Input character</param>
    /// <returns>True if input char matches</returns>
    public delegate bool Predicate(char c);

    public static partial class Parser
    {
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
            if (input.Length > 0 && predicate(input[0]))
            {
                match = input[0];
                return input.Slice(1);
            }
            else
            {
                match = '\0';
                return input;
            }
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
        /// Peek single character
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Peek(this ReadOnlySpan<char> input, out char match)
        {
            match = (input.Length == 0)
                ? '\0'
                : input[0];

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
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> SkipOne(this ReadOnlySpan<char> input, char predicate) =>
            input.Slice(MatchWhile(ref input, c => c == predicate, take: 1));

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
        /// Skip while predicate is true
        /// </summary>
        /// <param name="input">Input to match</param>
        /// <param name="value">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Skip(this ReadOnlySpan<char> input, string value)
        {
            if (input.Length < value.Length)
                return input;

            var i = 0;
            while (i < value.Length && input[i] == value[i])
                ++i;

            return input.Slice(i);
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
        public static ReadOnlySpan<char> Let(this ReadOnlySpan<char> rest, ReadOnlySpan<char> id) => 
            id;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Let<T>(this ReadOnlySpan<char> rest, T id) => 
            rest;

        /// <summary>
        /// Create an assertion
        /// </summary>
        /// <returns>Parser</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Assert(this ReadOnlySpan<char> rest, bool condition) =>
            rest;

        /// <summary>
        /// Create an assertion
        /// </summary>
        /// <returns>Parser</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Assert(this ReadOnlySpan<char> rest, string assertion)
        {
            if (assertion != null)
                throw new ParserException(assertion, rest.Length);

            return rest;
        }

        /// <summary>
        /// Return remaining buffer as 'out'
        /// </summary>
        /// <param name="lhs">Remaining buffer</param>
        /// <param name="rhs">Remaining buffer</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Rest(this ReadOnlySpan<char> lhs, out ReadOnlySpan<char> rhs) =>
            rhs = lhs;

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
    }
}