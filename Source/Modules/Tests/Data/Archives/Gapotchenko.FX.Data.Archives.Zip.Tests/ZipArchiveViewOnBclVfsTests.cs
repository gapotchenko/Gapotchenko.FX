using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
[TestCategory("bcl")]
public sealed class ZipArchiveViewOnBclVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation OpenVfs(Stream stream)
    {
        var vfs = new TestableVfs(stream);
        return new VfsLocation(vfs, $"{vfs.DirectorySeparatorChar}");
    }

    sealed class TestableVfs : TestableVfsKit<IZipArchiveView<System.IO.Compression.ZipArchive>>
    {
        public TestableVfs(Stream stream) :
            this(
                ZipArchive.CreateView(new System.IO.Compression.ZipArchive(stream, ZipArchiveMode.Update, true)),
                stream)
        {
        }

        TestableVfs(IZipArchiveView<System.IO.Compression.ZipArchive> baseView, Stream stream) :
        base(baseView, stream)
        {
        }
    }
}
