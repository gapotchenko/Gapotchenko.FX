// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
[TestCategory("local")]
public sealed class LocalFileSystemViewVfsTests : FileSystemViewVfsTests
{
    protected override IFileSystemView CreateView(out string rootPath)
    {
        var vfs = new TempLocalVfs();
        rootPath = vfs.RootPath;
        return vfs;
    }
}
