namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
public sealed class LocalFileSystemViewVfsTests : FileSystemViewVfsTests
{
    protected override IFileSystemView CreateVfs(out string rootPath)
    {
        var vfs = new TempLocalVfs();
        rootPath = vfs.RootPath;
        return vfs;
    }
}
