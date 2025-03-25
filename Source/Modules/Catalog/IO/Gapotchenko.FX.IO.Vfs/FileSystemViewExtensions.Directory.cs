// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    /// <inheritdoc cref="Directory.Delete(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static void DeleteDirectory(this IFileSystemView view, string path) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .DeleteDirectory(path, false);

    #region Copy

    /// <summary>
    /// Copies an existing directory to a new directory.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool)"/>
    /// <param name="view">The file-system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void CopyDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .CopyDirectory(sourcePath, destinationPath, false);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination <see cref="IFileSystemView"/> .
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, IFileSystemView, string, bool)"/>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath) =>
        CopyDirectory(sourceView, sourcePath, destinationView, destinationPath, false);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination <see cref="IFileSystemView"/>.
    /// Overwriting a directory of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to copy.</param>
    /// <param name="sourcePath">The path of the directory to copy.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to copy the directory to.</param>
    /// <param name="destinationPath">The path of the destination directory in the specified <see cref="IFileSystemView"/>.</param>
    /// <param name="overwrite">
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool)" path="//param[@name='overwrite']"/>
    /// </param>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));

        IOHelper.CopyDirectoryOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite);
    }

    #endregion

    #region Move

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the option to specify a new directory name.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool)"/>
    /// <param name="view">The file-system view to move the directory at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .MoveDirectory(sourcePath, destinationPath, false);

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the option to specify a new directory name.
    /// </summary>
    /// <inheritdoc cref="MoveDirectory(IFileSystemView, string, IFileSystemView, string, bool)"/>
    public static void MoveDirectory(
        this IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath) =>
        MoveDirectory(sourceView, sourcePath, destinationView, destinationPath, false);

    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to move.</param>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to move the directory to.</param>
    /// <param name="destinationPath">The path of the destination directory in the specified <see cref="IFileSystemView"/>.</param>
    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool)"/>
    /// <param name="overwrite"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));

        IOHelper.MoveDirectoryOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite);
    }

    #endregion
}
