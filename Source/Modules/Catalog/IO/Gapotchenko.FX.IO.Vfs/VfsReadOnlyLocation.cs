// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

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
    /// Gets or initializes the read-only file system view containing a file-system entry.
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
        if (view is IReadOnlyVirtualFileSystem fs &&
            fs.Location is { } storageLocation)
        {
            var storageView = storageLocation.View;
            BuildString(sb, storageView, storageLocation.Path);
            sb.Append(storageView.DirectorySeparatorChar);
        }
        sb.Append(path);
    }
}
