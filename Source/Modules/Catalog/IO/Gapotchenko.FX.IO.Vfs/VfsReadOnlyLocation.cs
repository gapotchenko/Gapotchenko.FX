// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// The path of a file-system entry associated with an <see cref="IReadOnlyFileSystemView"/>.
/// </summary>
public readonly struct VfsReadOnlyLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsLocation"/> structure using
    /// the <see cref="FileSystemView.Local">local</see> <see cref="IReadOnlyFileSystemView"/> and
    /// the specified file-system entry path.
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
    /// Initializes a new instance of the <see cref="VfsReadOnlyLocation"/> structure using the specified
    /// <see cref="IReadOnlyFileSystemView"/> and
    /// file-system entry path.
    /// </summary>
    /// <param name="view">The file system view.</param>
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

    /// <summary>
    /// Implicitly converts a file-system entry path to a <see cref="VfsReadOnlyLocation"/>
    /// associated with the local <see cref="IReadOnlyFileSystemView"/>.
    /// </summary>
    /// <param name="path">The path of a file-system entry.</param>
    public static implicit operator VfsReadOnlyLocation(string path) => new(path);
}
