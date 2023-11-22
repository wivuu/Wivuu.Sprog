using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Wivuu.Sprog;

public static class SpanExtensions
{
    #region Concat

    /// <summary>
    /// Concatenate a char to a span
    /// </summary>
    /// <param name="value">Input char</param>
    /// <returns>New string containing input characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string Concat(
        this ReadOnlySpan<char> input,
        char value)
    {
        fixed (char* f = &MemoryMarshal.GetReference(input))
        {
            return string.Create(
                input.Length + 1,
                (First: (IntPtr)f, FirstLength: input.Length, Second: value),
                static (destination, state) =>
                {
                    new Span<char>((char*)state.First, state.FirstLength).CopyTo(destination);
                    destination[^1] = state.Second;
                }
            );
        }
    }

    /// <summary>
    /// Concatenate a span to a char
    /// </summary>
    /// <param name="value">Input span</param>
    /// <returns>New string containing input characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string Concat(
        this char input,
        ReadOnlySpan<char> value)
    {
        fixed (char* f = &MemoryMarshal.GetReference(value))
        {
            return string.Create(
                value.Length + 1,
                (First: input, Second: (IntPtr)f, SecondLength: value.Length),
                static (destination, state) =>
                {
                    new Span<char>((char*)state.Second, state.SecondLength).CopyTo(destination[1..]);
                    destination[0] = state.First;
                }
            );
        }
    }

    /// <summary>
    /// Concatenate two spans
    /// </summary>
    /// <param name="rhs">Input span</param>
    /// <returns>New string containing input characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat(
        this ReadOnlySpan<char> lhs,
        ReadOnlySpan<char> rhs) => 
        string.Concat(lhs, rhs);

    #endregion
}