namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    /// <summary>
    /// Creates an empty virtual file system.
    /// </summary>
    /// <param name="rootPath">The root path to use for tests.</param>
    /// <returns>The <see cref="IFileSystemView"/> instance for the created virtual file system.</returns>
    protected abstract IFileSystemView CreateVfs(out string rootPath);

    /// <summary>
    /// Round-trips the specified virtual file system by unmounting, disposing, copying, and remounting it.
    /// </summary>
    /// <param name="vfs">
    /// The virtual file system to round tripe.
    /// When the method return value is <see langword="true"/>, the parameter contains the freshly opened virtual file system
    /// </param>
    /// <returns><see langword="true"/> when round-tripping is supported; otherwise, <see langword="false"/>.</returns>
    protected virtual bool TryRoundTripVfs([MaybeNullWhen(false)] ref IFileSystemView vfs)
    {
        DisposeVfs(vfs);
        vfs = null;
        return false;
    }

    /// <summary>
    /// Disposes the specified virtual file system.
    /// </summary>
    /// <param name="vfs">The virtual file system.</param>
    protected virtual void DisposeVfs(IFileSystemView? vfs)
    {
    }
}
