// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a context for virtual file system storage operations.
/// </summary>
public record VfsStorageContext
{
    /// <summary>
    /// Gets or initializes the virtual file system's storage location in the parent file system.
    /// </summary>
    /// <remarks>
    /// The value of this property determines the value of <see cref="IReadOnlyVirtualFileSystem.Location"/> property
    /// in a mounted file system.
    /// </remarks>
    public VfsReadOnlyLocation? Location { get; init; }
}
