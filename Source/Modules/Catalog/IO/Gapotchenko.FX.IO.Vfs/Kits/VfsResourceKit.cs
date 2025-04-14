// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides commonly used resources for a virtual file system implementation.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class VfsResourceKit
{
    #region Files

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// Could not find file '{<paramref name="path"/>}.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string CouldNotFindFile(string? path) => string.Format(Resources.CouldNotFindFileX, path);

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// The file '{path}' already exists.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string FileAlreadyExists(string? path) => string.Format(Resources.FileXAlreadyExists, path);

    #endregion

    #region Directories

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// Could not find directory '{<paramref name="path"/>}.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string CouldNotFindDirectory(string? path) => string.Format(Resources.CouldNotFindDirectoryX, path);

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// The directory '{path}' already exists.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string DirectoryAlreadyExists(string? path) => string.Format(Resources.DirectoryXAlreadyExists, path);

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// The directory '{path}' is not empty.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string DirectoryIsNotEmpty(string? path) => string.Format(Resources.DirectoryXIsNotEmpty, path);

    #endregion

    #region Entries

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// Cannot create '{path}' because a file or directory with the same name already exists.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string CannotCreateAlreadyExistingEntry(string? path) => string.Format(Resources.CannotCreateAlreadyExistingEntryX, path);

    #endregion

    #region Paths

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// The path is empty.
    /// </code>
    /// </summary>
    public static string PathIsEmpty => Resources.PathIsEmpty;

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// Could not find a part of the path '{<paramref name="path"/>}'.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string CouldNotFindPartOfPath(string? path) => string.Format(Resources.CouldNotFindPartOfPathX, path);

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// Access to the path '{path}' is denied.
    /// </code>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The formatted string.</returns>
    public static string AccessToPathIsDenied(string? path) => string.Format(Resources.AccessToPathXIsDenied, path);

    #endregion

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// File system does not support reading.
    /// </code>
    /// </summary>
    public static string CannotReadFS => Resources.CannotReadFS;

    /// <summary>
    /// Looks up a localized string similar to:
    /// <code>
    /// File system does not support writing.
    /// </code>
    /// </summary>
    public static string CannotWriteFS => Resources.CannotWriteFS;
}
