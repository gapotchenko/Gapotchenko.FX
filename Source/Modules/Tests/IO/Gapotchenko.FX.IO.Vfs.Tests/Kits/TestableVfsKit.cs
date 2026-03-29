// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

public abstract class TestableVfsKit<TFileSystem>(TFileSystem baseView, Stream stream) :
    VirtualFileSystemProxyKit<TFileSystem>(baseView),
    ITestableVfs
    where TFileSystem : IVirtualFileSystem
{
    public Stream Stream { get; } = stream;
}
