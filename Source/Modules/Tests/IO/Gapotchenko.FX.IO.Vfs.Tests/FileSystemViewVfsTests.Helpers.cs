namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    void RunVfsTest(VfsTest test, VfsTest? postTest = null)
    {
        var vfs = CreateVfs(out string rootPath);
        try
        {
            test(vfs, rootPath);

            if (postTest != null)
            {
                postTest(vfs, rootPath);
                if (TryRoundTripVfs(ref vfs))
                    postTest(vfs, rootPath);
            }
        }
        finally
        {
            DisposeVfs(vfs);
        }
    }

    delegate void VfsTest(IFileSystemView vfs, string rootPath);
}
