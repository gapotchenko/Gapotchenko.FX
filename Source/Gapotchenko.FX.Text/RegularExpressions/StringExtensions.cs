using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Text.RegularExpressions;

/// <summary>
/// Provides drop-in alternatives to conventional string functions that allow to use regular expressions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Searches the input string for the first occurrence of the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns>An object that contains information about the match when it is successful; <c>null</c> otherwise.</returns>
    public static Match? MatchRegex(this string input, string? pattern, StringComparison comparisionType)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (pattern == null)
            return null;

        var options = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        switch (comparisionType)
        {
            case StringComparison.CurrentCultureIgnoreCase:
                options |= RegexOptions.IgnoreCase;
                break;

            case StringComparison.InvariantCulture:
            case StringComparison.Ordinal:
                options |= RegexOptions.CultureInvariant;
                break;

            case StringComparison.InvariantCultureIgnoreCase:
            case StringComparison.OrdinalIgnoreCase:
                options |= RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(comparisionType));
        }

        var match = Regex.Match(input, pattern, options);
        if (!match.Success)
            return null;

        return match;
    }

    /// <summary>
    /// Searches the input string for the first occurrence of the specified regular expression pattern.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns>An object that contains information about the match when it is successful; <c>null</c> otherwise.</returns>
    public static Match? MatchRegex(this string input, string? pattern) => MatchRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Reports a zero-based index of the first occurrence of the specified regular expression pattern in input string,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns>A zero-based index position of value if the pattern is found, or -1 if it is not.</returns>
    public static int IndexOfRegex(this string input, string pattern, StringComparison comparisionType) =>
        MatchRegex(
            input,
            pattern ?? throw new ArgumentNullException(nameof(pattern)),
            comparisionType)
        ?.Index ?? -1;

    /// <summary>
    /// Reports a zero-based index of the first occurrence of the specified regular expression pattern in input string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns>A zero-based index position of value if the pattern is found, or -1 if it is not.</returns>
    public static int IndexOfRegex(this string input, string pattern) =>
        IndexOfRegex(
            input,
            pattern ?? throw new ArgumentNullException(nameof(pattern)),
            StringComparison.CurrentCulture);

    /// <summary>
    /// Determines whether the beginning of input string matches the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns><c>true</c> if pattern matches the beginning of input string; otherwise, <c>false</c>.</returns>
    public static bool StartsWithRegex(this string input, string pattern, StringComparison comparisionType)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        return IndexOfRegex(input, "^" + pattern, comparisionType) == 0;
    }

    /// <summary>
    /// Determines whether the beginning of input string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns><c>true</c> if pattern matches the beginning of input string; otherwise, <c>false</c>.</returns>
    public static bool StartsWithRegex(this string input, string pattern) => StartsWithRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Determines whether the end of input string matches the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns><c>true</c> if pattern matches the end of input string; otherwise, <c>false</c>.</returns>
    public static bool EndsWithRegex(this string input, string pattern, StringComparison comparisionType)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        return IndexOfRegex(input, pattern + "$", comparisionType) != -1;
    }

    /// <summary>
    /// Determines whether the end of input string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns><c>true</c> if pattern matches the end of input string; otherwise, <c>false</c>.</returns>
    public static bool EndsWithRegex(this string input, string pattern) => EndsWithRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Determines whether input string matches the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns><c>true</c> if input string matches specified pattern; otherwise, <c>false</c>.</returns>
    public static bool EqualsRegex(this string input, string? pattern, StringComparison comparisionType)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (pattern == null)
            return false;

        return IndexOfRegex(input, "^" + pattern + "$", comparisionType) == 0;
    }

    /// <summary>
    /// Determines whether input string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns><c>true</c> if input string matches specified pattern; otherwise, <c>false</c>.</returns>
    public static bool EqualsRegex(this string input, string? pattern) => EqualsRegex(input, pattern, StringComparison.CurrentCulture);
}
