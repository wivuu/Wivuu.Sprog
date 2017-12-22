using System;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    public partial struct ParserContext
    {
        internal ReadOnlySpan<char> Buffer;
        internal ParserError Error;

        public ParserContext(string buffer)
        {
            this.Buffer = buffer.AsSpan();
            this.Error  = null;
        }

        internal ParserContext(ReadOnlySpan<char> buffer, ParserError prevError)
        {
            this.Buffer = buffer;
            this.Error  = prevError;
        }

        /// <summary>
        /// Iterate through input until the predicate returns false
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Index match end</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int MatchWhile(Predicate predicate)
        {
            var i = 0;
            while (i < Buffer.Length && predicate( Buffer[i] ))
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
            while (i < Buffer.Length && i < take && predicate( Buffer[i] ))
                ++i;
                
            return i;
        }

        /// <summary>
        /// Return a new buffer at the input index
        /// </summary>
        /// <param name="n">Index</param>
        /// <returns>The next slice</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ParserContext Next(int n) =>
            new ParserContext(Buffer.Slice(n), Error);

        /// <summary>
        /// Take one character, if matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext TakeOne(out char match)
        {
            if (Buffer.Length > 0)
            {
                match = Buffer[0];
                return Next(1);
            }
            else
            {
                match = '\0';
                return this;
            }
        }

        /// <summary>
        /// Take one character, if matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext TakeOne(Predicate predicate, out char match)
        {
            if (Buffer.Length > 0 && predicate(Buffer[0]))
            {
                match = Buffer[0];
                return Next(1);
            }
            else
            {
                match = '\0';
                return this;
            }
        }

        /// <summary>
        /// Take multiple characters, while matching
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching characters</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Take(Predicate predicate, out string match)
        {
            var i = MatchWhile(predicate);
            match = Buffer.Slice(0, i).AsString();

            return Next(i);
        }

        /// <summary>
        /// Peek multiple characters
        /// </summary>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching string</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Peek(int take, out string match)
        {
            match = (Buffer.Length < take)
                ? null
                : Buffer.Slice(0, take).AsString();

            return this;
        }

        /// <summary>
        /// Peek single character
        /// </summary>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Peek(out char match)
        {        
            match = (Buffer.Length == 0)
                ? '\0'
                : Buffer[0];

            return this;
        }

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext SkipOne(Predicate predicate) =>
            Next(MatchWhile(predicate, take: 1));

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext SkipOne(char predicate) =>
            Next(MatchWhile(c => c == predicate, take: 1));

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Skip(Predicate predicate) =>
            Next(MatchWhile(predicate));

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="value">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Skip(string value)
        {
            if (Buffer.Length < value.Length)
                return this;

            var i = 0;
            while (i < value.Length && Buffer[i] == value[i])
                ++i;

            return Next(i);
        }

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Let(string id) => 
            this;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Let(ParserContext id) => 
            id;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Let<T>(T id) => 
            this;

        /// <summary>
        /// Return remaining buffer as 'out'
        /// </summary>
        /// <param name="rhs">Remaining buffer</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Rest(out ParserContext rhs) =>
            rhs = this;

        /// <summary>
        /// Throw an exception if failing condition
        /// </summary>
        /// <returns>Parser</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Assert(bool condition, string expected)
        {
            if (!condition)
                this.Error = new ParserError(Buffer.Length, expected);

            return this;
        }

        /// <summary>
        /// Returns an error summary
        /// </summary>
        /// <returns>Parser</returns>
        public ParserContext CheckError(out ParserError error)
        {
            error = this.Error;
            return this;
        }

        /// <summary>
        /// Convert input span to string
        /// </summary>
        /// <param name="input">Input span</param>
        /// <returns>New string containing input characters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe string ToString()
        {
            fixed (char* buffer = &Buffer.DangerousGetPinnableReference())
            {
                return new string(buffer, 0, Buffer.Length);
            }
        }
    }
}