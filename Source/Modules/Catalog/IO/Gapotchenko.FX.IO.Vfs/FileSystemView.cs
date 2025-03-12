// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides static methods for working with virtual file system views.
/// </summary>
public static class FileSystemView
{
    /// <summary>
    /// Gets the virtual file system view of the local file system.
    /// </summary>
    public static IFileSystemView Local => LocalFileSystemView.Instance;

    /// <summary>
    /// Gets a view on the specified file system with capabilities enforced according to the specified values.
    /// </summary>
    /// <param name="view">The base file system view to enforce the capabilities for.</param>
    /// <param name="canRead">Indicates whether the file system should support reading.</param>
    /// <param name="canWrite">Indicates whether the file system should support writing.</param>
    /// <returns>
    /// The view on the base file system that enforces the specified capabilities;
    /// otherwise, the base file system <paramref name="view"/> itself if it already matches the requested capabilities
    /// or is <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(view))]
    public static IFileSystemView? WithCapabilities(IFileSystemView? view, bool canRead, bool canWrite)
    {
        if (view is null)
        {
            return null;
        }
        else if (!canRead && view.CanRead ||
            !canWrite && view.CanWrite)
        {
            return new FileSystemViewWithCapabilities(view, canRead, canWrite);
        }
        else
        {
            return view;
        }

    }
}
