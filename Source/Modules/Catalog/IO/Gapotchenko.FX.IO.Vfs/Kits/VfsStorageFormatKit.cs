// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Threading.Tasks;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IVfsStorageFormat{TVfs, TOptions}"/> interface.
/// </summary>
/// <remarks/>
/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class VfsStorageFormatKit<TVfs, TOptions> : IVfsStorageFormat<TVfs, TOptions>
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
    IVirtualFileSystem IVfsStorageFormat.Create(Stream stream, bool leaveOpen, VfsOptions? options, VfsStorageContext? context) =>
        Create(stream, leaveOpen, (TOptions?)options, context);

    async Task<IVirtualFileSystem> IVfsStorageFormat.CreateAsync(Stream stream, bool leaveOpen, VfsOptions? options, VfsStorageContext? context, CancellationToken cancellationToken) =>
        await CreateAsync(stream, leaveOpen, (TOptions?)options, context, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public TVfs Create(Stream stream, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return CreateCore(stream, leaveOpen, options, context);
    }

    /// <inheritdoc/>
    public Task<TVfs> CreateAsync(Stream stream, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return CreateCoreAsync(stream, leaveOpen, options, context, cancellationToken);
    }

    /// <inheritdoc cref="Create(Stream, bool, TOptions?, VfsStorageContext?)"/>
    protected abstract TVfs CreateCore(Stream stream, bool leaveOpen, TOptions? options, VfsStorageContext? context);

    /// <inheritdoc cref="CreateAsync(Stream, bool, TOptions?, VfsStorageContext?, CancellationToken)"/>
    protected virtual Task<TVfs> CreateCoreAsync(Stream stream, bool leaveOpen, TOptions? options, VfsStorageContext? context, CancellationToken cancellationToken)
    {
        return TaskBridge.ExecuteAsync(() => CreateCore(stream, leaveOpen, options, context), cancellationToken);
    }

    IVirtualFileSystem IVfsStorageFormat.Mount(Stream stream, bool writable, bool leaveOpen, VfsOptions? options, VfsStorageContext? context) =>
        Mount(stream, writable, leaveOpen, (TOptions?)options, context);

    /// <inheritdoc/>
    public TVfs Mount(Stream stream, bool writable = false, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return MountCore(stream, writable, leaveOpen, options, context);
    }

    async Task<IVirtualFileSystem> IVfsStorageFormat.MountAsync(Stream stream, bool writable, bool leaveOpen, VfsOptions? options, VfsStorageContext? context, CancellationToken cancellationToken) =>
        await MountAsync(stream, writable, leaveOpen, (TOptions?)options, context, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public Task<TVfs> MountAsync(Stream stream, bool writable = false, bool leaveOpen = false, TOptions? options = null, VfsStorageContext? context = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return MountCoreAsync(stream, writable, leaveOpen, options, context, cancellationToken);
    }

    /// <inheritdoc cref="Mount(Stream, bool, bool, TOptions?, VfsStorageContext?)"/>
    protected abstract TVfs MountCore(Stream stream, bool writable, bool leaveOpen, TOptions? options, VfsStorageContext? context);

    /// <inheritdoc cref="MountAsync(Stream, bool, bool, TOptions?, VfsStorageContext?, CancellationToken)"/>
    protected virtual Task<TVfs> MountCoreAsync(Stream stream, bool writable, bool leaveOpen, TOptions? options, VfsStorageContext? context, CancellationToken cancellationToken)
    {
        return TaskBridge.ExecuteAsync(() => MountCore(stream, writable, leaveOpen, options, context), cancellationToken);
    }

    bool IVfsStorageFormat.IsMountable(Stream stream, VfsOptions? options, VfsStorageContext? context) =>
        IsMountable(stream, (TOptions?)options, context);

    Task<bool> IVfsStorageFormat.IsMountableAsync(Stream stream, VfsOptions? options, VfsStorageContext? context, CancellationToken cancellationToken) =>
        IsMountableAsync(stream, (TOptions?)options, context, cancellationToken);

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

    /// <inheritdoc/>
    public async Task<bool> IsMountableAsync(
        Stream stream,
        TOptions? options = null,
        VfsStorageContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        bool hasException = false;
        long oldPosition = stream.Position;
        try
        {
            return await IsMountableCoreAsync(stream, options, context, cancellationToken).ConfigureAwait(false);
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

    /// <inheritdoc cref="IsMountableAsync(Stream, TOptions?, VfsStorageContext?, CancellationToken)"/>
    protected virtual Task<bool> IsMountableCoreAsync(Stream stream, TOptions? options, VfsStorageContext? context, CancellationToken cancellationToken)
    {
        return TaskBridge.ExecuteAsync(() => IsMountableCore(stream, options, context), cancellationToken);
    }
}
