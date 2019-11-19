using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    public partial struct Parser
    {
        #region MatchWhile

        /// <summary>
        /// Iterate through input until the predicate returns false
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Index match end</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int MatchWhile(Predicate predicate)
        {
            var i = 0;
            while (i < Buffer.Length && predicate(Buffer[i]))
                ++i;

            return i;
        }

        /// <summary>
        /// Iterate through input until the predicate returns false or number of taken
        /// characters taken is met
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="take">Number of characters to take</param>
        /// <returns>Index match end</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int MatchWhile(Predicate predicate, int take)
        {
            var i = 0;
            while (i < Buffer.Length && i < take && predicate(Buffer[i]))
                ++i;

            return i;
        }

        #endregion

        #region Take

        /// <summary>
        /// Take one character, if matching
        /// </summary>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Take(out char match)
        {
            if (Buffer.Length > 0)
            {
                match = Buffer[0];
                return Buffer[1..];
            }
            else
            {
                match = default;
                return Buffer;
            }
        }

        /// <summary>
        /// Take one character, if matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Take(Predicate predicate, out char match)
        {
            if (Buffer.Length > 0 && predicate(Buffer[0]))
            {
                match = Buffer[0];
                return Buffer[1..];
            }
            else
            {
                match = default;
                return Buffer;
            }
        }

        /// <summary>
        /// Take multiple characters, while matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching characters</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Take(Predicate predicate, out ReadOnlySpan<char> match)
        {
            var i = MatchWhile(predicate);
            match = Buffer[..i];

            return Buffer[i..];
        }

        /// <summary>
        /// Take multiple characters, while matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching characters</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Take(Predicate predicate, out string match)
        {
            var i = MatchWhile(predicate);
            match = Buffer[..i].ToString();

            return Buffer[i..];
        }

        #endregion

        #region TakeWhile
        
        /// <summary>
        /// Take multiple items, while matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching items</param>
        /// <returns>Remainder of input</returns>
        public Parser TakeMany<T>(TakeManyCondition<T> predicate, out List<T> match)
        {
            var input = this;
            match     = new List<T>();

            do 
            {
                var (success, value) = predicate(ref input);
                
                if (success)
                    match.Add(value);
                else
                    break;
            }
            while (true);

            return input;
        }

        #endregion

        #region Peek

        /// <summary>
        /// Peek multiple characters
        /// </summary>
        /// <param name="length">Input test</param>
        /// <param name="match">Matching span</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Peek(int length, out ReadOnlySpan<char> match)
        {
            match = Buffer.Length < length
                ? default
                : Buffer[..length];

            return this;
        }

        /// <summary>
        /// Peek multiple characters
        /// </summary>
        /// <param name="length">Input test</param>
        /// <param name="match">Matching string</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Peek(int length, out string match)
        {
            match = Buffer.Length < length
                ? default
                : Buffer[..length].ToString();

            return this;
        }

        /// <summary>
        /// Peek single character
        /// </summary>
        /// <param name="match">Matching character</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Peek(out char match)
        {
            match = Buffer.Length == 0
                ? default
                : Buffer[0];

            return this;
        }

        #endregion

        #region Skip

        /// <summary>
        /// Skip one character
        /// </summary>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipOne() =>
            Buffer.Length > 0 
            ? Buffer[1..]
            : this;

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipOne(char predicate) =>
            Buffer.Length > 0 && Buffer[0] == predicate
            ? Buffer[1..]
            : this;

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipOne(Predicate predicate) =>
            Buffer.Length > 0 && predicate(Buffer[0])
            ? Buffer[1..]
            : this;

        /// <summary>
        /// Test if the input starts with the input value before skipping
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <param name="skipped">Input was skipped</param>
        /// <returns>True if the input matches the pattern</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(ReadOnlySpan<char> value, out bool skipped) =>
            (skipped = StartsWith(value))
            ? Skip(value)
            : this;

        /// <summary>
        /// Test if the input starts with the input value before skipping
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <param name="skipped">Input was skipped</param>
        /// <>True if the input matches the pattern</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(char value, out bool skipped) =>
            (skipped = StartsWith(value))
            ? SkipOne()
            : this;

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(Predicate predicate) =>
            Buffer[MatchWhile(predicate)..];

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(ReadOnlySpan<char> predicate)
        {
            if (predicate == null || Buffer.Length < predicate.Length)
                return Buffer;

            int i;
            for (i = 0; i < predicate.Length; ++i)
            {
                if (Buffer[i] != predicate[i])
                    return this;
            }

            return Buffer[i..];
        }
        
        /// <summary>
        /// Skip until predicate is matched
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipUntil(char predicate)
        {
            bool neq(char c) => c != predicate;

            return Skip(neq);
        }

        #endregion

        #region Let

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let(ReadOnlySpan<char> id) => 
            this;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let(string id) => 
            this;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let(Parser id) =>
            id;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let<T>(T id) => 
            this;

        #endregion

        #region Declare

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="output">Newly declared output</param>
        /// <param name="input">Input</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Declare(out string output, string input)
        {
            output = input;
            return this;
        }

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="output">Newly declared output</param>
        /// <param name="input">Input</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Declare(out char output, char input)
        {
            output = input;
            return this;
        }

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="output">Newly declared output</param>
        /// <param name="input">Input</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Declare<T>(out T output, T input)
        {
            output = input;
            return this;
        }
        
        #endregion

        #region Rest

        /// <summary>
        /// Return remaining buffer as 'out'
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Rest(out Parser rest) =>
            rest = this;

        #endregion

        #region If

        /// <summary>
        /// Return true if condition is true, or false if condition is false
        /// </summary>
        /// <returns>True or false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool If(bool condition) =>
            condition;
            
        /// <summary>
        /// Return true if condition is true, or false if condition is false
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <returns>True or false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool If(bool condition, out Parser rest)
        {
            rest = this;
            return condition;
        }

        #endregion
        
        #region Assert

        /// <summary>
        /// Create an assertion
        /// </summary>
        /// <returns>Parser</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Assert(ReadOnlySpan<char> assertion)
        {
            if (assertion != null)
                throw new ParserException(assertion.ToString(), Buffer.Length);

            return this;
        }

        /// <summary>
        /// Assertion fails
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserException Fail(string assertion) =>
            new ParserException(assertion, Buffer.Length);

        #endregion

        #region Tests

        /// <summary>
        /// Check if parser has reached EOF
        /// </summary>
        /// <returns>Input buffer is 0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEOF() =>
            Buffer.Length == 0;

        /// <summary>
        /// Test if the input starts with the input value
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <returns>True if the input matches the pattern</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(ReadOnlySpan<char> value)
        {
            if (value == null) 
                return false;

            int i;
            for (i = 0; i < value.Length && i < Buffer.Length; ++i) 
            {
                if (Buffer[i] != value[i])
                    return false;
            }

            return i == value.Length;
        }
        
        /// <summary>
        /// Test if the input starts with the input value
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <returns>True if the input matches the pattern</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(char value) => 
            Buffer.Length > 0 && Buffer[0] == value;
        
        #endregion

        #region Returns
        
        /// <summary>
        /// Return input value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Return<T>(T value) => value;

        #endregion
    }
}