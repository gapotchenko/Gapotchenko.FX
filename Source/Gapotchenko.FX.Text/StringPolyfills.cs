#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_STRING_GETHASHCODE_COMPARISON
#endif

namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill methods for <see cref="string"/> type.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StringPolyfills
{
    /// <summary>
    /// <para>
    /// Returns a value indicating whether a specified character occurs within this string,
    /// using the specified comparison rules.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value parameter"/> occurs within this string;
    /// otherwise, <see langword="false"/>.
    /// </returns>
#if TFF_STRING_CONTAINS_CHAR
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool Contains(
#if !TFF_STRING_CONTAINS_CHAR
        this
#endif
        string s, char value, StringComparison comparisonType)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

#if TFF_STRING_CONTAINS_CHAR
        return s.Contains(value, comparisonType);
#else
        ReadOnlySpan<char> span = stackalloc char[] { value };
        return s.AsSpan().Contains(span, comparisonType);
#endif
    }

    /// <summary>
    /// <para>
    /// Returns the hash code for this string using the specified rules.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
#if TFF_STRING_GETHASHCODE_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int GetHashCode(
#if !TFF_STRING_GETHASHCODE_COMPARISON
        this
#endif
        string s, StringComparison comparisonType)
    {
#if TFF_STRING_GETHASHCODE_COMPARISON
        return s.GetHashCode(comparisonType);
#else
        var comparer =
            comparisonType switch
            {
                StringComparison.CurrentCulture => StringComparer.CurrentCulture,
                StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
                StringComparison.InvariantCulture => StringComparer.InvariantCulture,
                StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
                StringComparison.Ordinal => StringComparer.Ordinal,
                StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
                _ => throw new ArgumentException("Specified string comparison is not supported.", nameof(comparisonType))
            };

        return comparer.GetHashCode(s);
#endif
    }
}
