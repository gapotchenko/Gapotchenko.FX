using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Tests;

[TestClass]
public sealed class MSCfbVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation CreateVfs()
    {
        var stream = new MemoryStream();
        return new VfsLocation(new MSCfbFileSystem(stream), "/");
    }

    protected override bool TryRoundTripVfs(ref IFileSystemView vfs)
    {
        var testableVfs = (TestableVfs)UnwrapVfs(vfs, out object? cookie);

        testableVfs.Dispose();

        var oldStream = testableVfs.Stream;
        oldStream.Position = 0;

        var newStream = new MemoryStream();
        oldStream.CopyTo(newStream);
        newStream.Position = 0;

        vfs = WrapVfs(new TestableVfs(newStream), cookie);
        return true;
    }

    sealed class TestableVfs :
        FileSystemViewProxyKit<MSCfbFileSystem>,
        IDisposable
    {
        public TestableVfs(Stream stream) :
            this(
                new MSCfbFileSystem(stream, true),
                stream)
        {
        }

        TestableVfs(MSCfbFileSystem baseView, Stream stream) :
            base(baseView)
        {
            Stream = stream;
        }

        public Stream Stream { get; }

        public void Dispose() => BaseView.Dispose();
    }
}
