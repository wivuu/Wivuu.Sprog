using System;
using System.Runtime.CompilerServices;

namespace csparser
{
    public static partial class Parser
    {
        /// <summary>
        /// Test if the input starts with the input value
        /// </summary>
        /// <param name="value">Input pattern</param>
        /// <returns>True if the input matches the pattern</returns>
        public static bool StartsWith(this ReadOnlySpan<char> input, string value)
        {
            if (input.Length < value.Length)
                return false;

            for (var i = 0; i < value.Length; ++i) 
            {
                if (input[i] != value[i])
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
        public static bool StartsWith(this ReadOnlySpan<char> input, char value) => 
            input[0] == value;
    }
}