using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation CreateVfs()
    {
        var stream = new MemoryStream();
        var vfs = new TestableVfs(stream);
        return new VfsLocation(vfs, $"{vfs.DirectorySeparatorChar}");
    }

    protected override async Task<VfsLocation> CreateVfsAsync(CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        var vfs = await TestableVfs.CreateAsync(stream, cancellationToken).ConfigureAwait(false);
        return new VfsLocation(vfs, $"{vfs.DirectorySeparatorChar}");
    }

    protected override bool TryRoundTripVfs(ref IFileSystemView vfs)
    {
        var archiveVfs = (TestableVfs)UnwrapVfs(vfs, out object? cookie);

        archiveVfs.Dispose();

        var oldStream = archiveVfs.Stream;
        oldStream.Position = 0;

        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        vfs = WrapVfs(new TestableVfs(newStream), cookie);
        return true;
    }

    sealed class TestableVfs : TestableVfsKit<ZipArchive>
    {
        public static async Task<TestableVfs> CreateAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            return new TestableVfs(
                await
                    ZipArchive.CreateAsync(stream, stream.CanWrite, true, cancellationToken: cancellationToken)
                    .ConfigureAwait(false),
                stream);
        }

        public TestableVfs(Stream stream) :
            this(
                new ZipArchive(stream, stream.CanWrite, true),
                stream)
        {
        }

        TestableVfs(ZipArchive baseView, Stream stream) :
            base(baseView, stream)
        {
        }
    }
}
