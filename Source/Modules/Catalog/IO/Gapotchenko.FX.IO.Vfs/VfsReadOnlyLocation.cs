// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a file-system entry location in <see cref="IReadOnlyFileSystemView"/>.
/// </summary>
public readonly struct VfsReadOnlyLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsReadOnlyLocation"/> struct using the specified path
    /// and <see cref="IReadOnlyFileSystemView"/> of the local file system.
    /// </summary>
    /// <param name="path">The path of a file-system entry.</param>
    [SetsRequiredMembers]
    public VfsReadOnlyLocation(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        View = FileSystemView.Local;
        Path = path;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsReadOnlyLocation"/> struct
    /// using the specified path and
    /// <see cref="IReadOnlyFileSystemView"/> instance.
    /// </summary>
    /// <param name="view">The read-only file system view.</param>
    /// <param name="path">The path of a file-system entry.</param>
    [SetsRequiredMembers]
    public VfsReadOnlyLocation(IReadOnlyFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(path);

        View = view;
        Path = path;
    }

    /// <summary>
    /// Gets or initializes the read-only file system view.
    /// </summary>
    public IReadOnlyFileSystemView View { get; init; }

    /// <summary>
    /// Gets or initializes the path of a file-system entry.
    /// </summary>
    public required string Path { get; init; }
}
