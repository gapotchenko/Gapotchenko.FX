// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

public abstract class TestableVfsKit<TFileSystem>(TFileSystem baseView, Stream stream) :
    FileSystemViewProxyKit<TFileSystem>(baseView),
    ITestableVfs
    where TFileSystem : IVirtualFileSystem
{
    public Stream Stream { get; } = stream;

    public void Dispose() => BaseView.Dispose();

    public ValueTask DisposeAsync() => BaseView.DisposeAsync();
}
