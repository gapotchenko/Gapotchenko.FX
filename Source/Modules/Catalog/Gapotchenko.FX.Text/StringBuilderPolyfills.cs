// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_STRINGBUILDER_APPEND_SPAN
#endif

using System.Text;

namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill methods for <see cref="StringBuilder"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StringBuilderPolyfills
{
    /// <summary>
    /// Appends the string representation of a specified read-only character span to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The read-only character span to append.</param>
    /// <returns>A reference to this instance after the append operation is completed.</returns>
#if TFF_STRINGBUILDER_APPEND_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static StringBuilder Append(
#if !TFF_STRINGBUILDER_APPEND_SPAN
        this
#endif
        StringBuilder sb, ReadOnlySpan<char> value)
    {
        ArgumentNullException.ThrowIfNull(sb);

#if TFF_STRINGBUILDER_APPEND_SPAN
        return sb.Append(value);
#else
        foreach (char c in value)
            sb.Append(c);

        return sb;
#endif
    }
}
