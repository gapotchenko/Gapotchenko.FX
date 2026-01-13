// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Utils;

interface IVfsTransform : IFileSystemView, IDisposable
{
    IFileSystemView UnderlyingVfs { get; }
}
