// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// The base class for defining options of a virtual file system.
/// </summary>
[ImmutableObject(true)]
public abstract record VfsOptions
{
    /// <summary>
    /// Gets or initializes the value indicating whether to track
    /// a virtual file system's storage location in the parent file system.
    /// </summary>
    /// <remarks>
    /// The value of this property affects the presence of <see cref="IReadOnlyVirtualFileSystem.StorageLocation"/> value
    /// in a mounted file system.
    /// </remarks>
    public bool TrackStorageLocation { get; init; } = true;
}
