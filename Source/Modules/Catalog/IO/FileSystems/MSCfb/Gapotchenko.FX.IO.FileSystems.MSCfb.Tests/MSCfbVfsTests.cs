using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

[TestClass]
public sealed class MSCfbVfsTests : FileSystemViewVfsTestKit
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
        var testableVfs = (ITestableVfs)UnwrapVfs(vfs, out object? cookie);

        // Unmount the existing file system.
        testableVfs.Dispose();

        // Copy the existing file system image to a new storage.
        var oldStream = testableVfs.Stream;
        oldStream.Position = 0;
        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        // Mount a new file system on the new storage using the copied image.
        vfs = WrapVfs(new TestableVfs(newStream), cookie);
        return true;
    }

    sealed class TestableVfs : TestableVfsKit<MSCfbFileSystem>
    {
        public static async Task<TestableVfs> CreateAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            return new TestableVfs(
                await
                    MSCfbFileSystem.CreateAsync(stream, stream.CanWrite, true, cancellationToken: cancellationToken)
                    .ConfigureAwait(false),
                stream);
        }

        public TestableVfs(Stream stream) :
            base(
                new MSCfbFileSystem(stream, stream.CanWrite, true),
                stream)
        {
        }

        TestableVfs(MSCfbFileSystem baseView, Stream stream) :
            base(baseView, stream)
        {
        }
    }
}
