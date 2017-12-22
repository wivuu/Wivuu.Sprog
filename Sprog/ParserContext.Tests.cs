using System;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    public partial struct ParserContext
    {
        /// <summary>
        /// Check if parser has reached EOF
        /// </summary>
        /// <returns>Input buffer is 0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEOF() =>
            Buffer.Length == 0;

        /// <summary>
        /// Check if any errors have ocurred
        /// </summary>
        /// <returns>True if there is an error; otherwise false</returns>
        public bool HasError => this.Error != null;

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
            : false;
    }
}