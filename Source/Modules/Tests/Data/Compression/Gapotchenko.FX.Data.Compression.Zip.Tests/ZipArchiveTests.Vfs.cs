using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests;

namespace Gapotchenko.FX.Data.Compression.Zip.Tests;

partial class ZipArchiveTests : FileSystemViewVfsTests
{
    protected override IFileSystemView CreateVfs(out string rootPath)
    {
        rootPath = "/";
        return new ArchiveVfs(new MemoryStream());
    }

    protected override bool TryRoundTripVfs([MaybeNullWhen(false)] ref IFileSystemView vfs)
    {
        var archiveVfs = (ArchiveVfs)vfs;

        DisposeVfs(archiveVfs);

        var oldStream = archiveVfs.Stream;
        oldStream.Position = 0;

        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        vfs = new ArchiveVfs(newStream);
        return true;
    }

    protected override void DisposeVfs(IFileSystemView? vfs) => ((IDisposable?)vfs)?.Dispose();

    sealed class ArchiveVfs(Stream stream) : ZipArchive(stream, true, true)
    {
        public Stream Stream => stream;
    }
}
