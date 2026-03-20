using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

[TestClass]
public sealed class MSCfbVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation OpenVfs(Stream stream)
    {
        var vfs = new TestableVfs(stream);
        return new VfsLocation(vfs, $"{vfs.DirectorySeparatorChar}");
    }

    protected override async Task<VfsLocation> OpenVfsAsync(Stream stream, CancellationToken cancellationToken)
    {
        var vfs = await TestableVfs.CreateAsync(stream, cancellationToken).ConfigureAwait(false);
        return new VfsLocation(vfs, $"{vfs.DirectorySeparatorChar}");
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
