// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IDataArchiveFormat{TArchive, TOptions}"/> interface.
/// </summary>
/// <remarks/>
/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class DataArchiveFormatKit<TArchive, TOptions> : IDataArchiveFormat<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
    /// <inheritdoc/>
    public IReadOnlyList<string> FileExtensions => field ??= GetFileExtensionsCore();

    /// <inheritdoc cref="FileExtensions"/>
    protected abstract IReadOnlyList<string> GetFileExtensionsCore();

    IVirtualFileSystem IVfsStorageFormat.Create(Stream stream, bool leaveOpen, VfsOptions? options, VfsStorageContext? context) =>
        Create(stream, leaveOpen, (TOptions?)options, context);

    /// <inheritdoc/>
    public TArchive Create(Stream stream, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return CreateCore(stream, leaveOpen, options, context);
    }

    /// <inheritdoc cref="Create(Stream, bool, TOptions?, VfsStorageContext?)"/>
    protected abstract TArchive CreateCore(Stream stream, bool leaveOpen, TOptions? options, VfsStorageContext? context);

    IVirtualFileSystem IVfsStorageFormat.Mount(Stream stream, bool writable, bool leaveOpen, VfsOptions? options, VfsStorageContext? context) =>
        Mount(stream, writable, leaveOpen, (TOptions?)options, context);

    /// <inheritdoc/>
    public TArchive Mount(Stream stream, bool writable = false, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return MountCore(stream, writable, leaveOpen, options, context);
    }

    /// <inheritdoc cref="Mount(Stream, bool, bool, TOptions?, VfsStorageContext?)"/>
    protected abstract TArchive MountCore(Stream stream, bool writable, bool leaveOpen, TOptions? options, VfsStorageContext? context);

    bool IVfsStorageFormat.IsMountable(Stream stream, VfsOptions? options, VfsStorageContext? context) =>
        IsMountable(stream, (TOptions?)options, context);

    /// <inheritdoc/>
    public bool IsMountable(Stream stream, TOptions? options = null, VfsStorageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        bool hasException = false;
        long oldPosition = stream.Position;
        try
        {
            return IsMountableCore(stream, options, context);
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
                // Do not overshadow the existing exception.
            }
        }
    }

    /// <inheritdoc cref="IsMountable(Stream, TOptions?, VfsStorageContext?)"/>
    protected abstract bool IsMountableCore(Stream stream, TOptions? options, VfsStorageContext? context);
}
