// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Tests.Utils;
using Gapotchenko.FX.Threading.Tasks;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

[TestCategory("vfs")]
public abstract partial class FileSystemViewVfsTestKit
{
    // This type is partial.
    // For the rest of the implementation, please take a look at the neighboring source files.

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
    /// <returns>A VFS location where the testing should be performed.</returns>
    protected abstract VfsLocation CreateVfs();

    /// <summary>
    /// Asynchronously creates an empty virtual file system view.
    /// </summary>
    /// <returns>A VFS location where the testing should be performed.</returns>
    protected virtual Task<VfsLocation> CreateVfsAsync(CancellationToken cancellationToken)
    {
        return TaskBridge.ExecuteAsync(CreateVfs, cancellationToken);
    }

    /// <summary>
    /// Round-trips the specified virtual file system by unmounting, disposing, copying, and remounting it.
    /// </summary>
    /// <param name="vfs">
    /// The virtual file system to round trip.
    /// When the method returns <see langword="true"/>, the parameter contains a freshly opened virtual file system,
    /// the previous file system is disposed.
    /// </param>
    /// <returns><see langword="true"/> when round-tripping is supported; otherwise, <see langword="false"/>.</returns>
    protected virtual bool TryRoundTripVfs(ref IFileSystemView vfs) => false;

    protected static IVirtualFileSystem CreateTemporaryVfs(out string rootPath)
    {
        var vfs = new TempLocalVfs();
        rootPath = vfs.RootPath;
        return vfs;
    }

    protected static IFileSystemView UnwrapVfs(IFileSystemView view, out object? cookie)
    {
        if (view is IVfsTransform vt)
        {
            cookie = vt.GetType();
            return vt.UnderlyingVfs;
        }
        else
        {
            cookie = null;
            return view;
        }
    }

    protected static IFileSystemView WrapVfs(IFileSystemView view, object? cookie)
    {
        if (cookie is null)
        {
            return view;
        }
        else if (cookie is Type type)
        {
            if (type == typeof(VfsSyncAsyncSwapTransform))
                return new VfsSyncAsyncSwapTransform(view);
            else
                throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    // ------------------------------------------------------------------------

    void RunVfsTest(VfsTest test) =>
        RunVfsTest(test, (VfsTest?)null);

    void RunVfsTest(VfsTest mutate, VfsTest? verify) =>
        RunVfsTest(
            mutate,
            verify is null
                ? null
                : (vfs, rootPath, phase) => verify(vfs, rootPath));

    void RunVfsTest(VfsTest mutate, VfsPhasedTest? verify)
    {
        RunVfsTest(mutate, verify, CreateVfs(), Fn.Identity);
        RunVfsTest(mutate, verify, TaskBridge.Execute(CreateVfsAsync), x => new VfsSyncAsyncSwapTransform(x));
    }

    void RunVfsTest(
        VfsTest mutate,
        VfsPhasedTest? verify,
        VfsLocation location,
        Func<IFileSystemView, IFileSystemView> transformVfs)
    {
        var (vfs, rootPath) = location;
        try
        {
            vfs = transformVfs(vfs);

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

    delegate void VfsTest(IFileSystemView vfs, string rootPath);

    delegate void VfsPhasedTest(IFileSystemView vfs, string rootPath, int phase);

    // ------------------------------------------------------------------------

    void RunStatefulVfsTest<T>(VfsTestInitialize<T> initialize, VfsStatefulTest<T> mutate, VfsStatefulTest<T>? verify, VfsTestCleanup<T>? cleanup)
    {
        RunStatefulVfsTest(
            initialize,
            mutate,
            verify is null
                ? null
                : (vfs, path, phase, state) => verify(vfs, path, state),
            cleanup);
    }

    void RunStatefulVfsTest<T>(VfsTestInitialize<T> initialize, VfsStatefulTest<T> mutate, VfsStatefulPhasedTest<T>? verify, VfsTestCleanup<T>? cleanup)
    {
        RunStatefulVfsTest(initialize, mutate, verify, cleanup, CreateVfs(), Fn.Identity, null);
        RunStatefulVfsTest(initialize, mutate, verify, cleanup, TaskBridge.Execute(CreateVfsAsync), x => new VfsSyncAsyncSwapTransform(x), null);
    }

    void RunStatefulVfsTest<T>(
        VfsTestInitialize<T> initialize,
        VfsStatefulTest<T> mutate,
        VfsStatefulPhasedTest<T>? verify,
        VfsTestCleanup<T>? cleanup,
        VfsLocation location,
        Func<IFileSystemView, IFileSystemView> transformVfs,
        Func<T, T>? transformState)
    {
        var state = initialize();
        try
        {
            if (transformState is not null)
                state = transformState(state);

            RunVfsTest(
                (vfs, path) => mutate(vfs, path, state),
                verify is null
                    ? null
                    : (vfs, path, phase) => verify(vfs, path, phase, state),
                location,
                transformVfs);
        }
        finally
        {
            cleanup?.Invoke(state);
        }
    }

    delegate T VfsTestInitialize<T>();

    delegate void VfsTestCleanup<T>(T state);

    delegate void VfsStatefulTest<T>(IFileSystemView vfs, string rootPath, T state);

    delegate void VfsStatefulPhasedTest<T>(IFileSystemView vfs, string rootPath, int phase, T state);

    // ------------------------------------------------------------------------

    static void DisposeVfs(IFileSystemView? vfs) => (vfs as IDisposable)?.Dispose();

    #endregion
}
