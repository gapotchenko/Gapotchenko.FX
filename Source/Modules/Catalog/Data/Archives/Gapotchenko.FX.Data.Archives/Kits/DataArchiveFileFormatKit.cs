// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Archives.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IDataArchiveFileFormat{TArchive, TOptions}"/> interface.
/// </summary>
/// <remarks/>
/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class DataArchiveFileFormatKit<TArchive, TOptions> : IDataArchiveFileFormat<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
    /// <inheritdoc/>
    public IReadOnlyList<string> FileExtensions => m_CachedFileExtensions ??= GetFileExtensionsCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<string>? m_CachedFileExtensions;

    /// <inheritdoc cref="FileExtensions"/>
    protected abstract IReadOnlyList<string> GetFileExtensionsCore();

    IVirtualFileSystem IVfsFormat.Create(Stream stream, bool leaveOpen, VfsOptions? options) =>
        Create(stream, leaveOpen, (TOptions?)options);

    /// <inheritdoc/>
    public TArchive Create(Stream stream, bool leaveOpen = false, TOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return CreateCore(stream, leaveOpen, options);
    }

    /// <inheritdoc cref="Create(Stream, bool, TOptions?)"/>
    protected abstract TArchive CreateCore(Stream stream, bool leaveOpen, TOptions? options);

    IVirtualFileSystem IVfsFormat.Mount(Stream stream, bool writable, bool leaveOpen, VfsOptions? options) =>
        Mount(stream, writable, leaveOpen, (TOptions?)options);

    /// <inheritdoc/>
    public TArchive Mount(Stream stream, bool writable = false, bool leaveOpen = false, TOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return MountCore(stream, writable, leaveOpen, options);
    }

    /// <inheritdoc cref="Mount(Stream, bool, bool, TOptions?)"/>
    protected abstract TArchive MountCore(Stream stream, bool writable, bool leaveOpen, TOptions? options);

    bool IVfsFormat.IsMountable(Stream stream, VfsOptions? options) =>
        IsMountable(stream, (TOptions?)options);

    /// <inheritdoc/>
    public bool IsMountable(Stream stream, TOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        bool hasException = false;
        long oldPosition = stream.Position;
        try
        {
            return IsMountableCore(stream, options);
        }
        catch
        {
            hasException = true;
            throw;
        }
        finally
        {
            try
            {
                stream.Position = oldPosition;
            }
            catch (IOException) when (hasException)
            {
            }
        }
    }

    /// <summary>
    /// <inheritdoc cref="IsMountable(Stream, TOptions?)"/>
    /// </summary>
    /// <param name="stream"><inheritdoc cref="IsMountable(Stream, TOptions?)"/></param>
    /// <param name="options"><inheritdoc cref="IsMountable(Stream, TOptions?)"/></param>
    protected abstract bool IsMountableCore(Stream stream, TOptions? options);
}
