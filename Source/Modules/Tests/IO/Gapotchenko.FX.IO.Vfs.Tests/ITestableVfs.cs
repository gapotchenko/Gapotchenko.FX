// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Vfs.Tests;

public interface ITestableVfs : IVirtualFileSystem
{
    /// <summary>
    /// Gets an underlying stream the file system is mounted on.
    /// </summary>
    Stream Stream { get; }
}
