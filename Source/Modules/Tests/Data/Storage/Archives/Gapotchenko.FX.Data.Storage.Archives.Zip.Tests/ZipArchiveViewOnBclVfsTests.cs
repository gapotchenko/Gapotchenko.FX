using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Tests;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Storage.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveViewOnBclVfsTests : FileSystemViewVfsTests
{
    protected override IFileSystemView CreateVfs(out string rootPath)
    {
        rootPath = "/";
        return new ArchiveVfs(new MemoryStream());
    }

    protected override bool TryRoundTripVfs([MaybeNullWhen(false)] ref IFileSystemView vfs)
    {
        var archiveVfs = (ArchiveVfs)vfs;

        archiveVfs.Dispose();

        var oldStream = archiveVfs.Stream;
        oldStream.Position = 0;

        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        vfs = new ArchiveVfs(newStream);
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
