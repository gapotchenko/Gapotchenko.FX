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

    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)"/>
    /// <param name="view">The file system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void CopyDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .CopyDirectory(sourcePath, destinationPath, overwrite, options);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination location.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination) =>
        CopyDirectory(sourceView, sourcePath, destination, false, VfsCopyOptions.None);

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination location.
    /// Overwriting a directory of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to copy.</param>
    /// <param name="sourcePath">The path of the directory to copy.</param>
    /// <param name="destination">The destination <see cref="VfsLocation"/> to copy the directory to.</param>
    /// <param name="overwrite">
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)" path="//param[@name='overwrite']"/>
    /// </param>
    /// <param name="options">
    /// <inheritdoc cref="IFileSystemView.CopyDirectory(string, string, bool, VfsCopyOptions)" path="//param[@name='options']"/>
    /// </param>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyDirectoryOptimized(
            new(sourceView, sourcePath),
            destination,
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

    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool, VfsMoveOptions)"/>
    /// <param name="view">The file system view to move the directory at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .MoveDirectory(sourcePath, destinationPath, overwrite, options);

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the option to specify a new directory name.
    /// </summary>
    /// <inheritdoc cref="MoveDirectory(IFileSystemView, string, VfsLocation, bool, VfsMoveOptions)"/>
    public static void MoveDirectory(
        this IFileSystemView sourceView,
        string sourcePath,
        VfsLocation destination) =>
        MoveDirectory(sourceView, sourcePath, destination, false, VfsMoveOptions.None);

    /// <inheritdoc cref="IFileSystemView.MoveDirectory(string, string, bool, VfsMoveOptions)"/>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the directory to move.</param>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destination">The destination <see cref="VfsLocation"/> to move the directory to.</param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void MoveDirectory(
        this IFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveDirectoryOptimized(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options);
    }

    #endregion
}
