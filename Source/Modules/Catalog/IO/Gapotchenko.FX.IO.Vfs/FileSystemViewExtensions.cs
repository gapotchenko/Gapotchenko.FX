// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IFileSystemView"/>.
/// </summary>
public static class FileSystemViewExtensions
{
    /// <inheritdoc cref="File.OpenText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader OpenTextFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(view.OpenFileForReading(path), Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllTextFromFileCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else
            return ReadAllTextFromFileCore(view, path, encoding);
    }

    static string ReadAllTextFromFileCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllText(path, encoding);
        }
        else
        {
            using var sr = new StreamReader(view.OpenFileForReading(path), encoding);
            return sr.ReadToEnd();
        }
    }
}
