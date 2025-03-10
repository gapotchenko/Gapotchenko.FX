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
    /// <param name="fileSystem">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader OpenTextFile(this IFileSystemView fileSystem, string path)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        else if (fileSystem is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(fileSystem.OpenFileForReading(path), Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="fileSystem">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IFileSystemView fileSystem, string path)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        else if (fileSystem is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllTextFromFileCore(fileSystem, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="fileSystem">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IFileSystemView fileSystem, string path, Encoding encoding)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        else
            return ReadAllTextFromFileCore(fileSystem, path, encoding);
    }

    static string ReadAllTextFromFileCore(IFileSystemView fileSystem, string path, Encoding encoding)
    {
        if (fileSystem is LocalFileSystemView)
        {
            return File.ReadAllText(path, encoding);
        }
        else
        {
            using var sr = new StreamReader(fileSystem.OpenFileForReading(path), encoding);
            return sr.ReadToEnd();
        }
    }
}
