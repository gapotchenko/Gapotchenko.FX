using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation CreateVfs()
    {
        var stream = new MemoryStream();
        return new VfsLocation(new ArchiveVfs(stream), "/");
    }

    protected override async Task<VfsLocation> CreateVfsAsync(CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        return new VfsLocation(
            await ArchiveVfs.CreateAsync(stream, cancellationToken).ConfigureAwait(false),
            "/");
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

    sealed class ArchiveVfs :
        FileSystemViewProxyKit<ZipArchive>,
        IDisposable
    {
        public ArchiveVfs(Stream stream) :
            this(
                new ZipArchive(stream, stream.CanWrite, true),
                stream)
        {
        }

        ArchiveVfs(ZipArchive baseView, Stream stream) :
            base(baseView)
        {
            Stream = stream;
        }

        public static async Task<ArchiveVfs> CreateAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            return new ArchiveVfs(
                await
                    ZipArchive.CreateAsync(stream, stream.CanWrite, true, cancellationToken: cancellationToken)
                    .ConfigureAwait(false),
                stream);
        }

        public Stream Stream { get; }

        public void Dispose() => BaseView.Dispose();
    }
}
