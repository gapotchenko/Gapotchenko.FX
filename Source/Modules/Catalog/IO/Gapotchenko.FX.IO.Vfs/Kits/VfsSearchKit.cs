// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides facilities for implementing search functionality of a virtual file system.
/// </summary>
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
}
