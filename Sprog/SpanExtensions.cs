using System;
using System.Runtime.CompilerServices;

namespace Wivuu.Sprog
{
    [Obsolete("Replaced with 'ToString()'")]
    public static class SpanExtensions
    {
        #region AsString

        /// <summary>
        /// Convert input span to string
        /// </summary>
        /// <param name="input">Input span</param>
        /// <returns>New string containing input characters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Replaced with 'ToString()'")]
        internal static string AsString(this ReadOnlySpan<char> input) => input.ToString();

        #endregion
    }
}
