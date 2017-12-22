using System;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    public static class SpanExtensions
    {
        #region AsString

        /// <summary>
        /// Convert input span to string
        /// </summary>
        /// <param name="input">Input span</param>
        /// <returns>New string containing input characters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe string AsString(this ReadOnlySpan<char> input)
        {
            fixed (char* buffer = &input.DangerousGetPinnableReference())
            {
                return new string(buffer, 0, input.Length);
            }
        }

        #endregion
    }
}
