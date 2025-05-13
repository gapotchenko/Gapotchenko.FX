﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Defines the interface of a mountable read-only virtual file system.
/// </summary>
public interface IReadOnlyVirtualFileSystem : IReadOnlyFileSystemView, IDisposable
{
}
