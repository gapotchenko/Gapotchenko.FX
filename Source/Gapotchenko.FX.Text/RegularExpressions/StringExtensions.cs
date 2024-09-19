// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Text.RegularExpressions;

/// <summary>
/// Provides drop-in alternatives to conventional string functions that allow to use regular expressions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StringExtensions
{
    /// <summary>
    /// Searches the input string for the first occurrence of the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns>An object that contains information about the match when it is successful; <see langword="null"/> otherwise.</returns>
    public static Match? MatchRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string? pattern,
        StringComparison comparisionType)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (pattern == null)
            return null;

        var options =
            RegexOptions.Singleline | RegexOptions.ExplicitCapture |
            comparisionType switch
            {
                StringComparison.CurrentCulture => RegexOptions.None,
                StringComparison.CurrentCultureIgnoreCase => RegexOptions.IgnoreCase,
                StringComparison.InvariantCulture or StringComparison.Ordinal => RegexOptions.CultureInvariant,
                StringComparison.InvariantCultureIgnoreCase or StringComparison.OrdinalIgnoreCase => RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                _ => throw new ArgumentOutOfRangeException(nameof(comparisionType)),
            };

        var match = Regex.Match(input, pattern, options);
        if (match.Success)
            return match;
        else
            return null;
    }

    /// <summary>
    /// Searches the input string for the first occurrence of the specified regular expression pattern.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns>An object that contains information about the match when it is successful; <see langword="null"/> otherwise.</returns>
    public static Match? MatchRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string? pattern) =>
        MatchRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Reports a zero-based index of the first occurrence of the specified regular expression pattern in input string,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns>A zero-based index position of value if the pattern is found, or -1 if it is not.</returns>
    public static int IndexOfRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern,
        StringComparison comparisionType) =>
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
    public static int IndexOfRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern) =>
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
    /// <returns><see langword="true"/> if pattern matches the beginning of input string; otherwise, <see langword="false"/>.</returns>
    public static bool StartsWithRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern,
        StringComparison comparisionType)
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
    /// <returns><see langword="true"/> if pattern matches the beginning of input string; otherwise, <see langword="false"/>.</returns>
    public static bool StartsWithRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern) =>
        StartsWithRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Determines whether the end of input string matches the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns><see langword="true"/> if pattern matches the end of input string; otherwise, <see langword="false"/>.</returns>
    public static bool EndsWithRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern,
        StringComparison comparisionType)
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
    /// <returns><see langword="true"/> if pattern matches the end of input string; otherwise, <see langword="false"/>.</returns>
    public static bool EndsWithRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string pattern) =>
        EndsWithRegex(input, pattern, StringComparison.CurrentCulture);

    /// <summary>
    /// Determines whether input string matches the specified regular expression pattern,
    /// using the specified string comparison type.        
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="comparisionType">The string comparison type.</param>
    /// <returns><see langword="true"/> if input string matches specified pattern; otherwise, <see langword="false"/>.</returns>
    public static bool EqualsRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string? pattern,
        StringComparison comparisionType)
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
    /// <returns><see langword="true"/> if input string matches specified pattern; otherwise, <see langword="false"/>.</returns>
    public static bool EqualsRegex(
        this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string? pattern) =>
        EqualsRegex(input, pattern, StringComparison.CurrentCulture);
}
