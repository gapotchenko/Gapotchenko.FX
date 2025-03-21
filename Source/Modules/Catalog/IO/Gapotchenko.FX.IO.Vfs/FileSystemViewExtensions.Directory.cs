// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    /// <inheritdoc cref="Directory.Delete(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static void DeleteDirectory(this IFileSystemView view, string path) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .DeleteDirectory(path, false);
}
