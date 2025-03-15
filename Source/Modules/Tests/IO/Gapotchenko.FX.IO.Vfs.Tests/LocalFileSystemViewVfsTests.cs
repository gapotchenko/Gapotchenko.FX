using Gapotchenko.FX.IO.Vfs.Kits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
public sealed class LocalFileSystemViewVfsTests : FileSystemViewVfsTests
{
    protected override IFileSystemView CreateVfs(out string rootPath)
    {
        rootPath = Path.Combine(
            Path.GetTempPath(),
            "Gapotchenko", "Gapotchenko.FX", "Tests", "Gapotchenko.FX.IO.Vfs.Tests", "VFS",
            Path.GetRandomFileName());
        return new LocalVfs(rootPath);
    }

    protected override void DisposeVfs(IFileSystemView? vfs) => ((IDisposable?)vfs)?.Dispose();

    sealed class LocalVfs : FileSystemViewProxyKit, IDisposable
    {
        public LocalVfs(string rootPath) :
            base(FileSystemView.Local)
        {
            Directory.CreateDirectory(rootPath);
            m_RootPath = rootPath;
        }

        public void Dispose()
        {
            Directory.Delete(m_RootPath, true);
        }

        readonly string m_RootPath;
    }
}
