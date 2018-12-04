using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Wivuu.Sprog
{
    /// <summary>
    /// Test if the input character matches
    /// </summary>
    /// <param name="c">Input character</param>
    /// <returns>True if input char matches</returns>
    public delegate bool Predicate(char c);
    
    /// <summary>
    /// Condition of the match
    /// </summary>
    /// <param name="input">Input parser buffer</param>
    /// <returns>True if the input succeeds, and the value of the successfully parsed value</returns>
    public delegate (bool success, T value) TakeManyCondition<T>(ref Parser input);

    /// <summary>
    /// Sprog Parser
    /// </summary>
    public readonly ref partial struct Parser
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
            ref char buffer_ptr = ref MemoryMarshal.GetReference(Buffer);

            fixed (char* ptr = &buffer_ptr)
                return new string(ptr, 0, Buffer.Length);
        }

        #endregion
    }
}