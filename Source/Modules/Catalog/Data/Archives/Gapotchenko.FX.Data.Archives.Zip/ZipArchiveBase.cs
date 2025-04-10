﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// This is an infrastructure type that should never be used by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
abstract class ZipArchiveBase : FileSystemViewKit, IZipArchive
{
    private protected ZipArchiveBase()
    {
    }

    #region Paths

    /// <inheritdoc/>
    public sealed override char DirectorySeparatorChar => VfsPathKit.DirectorySeparatorChar;

    /// <inheritdoc/>
    public sealed override StringComparer PathComparer => m_PathComparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private protected static StringComparer m_PathComparer => StringComparer.Ordinal;

    /// <summary>
    /// Gets the path string prefixed by the specified path.
    /// </summary>
    /// <param name="prefixPath">The prefix path.</param>
    /// <param name="parts">The parts of a path to be prefixed.</param>
    /// <returns>The path string prefixed by <paramref name="prefixPath"/>.</returns>
    private protected string GetPrefixedPath(in StructuredPath prefixPath, ReadOnlySpan<string> parts)
    {
        if (prefixPath.OriginalPath is not null and string originalPath)
        {
            int level = prefixPath.Parts.Length;
            if (parts.Length >= level)
                return this.JoinPaths(originalPath, VfsPathKit.Join(parts[level..]));
        }

        return GetFullPathCore(parts);
    }

    /// <inheritdoc/>
    protected sealed override string GetFullPathCore(string path) =>
        GetFullPathCore(VfsPathKit.Split(path)) ??
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

    [return: NotNullIfNotNull(nameof(parts))]
    private protected static string? GetFullPathCore(ReadOnlySpan<string> parts) =>
        parts == null
            ? null!
            : (VfsPathKit.DirectorySeparatorChar + VfsPathKit.Join(parts));

    #endregion

    /// <inheritdoc/>
    public abstract void Dispose();
}
