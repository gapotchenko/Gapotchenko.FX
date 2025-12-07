// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a file-system entry location in <see cref="IFileSystemView"/>.
/// </summary>
public readonly struct VfsLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsLocation"/> struct using the specified path
    /// and <see cref="IFileSystemView"/> of the local file system.
    /// </summary>
    /// <param name="path">The path of a file-system entry.</param>
    [SetsRequiredMembers]
    public VfsLocation(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        View = FileSystemView.Local;
        Path = path;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VfsLocation"/> struct
    /// using the specified path and
    /// <see cref="IFileSystemView"/> instance.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The path of a file-system entry.</param>
    [SetsRequiredMembers]
    public VfsLocation(IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(path);

        View = view;
        Path = path;
    }

    /// <summary>
    /// Gets or initializes the file system view.
    /// </summary>
    public required IFileSystemView View { get; init; }

    /// <summary>
    /// Gets or initializes the path of a file-system entry.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Implicitly converts a <see cref="VfsLocation"/> to a <see cref="VfsReadOnlyLocation"/>.
    /// </summary>
    /// <param name="location">The location.</param>
    public static implicit operator VfsReadOnlyLocation(VfsLocation location) => new(location.View, location.Path);
}
