namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill methods for <see cref="char"/> type.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class CharPolyfills
{
    /// <summary>
    /// <para>
    /// Determines whether this character and a specified <see cref="char"/> value have the same value.
    /// A parameter specifies the culture, case, and sort rules used in the comparison.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="c">This character.</param>
    /// <param name="value">The character to compare to this character.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the characters will be compared.</param>
    /// <returns>
    /// <see langword="true"/> if the value of the <paramref name="value"/> parameter is the same as this character;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals(this char c, char value, StringComparison comparisonType)
    {
        ReadOnlySpan<char> span = stackalloc char[] { c, value };
        return MemoryExtensions.Equals(span.Slice(0, 1), span.Slice(1, 1), comparisonType);
    }
}
