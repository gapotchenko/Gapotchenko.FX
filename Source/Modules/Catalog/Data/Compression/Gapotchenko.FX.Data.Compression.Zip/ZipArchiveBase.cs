// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Compression.Zip;

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
    public sealed override char DirectorySeparatorChar => C_DirectorySeparatorChar;

    /// <inheritdoc/>
    public sealed override StringComparer PathComparer => m_PathComparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private protected static StringComparer m_PathComparer => StringComparer.InvariantCulture;

    /// <inheritdoc/>
    protected sealed override string GetFullPathCore(string path) =>
        GetFullPathCore(VfsPathKit.Split(path)) ??
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

    [return: NotNullIfNotNull(nameof(parts))]
    private protected static string? GetFullPathCore(ReadOnlySpan<string> parts) =>
        parts == null
            ? null!
            : (C_DirectorySeparatorChar + VfsPathKit.Join(parts, C_DirectorySeparatorChar));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal const char C_DirectorySeparatorChar = '/';

    #endregion

    /// <inheritdoc/>
    public abstract void Dispose();
}
