// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Text;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// The path of a file-system entry associated with an <see cref="IFileSystemView"/>.
/// </summary>
public readonly struct VfsLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsLocation"/> structure using
    /// the <see cref="FileSystemView.Local"/> and
    /// the specified file-system entry path.
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
    /// Initializes a new instance of the <see cref="VfsLocation"/> structure using the specified
    /// <see cref="IFileSystemView"/> and
    /// file-system entry path.
    /// </summary>
    /// <param name="view">The file system view containing a file-system entry.</param>
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
    /// Gets or initializes the file system view containing an entry.
    /// </summary>
    public required IFileSystemView View { get; init; }

    /// <summary>
    /// Gets or initializes the path of a file-system entry.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Implicitly converts a file-system entry path to a <see cref="VfsLocation"/>
    /// associated with the local <see cref="IFileSystemView"/>.
    /// </summary>
    /// <param name="path">The path of a file-system entry.</param>
    public static implicit operator VfsLocation(string path) => new(path);

    /// <summary>
    /// Implicitly converts a <see cref="VfsLocation"/> to a <see cref="VfsReadOnlyLocation"/>.
    /// </summary>
    /// <param name="location">The location.</param>
    public static implicit operator VfsReadOnlyLocation(VfsLocation location) => new(location.View, location.Path);

    /// <summary>
    /// Desconstructs the instance into the view and the path.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="path">The path.</param>
    public void Deconstruct(out IFileSystemView view, out string path)
    {
        view = View;
        path = Path;
    }

    /// <inheritdoc/>
    public override string ToString() => VfsLocationFormatter.GetString(View, Path);
}

/// <summary>
/// The path of a file-system entry associated with an <see cref="IReadOnlyFileSystemView"/>.
/// </summary>
public readonly struct VfsReadOnlyLocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VfsReadOnlyLocation"/> structure using
    /// the <see cref="FileSystemView.Local"/> and
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
    /// <param name="view">The file system view containing a file-system entry.</param>
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
    /// Gets or initializes the read-only file system view containing an entry.
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

    /// <summary>
    /// Desconstructs the instance into the view and the path.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="path">The path.</param>
    public void Deconstruct(out IReadOnlyFileSystemView view, out string path)
    {
        view = View;
        path = Path;
    }

    /// <inheritdoc/>
    public override string ToString() => VfsLocationFormatter.GetString(View, Path);
}

static class VfsLocationFormatter
{
    public static string GetString(IReadOnlyFileSystemView view, string path)
    {
        var sb = new StringBuilder();
        BuildString(sb, view, path);
        return sb.ToString();
    }

    static void BuildString(StringBuilder sb, IReadOnlyFileSystemView view, string path)
    {
        if (view is IStorageMountableVfs mountedFS &&
            mountedFS.Location is { } mountLocation)
        {
            // Storage path
            var storageView = mountLocation.View;
            BuildString(sb, storageView, mountLocation.Path);

            // Directory separator
            sb.Append(storageView.DirectorySeparatorChar);

            // Local path, without a starting directory separator
            sb.Append(path.AsSpan().TrimStart([view.DirectorySeparatorChar, view.AltDirectorySeparatorChar]));
        }
        else
        {
            // Local path only
            sb.Append(path);
        }
    }
}
