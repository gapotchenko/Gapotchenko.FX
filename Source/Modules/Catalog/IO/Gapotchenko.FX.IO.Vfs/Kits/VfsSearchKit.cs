// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides facilities for implementing search functionality of a virtual file system.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class VfsSearchKit
{
    /// <summary>
    /// Gets a maximum directory depth to recurse while enumerating file-system entries
    /// using the specified <see cref="SearchOption"/>.
    /// </summary>
    /// <param name="searchOption">
    /// One of the enumeration values that specifies whether the search operation
    /// should include only the current directory or
    /// should include all subdirectories. 
    /// </param>
    /// <returns>
    /// The maximum directory depth to recurse while enumerating file-system entries.
    /// When the returned value is <c>0</c>, the enumeration should return the contents of the initial directory.
    /// When the returned value is <see cref="int.MaxValue"/>, the maximum directory depth to recurse is unlimited.
    /// </returns>
    public static int GetMaxRecursionDepth(SearchOption searchOption) =>
        searchOption switch
        {
            SearchOption.TopDirectoryOnly => 0,
            SearchOption.AllDirectories => int.MaxValue
        };

    /// <summary>
    /// Gets a maximum directory depth to recurse while enumerating file-system entries
    /// using the specified <see cref="EnumerationOptions"/>.
    /// </summary>
    /// <param name="enumerationOptions">An object that describes the search and enumeration configuration to use.</param>
    /// <returns>
    /// The maximum directory depth to recurse while enumerating file-system entries.
    /// When the returned value is <c>0</c>, the enumeration should return the contents of the initial directory.
    /// When the returned value is <see cref="int.MaxValue"/>, the recursion depth is unlimited.
    /// </returns>
    public static int GetMaxRecursionDepth(EnumerationOptions enumerationOptions)
    {
        ArgumentNullException.ThrowIfNull(enumerationOptions);

        int maxRecursionDepth;
        if (enumerationOptions.RecurseSubdirectories)
        {
#if NET6_0_OR_GREATER
            maxRecursionDepth = enumerationOptions.MaxRecursionDepth;
            if (maxRecursionDepth < 0)
                maxRecursionDepth = int.MaxValue;
#else
            maxRecursionDepth = int.MaxValue;
#endif
        }
        else
        {
            maxRecursionDepth = 0;
        }
        return maxRecursionDepth;
    }

    /// <summary>
    /// Gets a <see cref="VfsSearchExpressionOptions"/> value
    /// for the specified <see cref="MatchCasing"/>
    /// in the specified file system view.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="matchCasing">The desired match casing.</param>
    /// <returns>
    /// The <see cref="VfsSearchExpressionOptions"/> value
    /// representing the desired <paramref name="matchCasing"/>.
    /// </returns>
    public static VfsSearchExpressionOptions GetSearchExpressionOptions(IReadOnlyFileSystemView view, MatchCasing matchCasing)
    {
        ArgumentNullException.ThrowIfNull(view);

        var effectiveMatchCasing =
            Empty.Nullify(matchCasing, MatchCasing.PlatformDefault) ??
            GetDefaultMatchCasing(view);

        if (effectiveMatchCasing == MatchCasing.CaseInsensitive)
            return VfsSearchExpressionOptions.IgnoreCase;
        else
            return VfsSearchExpressionOptions.None;
    }

    /// <summary>
    /// Gets a value indicating whether the specified <see cref="MatchCasing"/> value
    /// represents the default match casing for the specified file system view.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="matchCasing">The match casing value to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="matchCasing"/> represents 
    /// the default value for <paramref name="view"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsDefaultMatchCasing(IReadOnlyFileSystemView view, MatchCasing matchCasing)
    {
        ArgumentNullException.ThrowIfNull(view);

        return
            matchCasing == MatchCasing.PlatformDefault ||
            matchCasing == GetDefaultMatchCasing(view);
    }

    /// <summary>
    /// Gets the default <see cref="MatchCasing"/> value
    /// for the specified virtual file system view.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <returns>The default <see cref="MatchCasing"/> value.</returns>
    static MatchCasing GetDefaultMatchCasing(IReadOnlyFileSystemView view)
    {
        return IsIgnoreCaseComparison(view.PathComparison)
            ? MatchCasing.CaseInsensitive
            : MatchCasing.CaseSensitive;
    }

    static bool IsIgnoreCaseComparison(StringComparison comparison) =>
        comparison is
            StringComparison.CurrentCultureIgnoreCase or
            StringComparison.InvariantCultureIgnoreCase or
            StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Calculates the effective directory path that should be used for enumerating file-system entries
    /// by the specified search pattern.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <exception cref="ArgumentException">Second path fragment defined by <paramref name="searchPattern"/> must be rooted.</exception>
    public static void AdjustPatternPath(IReadOnlyFileSystemView view, ref string path, ref string searchPattern)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view.IsPathRooted(searchPattern))
            throw new ArgumentException(VfsResourceKit.SecondPathFragmentMustNotBeRooted, nameof(searchPattern));

        string? directoryName = view.GetDirectoryName(searchPattern);
        if (!string.IsNullOrEmpty(directoryName))
        {
            path = view.JoinPaths(path, directoryName);
            searchPattern = view.GetFileName(searchPattern);
        }
    }
}
