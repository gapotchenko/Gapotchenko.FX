// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Utils;

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    /// <inheritdoc cref="Directory.Delete(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static void DeleteDirectory(this IFileSystemView view, string path) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .DeleteDirectory(path, false);

    #region Copy

    /// <summary>
    /// Copies an existing directory to a new directory.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)"/>
    /// <param name="view">The file system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void CopyDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        CopyDirectory(view, sourcePath, destinationPath, false);

    /// <summary>
    /// Copies an existing directory to a new directory.
    /// Overwriting a directory of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)"/>
    /// <param name="view">The file system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    public static void CopyDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .CopyDirectory(sourcePath, destinationPath, overwrite, VfsCopyOptions.None);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination <see cref="IFileSystemView"/> .
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, IFileSystemView, string, bool, VfsCopyOptions)"/>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath) =>
        CopyDirectory(sourceView, sourcePath, destinationView, destinationPath, false, VfsCopyOptions.None);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination <see cref="IFileSystemView"/>.
    /// Overwriting a directory of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to copy.</param>
    /// <param name="sourcePath">The path of the directory to copy.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to copy the directory to.</param>
    /// <param name="destinationPath">The path of the destination directory in the specified <see cref="IFileSystemView"/>.</param>
    /// <param name="overwrite">
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)" path="//param[@name='overwrite']"/>
    /// </param>
    /// <param name="options">
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)" path="//param[@name='options']"/>
    /// </param>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyDirectoryOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite,
            options);
    }

    #endregion

    #region Move

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the option to specify a new directory name.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool, VfsMoveOptions)"/>
    /// <param name="view">The file system view to move the directory at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        MoveDirectory(view, sourcePath, destinationPath, false);

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the options to specify a new directory name and
    /// to overwrite the destination directory if it already exists.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool, VfsMoveOptions)"/>
    /// <param name="view">The file system view to move the directory at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .MoveDirectory(sourcePath, destinationPath, overwrite, VfsMoveOptions.None);

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the option to specify a new directory name.
    /// </summary>
    /// <inheritdoc cref="MoveDirectory(IFileSystemView, string, IFileSystemView, string, bool, VfsMoveOptions)"/>
    public static void MoveDirectory(
        this IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath) =>
        MoveDirectory(sourceView, sourcePath, destinationView, destinationPath, false, VfsMoveOptions.None);

    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool, VfsMoveOptions)"/>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to move.</param>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to move the directory to.</param>
    /// <param name="destinationPath">The path of the destination directory in the specified <see cref="IFileSystemView"/>.</param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveDirectoryOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite,
            options);
    }

    #endregion
}
