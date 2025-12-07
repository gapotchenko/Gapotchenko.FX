// Gapotchenko.FX
//
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
    /// <summary>
    /// Gets the virtual file system storage location in the parent file system,
    /// or <see langword="null"/> if the location is not tracked.
    /// </summary>
    VfsReadOnlyLocation? Location { get; }
}
