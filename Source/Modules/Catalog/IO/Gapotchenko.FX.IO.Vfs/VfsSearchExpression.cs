// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents an immutable file-system search expression
/// which is also colloquially known as wildcard.
/// It can match file and directory names against a specified search pattern.
/// </summary>
public readonly partial struct VfsSearchExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure that matches everything.
    /// </summary>
    public VfsSearchExpression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure for the specified search expression.
    /// </summary>
    /// <inheritdoc cref="VfsSearchExpression(string?, char, VfsSearchExpressionOptions)"/>
    public VfsSearchExpression(string? pattern, char directorySeparatorChar) :
        this(pattern, directorySeparatorChar, VfsSearchExpressionOptions.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure for the specified search expression,
    /// with options that modify the pattern interpretation.
    /// </summary>
    /// <inheritdoc cref="VfsSearchExpression(string?, char, MatchType, VfsSearchExpressionOptions)"/>
    public VfsSearchExpression(string? pattern, char directorySeparatorChar, VfsSearchExpressionOptions options) :
        this(pattern, directorySeparatorChar, MatchType.Win32, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure for the specified search expression,
    /// with match type and options that modify the pattern interpretation.
    /// </summary>
    /// <param name="pattern">
    /// The search expression pattern to match 
    /// or <see langword="null"/> to match everything.
    /// </param>
    /// <param name="directorySeparatorChar">
    /// The character used to separate directory levels in a path string
    /// that reflects a hierarchical file system organization.
    /// </param>
    /// <param name="matchType">The match type.</param>
    /// <param name="options">
    /// A bitwise combination of the enumeration values that modify the search expression behavior.
    /// </param>
    public VfsSearchExpression(string? pattern, char directorySeparatorChar, MatchType matchType, VfsSearchExpressionOptions options)
    {
        if (matchType is not (MatchType.Simple or MatchType.Win32))
            throw new ArgumentOutOfRangeException(nameof(matchType), Resources.EnumValueIsOutOfLegalRange);
        if ((options & ~VfsSearchExpressionOptions.IgnoreCase) != 0)
            throw new ArgumentOutOfRangeException(nameof(options), Resources.EnumValueIsOutOfLegalRange);

        m_Impl = pattern is null
            ? null
            : CreateImpl(pattern, directorySeparatorChar, matchType, options);
    }

    /// <summary>
    /// Indicates whether the search expression
    /// specified in the <see cref="VfsSearchExpression"/> constructor
    /// finds a match in the specified input span.
    /// </summary>
    /// <param name="input">The span to search for a match.</param>
    /// <returns>
    /// <see langword="true"/> if the search expression finds a match;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsMatch(ReadOnlySpan<char> input)
    {
        var impl = m_Impl;
        return impl is null || impl.IsMatch(input);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IImpl? m_Impl;
}
