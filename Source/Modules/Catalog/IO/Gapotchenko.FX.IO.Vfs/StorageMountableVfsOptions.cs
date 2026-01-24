// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// The base class for defining options of a virtual file system mountable on a storage.
/// </summary>
[ImmutableObject(true)]
public abstract record StorageMountableVfsOptions : VfsOptions
{
    /// <summary>
    /// Gets or initializes the value indicating whether to track
    /// a virtual file system's location in the parent storage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value of this property affects the presence of <see cref="IStorageMountableVfs.Location"/> value
    /// in a mounted file system.
    /// </para>
    /// <para>
    /// The default value is <see langword="true"/>.
    /// </para>
    /// </remarks>
    public bool TrackLocation { get; init; } = true;
}
