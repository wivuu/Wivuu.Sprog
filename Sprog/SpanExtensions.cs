using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            ref char buffer_ptr = ref MemoryMarshal.GetReference(input);
            
            fixed (char* ptr = &buffer_ptr)
                return new string(ptr, 0, input.Length);
        }

        #endregion
    }
}
