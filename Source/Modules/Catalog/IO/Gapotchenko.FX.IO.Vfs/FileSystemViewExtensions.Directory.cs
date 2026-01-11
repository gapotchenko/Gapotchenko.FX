// Gapotchenko.FX
//
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
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static void DeleteDirectory(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        view.DeleteDirectory(path, false);
    }

    /// <summary>
    /// Asynchronously deletes the specified directory.
    /// </summary>
    /// <inheritdoc cref="DeleteDirectory(IFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous directory deletion operation.</returns>
    public static Task DeleteDirectoryAsync(this IFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.DeleteDirectoryAsync(path, false, cancellationToken);
    }

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
    /// Asynchronously copies an existing directory to a new directory.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyDirectoryAsync(string, string, bool, VfsCopyOptions, CancellationToken)"/>
    /// <param name="view">The file system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task CopyDirectoryAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default) =>
        CopyDirectoryAsync(view, sourcePath, destinationPath, false, cancellationToken: cancellationToken);

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
        VfsCopyOptions options = VfsCopyOptions.None)
    {
        ArgumentNullException.ThrowIfNull(view);

        view.CopyDirectory(sourcePath, destinationPath, overwrite, options);
    }

    /// <inheritdoc cref="IFileSystemView.CopyDirectoryAsync(string, string, bool, VfsCopyOptions, CancellationToken)"/>
    /// <param name="view">The file system view to copy the directory for.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task CopyDirectoryAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.CopyDirectoryAsync(sourcePath, destinationPath, overwrite, options, cancellationToken);
    }

    // ------------------------------------------------------------------------

    /// <summary>
    /// Copies an existing directory to a new directory in the specified destination location.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination) =>
        CopyDirectory(sourceView, sourcePath, destination, false, VfsCopyOptions.None);

    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, VfsLocation)"/>
    [Obsolete("Use CopyDirectory(string, VfsLocation) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destination) =>
        throw new InvalidOperationException();

    /// <summary>
    /// Asynchronously copies an existing directory to a new directory in the specified destination location.
    /// Overwriting a directory of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyDirectoryAsync(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions, CancellationToken)"/>
    public static Task CopyDirectoryAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        CancellationToken cancellationToken = default) =>
        CopyDirectoryAsync(sourceView, sourcePath, destination, false, VfsCopyOptions.None, cancellationToken);

    /// <inheritdoc cref="CopyDirectoryAsync(IReadOnlyFileSystemView, string, VfsLocation, CancellationToken)"/>
    [Obsolete("Use CopyDirectoryAsync(string, VfsLocation, CancellationToken) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyDirectoryAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException();

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

    /// <summary>
    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    /// </summary>
    /// <param name="sourceView"><inheritdoc/></param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destination"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous directory copy operation.</returns>
    public static Task CopyDirectoryAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        return IOHelper.CopyDirectoryOptimizedAsync(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options,
            cancellationToken);
    }

    /// <inheritdoc cref="CopyDirectory(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    [Obsolete("Use CopyDirectory(string, VfsLocation, bool, VfsCopyOptions) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void CopyDirectory(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None) =>
        throw new InvalidOperationException();

    /// <inheritdoc cref="CopyDirectoryAsync(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions, CancellationToken)"/>
    [Obsolete("Use CopyDirectoryAsync(string, VfsLocation, bool, VfsCopyOptions, CancellationToken) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyDirectoryAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException();

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
        VfsMoveOptions options = VfsMoveOptions.None)
    {
        ArgumentNullException.ThrowIfNull(view);

        view.MoveDirectory(sourcePath, destinationPath, overwrite, options);
    }

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
