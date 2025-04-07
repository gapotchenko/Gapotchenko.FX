// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

[TestCategory("vfs")]
public abstract partial class FileSystemViewVfsTestsKit
{
    // This class is partial. Please take a look at the neighboring source files.

    [TestMethod]
    public void FileSystemView_Vfs_Empty()
    {
        RunVfsTest(Verify);

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.IsPathRooted(rootPath));
            Assert.IsTrue(vfs.DirectoryExists(rootPath));
            Assert.IsFalse(vfs.EnumerateEntries(rootPath).Any());
        }
    }

    #region Engine

    /// <summary>
    /// Creates an empty virtual file system view.
    /// </summary>
    /// <param name="rootPath">The root path to use for tests.</param>
    /// <returns>The <see cref="IFileSystemView"/> instance for the created virtual file system.</returns>
    protected abstract IFileSystemView CreateVfs(out string rootPath);

    /// <summary>
    /// Round-trips the specified virtual file system by unmounting, disposing, copying, and remounting it.
    /// </summary>
    /// <param name="vfs">
    /// The virtual file system to round trip.
    /// When the method return value is <see langword="true"/>, the parameter contains the freshly opened virtual file system
    /// </param>
    /// <returns><see langword="true"/> when round-tripping is supported; otherwise, <see langword="false"/>.</returns>
    protected virtual bool TryRoundTripVfs([MaybeNullWhen(false)] ref IFileSystemView vfs)
    {
        DisposeVfs(vfs);
        vfs = null;
        return false;
    }

    protected static IVirtualFileSystem CreateTemporaryVfs(out string rootPath)
    {
        var vfs = new TempLocalVfs();
        rootPath = vfs.RootPath;
        return vfs;
    }

    void RunVfsTest(VfsMutatingTest test) =>
        RunVfsTest(test, (VfsPhasedReadOnlyTest?)null);

    void RunVfsTest(VfsMutatingTest mutate, VfsReadOnlyTest? verify) =>
        RunVfsTest(
            mutate,
            verify is null
                ? null
                : (vfs, rootPath, phase) => verify(vfs, rootPath));

    void RunVfsTest(VfsMutatingTest mutate, VfsPhasedReadOnlyTest? verify)
    {
        var vfs = CreateVfs(out string rootPath);
        try
        {
            mutate(vfs, rootPath);

            if (verify is not null)
            {
                verify(vfs, rootPath, 0);
                if (TryRoundTripVfs(ref vfs))
                    verify(vfs, rootPath, 1);
            }
        }
        finally
        {
            DisposeVfs(vfs);
        }
    }

    delegate void VfsMutatingTest(IFileSystemView vfs, string rootPath);

    delegate void VfsReadOnlyTest(IReadOnlyFileSystemView vfs, string rootPath);

    delegate void VfsPhasedReadOnlyTest(IReadOnlyFileSystemView vfs, string rootPath, int phase);

    static void DisposeVfs(IFileSystemView? vfs) => (vfs as IDisposable)?.Dispose();

    #endregion
}
