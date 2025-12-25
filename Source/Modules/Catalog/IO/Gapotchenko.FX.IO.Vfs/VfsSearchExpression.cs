// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
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
    /// Returns the instance of the <see cref="VfsSearchExpression"/> structure
    /// that represents the absence of expression
    /// and matches everything.
    /// </summary>
    public static VfsSearchExpression None => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure that matches everything.
    /// </summary>
    public VfsSearchExpression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsSearchExpression"/> structure for the specified search expression
    /// with specified match type and options that modify the pattern interpretation.
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
    public VfsSearchExpression(
        string? pattern,
        char directorySeparatorChar = VfsPathKit.DirectorySeparatorChar,
        MatchType matchType = MatchType.Win32,
        VfsSearchExpressionOptions options = VfsSearchExpressionOptions.None)
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
    public bool IsMatch(ReadOnlySpan<char> input) => m_Impl?.IsMatch(input) ?? true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IImpl? m_Impl;
}
