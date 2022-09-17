using System;
using System.Runtime.CompilerServices;

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
    public static string Concat(
        this ReadOnlySpan<char> input,
        char value)
    {
        Span<char> result = stackalloc char[input.Length + 1];

        input.CopyTo(result);
        result[^1] = value;

        return new string(result);
    }

    /// <summary>
    /// Concatenate a span to a char
    /// </summary>
    /// <param name="value">Input span</param>
    /// <returns>New string containing input characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat(
        this char input,
        ReadOnlySpan<char> value)
    {
        Span<char> result = stackalloc char[value.Length + 1];

        value.CopyTo(result[1..]);
        result[0] = input;

        return new string(result);
    }

    /// <summary>
    /// Concatenate two spans
    /// </summary>
    /// <param name="rhs">Input span</param>
    /// <returns>New string containing input characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat(
        this ReadOnlySpan<char> lhs,
        ReadOnlySpan<char> rhs)
    {
        Span<char> result = stackalloc char[lhs.Length + rhs.Length];

        lhs.CopyTo(result);
        rhs.CopyTo(result[lhs.Length..]);

        return new string(result);
    }

    #endregion
}