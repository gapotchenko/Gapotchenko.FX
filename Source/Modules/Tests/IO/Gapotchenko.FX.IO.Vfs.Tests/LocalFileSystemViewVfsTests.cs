// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Tests.Kits;
using Gapotchenko.FX.IO.Vfs.Tests.Utils;

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
[TestCategory("local")]
public sealed class LocalFileSystemViewVfsTests : FileSystemViewVfsTestKit
{
    protected override VfsLocation CreateVfs()
    {
        var vfs = new TempLocalVfs();
        return new VfsLocation(vfs, vfs.RootPath);
    }
}
