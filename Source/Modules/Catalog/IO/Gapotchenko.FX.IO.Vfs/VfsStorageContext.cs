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
    /// Gets or initializes the virtual file system storage location in the parent file system.
    /// </summary>
    public VfsReadOnlyLocation? Location { get; init; }
}
