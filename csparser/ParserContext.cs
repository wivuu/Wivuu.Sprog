using System;
using System.Runtime.CompilerServices;

namespace csparser
{
    public partial struct ParserContext
    {
        internal ReadOnlySpan<char> Buffer;

        internal readonly int Index;

        public ParserContext(string buffer)
        {
            this.Buffer = buffer.AsSpan();
            this.Index  = 0;
        }

        internal ParserContext(ReadOnlySpan<char> buffer, int index = 0)
        {
            this.Buffer = buffer;
            this.Index  = index;
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
            new ParserContext(Buffer.Slice(n), Index + n);

        /// <summary>
        /// Return an exception containing a basic error report
        /// </summary>
        /// <returns>Error report exception</returns>
        ParseException ErrorReport(string expected = null)
        {
            const int MaxRest = 50;

            var line = 0; // TODO: retrieve line & col
            var col  = 0;
            var rest = Buffer.Length == 0 
                ? "End of file" 
                : Buffer.Length > MaxRest 
                ? Buffer.ToString().Remove(MaxRest)
                : Buffer.ToString();
            
            return new ParseException(line, col, rest, expected);
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
            var i = MatchWhile(predicate, take: 1);
            match = Buffer[0];

            return Next(i);
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
            Peek(1, out var m);
            match = m[0];

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
                throw ErrorReport(value);

            var i = 0;
            while (i < value.Length)
            {
                if (Buffer[i] != value[i])
                    throw Next(i).ErrorReport(value);

                ++i;
            }

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
        /// Check if parser has reached EOF
        /// </summary>
        /// <returns>Input buffer is 0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEOF() =>
            Buffer.Length == 0;

        /// <summary>
        /// Throw an exception if failing condition
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserContext Assert(bool condition, string expected)
        {
            if (!condition)
                throw ErrorReport(expected);

            return this;
        }

        /// <summary>
        /// Test if the input starts with the input value
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <returns>True if the input matches the pattern</returns>
        public bool StartsWith(string value)
        {
            if (Buffer.Length < value.Length)
                return false;

            for (var i = 0; i < value.Length; ++i) 
            {
                if (Buffer[i] != value[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Test if the input starts with the input value
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <returns>True if the input matches the pattern</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(char value) =>
            Buffer.Length > 0 
            ? Buffer[0] == value
            : throw ErrorReport();

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