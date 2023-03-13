namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill methods for <see cref="string"/> type.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// <para>
    /// Determines whether the beginning of the string instance matches the specified character value.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns><see langword="true"/> if the string instance begins with the value; otherwise, <see langword="false"/>.</returns>
#if TFF_STRING_OPWITH_CHAR
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool StartsWith(
#if !TFF_STRING_OPWITH_CHAR
        this
#endif
        string s, char value)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        return s.Length != 0 && s[0] == value;
    }

    /// <summary>
    /// <para>
    /// Determines whether the end of the string instance matches the specified character value.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns><see langword="true"/> if the string instance ends with the value; otherwise, <see langword="false"/>.</returns>
#if TFF_STRING_OPWITH_CHAR
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool EndsWith(
#if !TFF_STRING_OPWITH_CHAR
        this
#endif
        string s, char value)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        int n = s.Length;
        return n != 0 && s[n - 1] == value;
    }

    /// <summary>
    /// <para>
    /// Returns a value indicating whether a specified character occurs within this string.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <returns><see langword="true"/> if the <paramref name="value parameter"/> occurs within this string; otherwise, <see langword="false"/>.</returns>
#if TFF_STRING_CONTAINS_CHAR
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool Contains(
#if !TFF_STRING_CONTAINS_CHAR
        this
#endif
        string s, char value)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

#if TFF_STRING_CONTAINS_CHAR
        return s.Contains(value);
#else
        return s.IndexOf(value) != -1;
#endif
    }
}
