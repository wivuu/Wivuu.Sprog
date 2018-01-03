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

    /// <summary>
    /// Sprog Parser
    /// </summary>
    public partial struct Parser
    {
        #region Members

        public readonly ReadOnlySpan<char> Buffer;

        public int Length => Buffer.Length;

        #endregion

        #region Constructors

        public Parser(string input) : this(input.AsSpan()) { }

        public Parser(ReadOnlySpan<char> buffer)
        {
            this.Buffer = buffer;
        }

        #endregion

        #region Operators

        public static implicit operator Parser(ReadOnlySpan<char> input) => new Parser(input);

        public char this[int i] => Buffer[i];

        #endregion

        #region Methods

        /// <summary>
        /// Convert input span to string
        /// </summary>
        /// <param name="input">Input span</param>
        /// <returns>New string containing input characters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe override string ToString()
        {
            var buffer = Buffer;

            fixed (char* buffer_ptr = &buffer.DangerousGetPinnableReference())
            {
                return new string(buffer_ptr, 0, buffer.Length);
            }
        }

        #endregion
    }
}