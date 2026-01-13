// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IVirtualFileSystem"/> proxy.
/// </summary>
/// <typeparam name="T">The type of the base file system.</typeparam>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class VirtualFileSystemProxyKit<T> : FileSystemViewProxyKit<T>, IVirtualFileSystem
    where T : IVirtualFileSystem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualFileSystemProxyKit{T}"/> class with the specified base file system.
    /// </summary>
    /// <param name="baseView">The base file system to create the proxy for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseView"/> is <see langword="null"/>.</exception>
    protected VirtualFileSystemProxyKit(T baseView) :
        base(baseView)
    {
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        BaseView.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        await BaseView.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
