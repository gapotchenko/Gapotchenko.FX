using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
[TestCategory("bcl")]
public sealed class ZipArchiveViewOnBclVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation CreateVfs()
    {
        return new VfsLocation(new ArchiveVfs(new MemoryStream()), "/");
    }

    protected override bool TryRoundTripVfs(ref IFileSystemView vfs)
    {
        var archiveVfs = (ArchiveVfs)UnwrapVfs(vfs, out object? cookie);

        archiveVfs.Dispose();

        var oldStream = archiveVfs.Stream;
        oldStream.Position = 0;

        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        vfs = WrapVfs(new ArchiveVfs(newStream), cookie);
        return true;
    }

    sealed class ArchiveVfs(Stream stream) :
        FileSystemViewProxyKit<IZipArchiveView<System.IO.Compression.ZipArchive>>(
            ZipArchive.CreateView(
                new System.IO.Compression.ZipArchive(stream, ZipArchiveMode.Update, true))),
        IDisposable
    {
        public Stream Stream => stream;

        public void Dispose() => BaseView.Dispose();
    }
}
