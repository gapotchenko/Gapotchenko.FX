// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Tests.Kits;

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
[TestCategory("local")]
public sealed class LocalFileSystemViewVfsTests : FileSystemViewVfsTestsKit
{
    protected override IFileSystemView CreateVfs(out string rootPath)
    {
        var vfs = new TempLocalVfs();
        rootPath = vfs.RootPath;
        return vfs;
    }
}
