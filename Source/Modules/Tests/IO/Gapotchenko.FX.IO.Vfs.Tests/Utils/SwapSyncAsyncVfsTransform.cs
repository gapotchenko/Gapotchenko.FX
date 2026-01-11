// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.Threading.Tasks;

namespace Gapotchenko.FX.IO.Vfs.Tests.Utils;

/// <summary>
/// Swaps synchronous and asynchronous VFS operations.
/// </summary>
/// <param name="baseView">The base file system view.</param>
sealed class SwapSyncAsyncVfsTransform(IFileSystemView baseView) : FileSystemViewProxyKit(baseView), IVfsTransform
{
    public IFileSystemView UnderlyingVfs => BaseView;

    public void Dispose()
    {
        if (BaseView is IDisposable disposable)
            disposable.Dispose();
    }

    // ------------------------------------------------------------------------

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        return TaskBridge.Execute(cancellationToken => base.FileExistsAsync(path, cancellationToken));
    }

    public override Task<bool> FileExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.FileExists(path), cancellationToken);
    }

    // TODO
}
