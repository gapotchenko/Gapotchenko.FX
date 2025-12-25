// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;

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
    /// Gets a value indicating whether the specified file system view represents local file system.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="view"/> represents local file system;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static bool IsLocal(IReadOnlyFileSystemView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view is LocalFileSystemView;
    }

    /// <summary>
    /// Gets a view on the specified virtual file system with capabilities enforced according to the specified values.
    /// </summary>
    /// <param name="view">The virtual file system view to enforce the capabilities for.</param>
    /// <param name="canRead">Indicates whether the file system should support reading.</param>
    /// <param name="canWrite">Indicates whether the file system should support writing.</param>
    /// <returns>
    /// The view on the specified virtual file system that enforces the specified capabilities;
    /// otherwise, the virtual file-system <paramref name="view"/> itself if it already matches the requested capabilities
    /// or if it is <see langword="null"/>.
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

    /// <summary>
    /// Gets an <see cref="IReadOnlyFileSystemView"/> instance from the specified file system view provider.
    /// </summary>
    /// <remarks>
    /// This method allows to retrieve an <see cref="IReadOnlyFileSystemView"/> instance
    /// from an object implementing <see cref="IReadOnlyFileSystemViewProvider"/> interface.
    /// The interface can be implemented implicitly by the object for the sake of clarity,
    /// which makes <see cref="IReadOnlyFileSystemViewProvider.FileSystemView"/> property unobservable to a user.
    /// By using this method, the user can always retrieve <see cref="IReadOnlyFileSystemView"/> instance provided by the specified object without a need of a type cast.
    /// </remarks>
    /// <param name="provider">The object implementing <see cref="IReadOnlyFileSystemViewProvider"/> interface.</param>
    /// <returns>The <see cref="IReadOnlyFileSystemView"/> instance retrieved from the <paramref name="provider"/> object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null"/>.</exception>
    public static IReadOnlyFileSystemView For(IReadOnlyFileSystemViewProvider provider) =>
        (provider ?? throw new ArgumentNullException(nameof(provider)))
        .FileSystemView;

    /// <summary>
    /// Gets an <see cref="IFileSystemView"/> instance from the specified file system view provider.
    /// </summary>
    /// <remarks>
    /// This method allows to retrieve an <see cref="IFileSystemView"/> instance
    /// from an object implementing <see cref="IFileSystemViewProvider"/> interface.
    /// The interface can be implemented implicitly by the object for the sake of clarity,
    /// which makes <see cref="IFileSystemViewProvider.FileSystemView"/> property unobservable to a user.
    /// By using this method, the user can always retrieve <see cref="IFileSystemView"/> instance provided by the specified object without a need of a type cast.
    /// </remarks>
    /// <param name="provider">The object implementing <see cref="IFileSystemViewProvider"/> interface.</param>
    /// <returns>The <see cref="IFileSystemView"/> instance retrieved from the <paramref name="provider"/> object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null"/>.</exception>
    public static IFileSystemView For(IFileSystemViewProvider provider) =>
        (provider ?? throw new ArgumentNullException(nameof(provider)))
        .FileSystemView;

    /// <inheritdoc cref="For(IReadOnlyFileSystemViewProvider)"/>
    /// <exception cref="ArgumentException">The object does not implement <see cref="IReadOnlyFileSystemViewProvider"/> interface.</exception>
    public static IReadOnlyFileSystemView For(IReadOnlyFileSystemViewProvisionIntent provider) =>
        ((provider ?? throw new ArgumentNullException(nameof(provider)))
        as IReadOnlyFileSystemViewProvider
        ?? throw new ArgumentException(
            ResourceHelper.ObjectDoesNotImplInterface(nameof(IReadOnlyFileSystemViewProvider)),
            nameof(provider)))
        .FileSystemView;

    /// <inheritdoc cref="For(IFileSystemViewProvider)"/>
    /// <exception cref="ArgumentException">The object does not implement <see cref="IFileSystemViewProvider"/> interface.</exception>
    public static IFileSystemView For(IFileSystemViewProvisionIntent provider) =>
        ((provider ?? throw new ArgumentNullException(nameof(provider)))
        as IFileSystemViewProvider
        ?? throw new ArgumentException(
            ResourceHelper.ObjectDoesNotImplInterface(nameof(IFileSystemViewProvider)),
            nameof(provider)))
        .FileSystemView;
}
