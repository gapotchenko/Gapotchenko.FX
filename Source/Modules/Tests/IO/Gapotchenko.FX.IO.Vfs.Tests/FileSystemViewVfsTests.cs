// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestCategory("vfs")]
public abstract partial class FileSystemViewVfsTests
{
    // This class is partial. Please take a look at the neighboring source files.

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

    void RunVfsTest(VfsMutatingTest test)
    {
        var vfs = CreateVfs(out string rootPath);
        try
        {
            test(vfs, rootPath);
        }
        finally
        {
            DisposeVfs(vfs);
        }
    }

    void RunVfsTest(VfsMutatingTest mutatingTest, VfsReadOnlyTest readOnlyTest)
    {
        var vfs = CreateVfs(out string rootPath);
        try
        {
            mutatingTest(vfs, rootPath);

            readOnlyTest(vfs, rootPath);
            if (TryRoundTripVfs(ref vfs))
                readOnlyTest(vfs, rootPath);
        }
        finally
        {
            DisposeVfs(vfs);
        }
    }

    delegate void VfsReadOnlyTest(IReadOnlyFileSystemView vfs, string rootPath);

    delegate void VfsMutatingTest(IFileSystemView vfs, string rootPath);

    static void DisposeVfs(IFileSystemView? vfs) => (vfs as IDisposable)?.Dispose();
}
