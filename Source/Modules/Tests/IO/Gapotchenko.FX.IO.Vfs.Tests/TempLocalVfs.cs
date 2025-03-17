// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs.Tests;

sealed class TempLocalVfs : FileSystemViewProxyKit, IDisposable
{
    public TempLocalVfs() :
        base(FileSystemView.Local)
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            "Gapotchenko", "Gapotchenko.FX",
            "Tests",
            "Gapotchenko.FX.IO.Vfs.Tests", "VFS",
            Path.GetRandomFileName());

        Directory.CreateDirectory(rootPath);
        RootPath = rootPath;
    }

    public void Dispose()
    {
        Directory.Delete(RootPath, true);
    }

    public string RootPath { get; }
}
