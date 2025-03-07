// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO;

/// <summary>
/// Represents a read-only virtual file system view.
/// </summary>
public interface IReadOnlyFileSystemView
{
    #region File

    /// <inheritdoc cref="File.Exists(string?)"/>
    bool FileExists([NotNullWhen(true)] string? path);

    /// <inheritdoc cref="Directory.EnumerateFiles(string)"/>
    IEnumerable<string> EnumerateFiles(string path);

    /// <inheritdoc cref="File.OpenRead(string)"/>
    Stream OpenFileForReading(string path);

    #endregion

    #region Path

    /// <inheritdoc cref="Path.GetFullPath(string)"/>
    string GetFullPath(string path);

    #endregion

    #region Directory

    /// <inheritdoc cref="Directory.Exists(string?)"/>
    bool DirectoryExists([NotNullWhen(true)] string? path);

    /// <inheritdoc cref="Directory.EnumerateDirectories(string)"/>
    IEnumerable<string> EnumerateDirectories(string path);

    /// <inheritdoc cref="FileSystem.EntryExists(string?)"/>
    bool EntryExists([NotNullWhen(true)] string? path);

    #endregion

    #region Entry

    /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string)"/>
    IEnumerable<string> EnumerateEntries(string path);

    #endregion
}
