using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    public partial struct Parser
    {
        #region MatchWhile

        /// <summary>
        /// Iterate through input until the predicate returns false
        /// </summary>
        /// <param name="input">Input to match</param>
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
        /// <param name="input">Input to match</param>
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
        /// <param name="input">Input to match</param>
        /// <param name="predicate">Input test</param>
        /// <param name="match">Matching character</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Take(Predicate predicate, out char match)
        {
            if (Buffer.Length > 0 && predicate(Buffer[0]))
            {
                match = Buffer[0];
                return Buffer.Slice(1);
            }
            else
            {
                match = '\0';
                return Buffer;
            }
        }

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
                return Buffer.Slice(1);
            }
            else
            {
                match = '\0';
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
        public Parser Take(Predicate predicate, out string match)
        {
            var i = MatchWhile(predicate);
            match = Buffer.Slice(0, i).AsString();

            return Buffer.Slice(i);
        }

        #endregion

        #region Peek

        /// <summary>
        /// Peek multiple characters
        /// </summary>
        /// <param name="take">Input test</param>
        /// <param name="match">Matching string</param>
        /// <returns>True if enough characters to match; otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Peek(int take, out string match)
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
        public Parser Peek(out char match)
        {
            match = (Buffer.Length == 0)
                ? '\0'
                : Buffer[0];

            return this;
        }

        #endregion

        #region Skip

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipOne(Predicate predicate) =>
            Buffer.Slice(MatchWhile(predicate, take: 1));

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser SkipOne() =>
            Buffer.Length > 0 
            ? Buffer.Slice(1) 
            : this;

        /// <summary>
        /// Skip one character if predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(char predicate) =>
            Buffer.Slice(MatchWhile(c => c == predicate, take: 1));

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="predicate">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(Predicate predicate) =>
            Buffer.Slice(MatchWhile(predicate));

        /// <summary>
        /// Skip while predicate is true
        /// </summary>
        /// <param name="value">Input test</param>
        /// <returns>Remainder of input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Skip(string value)
        {
            if (Buffer.Length < value.Length)
                return Buffer;

            var i = 0;
            while (i < value.Length && Buffer[i] == value[i])
                ++i;

            return Buffer.Slice(i);
        }

        #endregion

        #region Let

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let(string id) => 
            this;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let(Parser id) =>
            id;

        /// <summary>
        /// Assign and return remaining buffer
        /// </summary>
        /// <param name="rest">Remaining buffer</param>
        /// <param name="id">Assignments</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Let<T>(T id) => 
            this;

        #endregion

        #region Rest

        /// <summary>
        /// Return remaining buffer as 'out'
        /// </summary>
        /// <param name="lhs">Remaining buffer</param>
        /// <param name="rhs">Remaining buffer</param>
        /// <returns>Remaining buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Rest(out Parser rhs) =>
            rhs = this;

        #endregion

        #region Assert

        /// <summary>
        /// Create an assertion
        /// </summary>
        /// <returns>Parser</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parser Assert(string assertion)
        {
            if (assertion != null)
                throw new ParserException(assertion, Buffer.Length);

            return this;
        }

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
            Buffer[0] == value;

        #endregion
    }
}