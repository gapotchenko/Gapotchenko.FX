using System;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Extensions for <see cref="ReadOnlySpan{T}"/>.
/// </summary>
public static class ReadOnlySpanExtensions
{
    /// <summary>
    /// <para>
    /// Determines whether the beginning of the span matches the specified character value.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="span">The span.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns><c>true</c> if the span begins with the value; otherwise, <c>false</c>.</returns>
    public static bool StartsWith(this ReadOnlySpan<char> span, char value) =>
        !span.IsEmpty &&
        span[0] == value;

    /// <summary>
    /// <para>
    /// Determines whether the end of the span matches the specified character value.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="span">The span.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns><c>true</c> if the span ends with the value; otherwise, <c>false</c>.</returns>
    public static bool EndsWith(this ReadOnlySpan<char> span, char value)
    {
        int length = span.Length;
        return
            length != 0 &&
            span[length - 1] == value;
    }
}
