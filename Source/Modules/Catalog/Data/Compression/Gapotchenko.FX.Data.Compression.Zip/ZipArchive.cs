// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;
using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Memory;
using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Compression.Zip;

/// <summary>
/// Represents a package of compressed files in the ZIP archive format.
/// </summary>
public class ZipArchive : IDataArchive, IDisposable
{
    /// <summary>
    /// Initializes a new read-only instance of the <see cref="ZipArchive"/> class
    /// from the specified stream.
    /// </summary>
    /// <param name="stream">The stream that contains archive to read.</param>
    public ZipArchive(Stream stream) :
        this(stream, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class
    /// from the specified stream and with the <see cref="CanWrite"/> property set as specified.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="CanWrite"/> property, 
    /// which determines whether the archive supports writing.
    /// </param>
    public ZipArchive(Stream stream, bool writable) :
        this(stream, writable, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class
    /// from the specified stream and with the <see cref="CanWrite"/> property set as specified,
    /// and optionally leaves the stream open.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="CanWrite"/> property, 
    /// which determines whether the archive supports writing.
    /// </param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="ZipArchive"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public ZipArchive(Stream stream, bool writable, bool leaveOpen)
    {
        CanWrite = writable;

        ZipArchiveMode mode;
        if (writable)
        {
            if (!stream.CanSeek || stream.Length == 0)
                mode = ZipArchiveMode.Create;
            else
                mode = ZipArchiveMode.Update;
        }
        else
        {
            mode = ZipArchiveMode.Read;
        }

        m_UnderlyingArchive = new(stream, mode, leaveOpen);
    }

    #region Capabilities

    /// <inheritdoc/>
    public bool CanRead => true;

    void ValidateWrite()
    {
        if (!CanWrite)
            throw new NotSupportedException("Archive does not support writing.");
    }

    /// <inheritdoc/>
    public bool CanWrite { get; }

    #endregion

    #region Files

    /// <inheritdoc/>
    public bool FileExists([NotNullWhen(true)] string? path) => EntryExistsCore(path, true, false);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles(string path) =>
        EnumerateEntriesCore(path, null, SearchOption.TopDirectoryOnly, true, false);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        EnumerateEntriesCore(path, searchPattern, SearchOption.TopDirectoryOnly, true, false);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesCore(path, searchPattern, searchOption, true, false);

    /// <inheritdoc/>
    public Stream OpenFileForReading(string path)
    {
        return StreamView.WithCapabilities(GetFileArchiveEntry(path).Open(), true, false, true);
    }

    /// <inheritdoc/>
    public void DeleteFile(string path)
    {
        ValidateWrite();
        GetFileArchiveEntry(path).Delete();
    }

    ZipArchiveEntry GetFileArchiveEntry(string path)
    {
        bool directoryExists;

        string[]? filePathParts = VfsPathKit.Split(path);
        if (filePathParts is null)
        {
            directoryExists = false;
        }
        else if (filePathParts is [])
        {
            directoryExists = true;
        }
        else
        {
            var entry = m_UnderlyingArchive.GetEntry(VfsPathKit.Combine(filePathParts));
            if (entry != null)
                return entry;

            directoryExists = filePathParts.Length == 1;
            if (!directoryExists)
                directoryExists = m_UnderlyingArchive.GetEntry(VfsPathKit.Combine(filePathParts.AsSpan()[..^1]) + DirectorySeparatorChar) != null;
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));
        else
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);
    }

    #endregion

    #region Directories

    /// <inheritdoc/>
    public bool DirectoryExists([NotNullWhen(true)] string? path) => EntryExistsCore(path, false, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateDirectories(string path) =>
        EnumerateEntriesCore(path, null, SearchOption.TopDirectoryOnly, false, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        EnumerateEntriesCore(path, searchPattern, SearchOption.TopDirectoryOnly, false, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesCore(path, searchPattern, searchOption, false, true);

    /// <inheritdoc/>
    public void CreateDirectory(string path)
    {
        ValidateWrite();

        if (DirectoryExists(path))
            return;

        bool created = false;

        foreach (string subPath in FileSystem.EnumerateSubpaths(path).Reverse())
        {
            if (!created && DirectoryExists(subPath))
                continue;

            m_UnderlyingArchive.CreateEntry(VfsPathKit.Combine(VfsPathKit.Split(subPath)) + DirectorySeparatorChar);
            created = true;
        }
    }

    /// <inheritdoc/>
    public void DeleteDirectory(string path) => DeleteDirectory(path, false);

    /// <inheritdoc/>
    public void DeleteDirectory(string path, bool recursive)
    {
        ValidateWrite();

        string[]? pathParts = VfsPathKit.Split(path);
        if (pathParts is [])
            throw new IOException(VfsResourceKit.AccessToPathIsDenied(path));

        if (!recursive)
        {
            if (EnumerateEntries(path).Any())
                throw new IOException(VfsResourceKit.DirectoryIsNotEmpty(path));
        }
        else
        {
            var entryPaths = EnumerateEntries(path).ToList();
            foreach (string entryPath in entryPaths)
            {
                if (FileExists(entryPath))
                    DeleteFile(entryPath);
                else
                    DeleteDirectory(entryPath, true);
            }
        }

        GetDirectoryArchiveEntry(pathParts).Delete();
    }

    ZipArchiveEntry GetDirectoryArchiveEntry(string[]? pathParts)
    {
        string? path = VfsPathKit.Combine(pathParts);
        if (path != null)
        {
            var entry = m_UnderlyingArchive.GetEntry(path + DirectorySeparatorChar);
            if (entry != null)
                return entry;
        }

        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));
    }

    static bool IsDirectoryArchiveEntry(string fullName) => fullName.EndsWith(DirectorySeparatorChar);

    #endregion

    #region Entries

    /// <inheritdoc/>
    public bool EntryExists([NotNullWhen(true)] string? path) => EntryExistsCore(path, true, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateEntries(string path) =>
        EnumerateEntriesCore(path, null, SearchOption.TopDirectoryOnly, true, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateEntries(string path, string searchPattern) =>
        EnumerateEntriesCore(path, searchPattern, SearchOption.TopDirectoryOnly, true, true);

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesCore(path, searchPattern, searchOption, true, true);

    bool EntryExistsCore(
        [NotNullWhen(true)] string? path,
        bool considerFiles,
        bool considerDirectories)
    {
        string[]? pathParts = VfsPathKit.Split(path);
        if (pathParts is null)
            return false;

        if (pathParts is [])
        {
            // The root directory always exists.
            // The root file never exists.
            return considerDirectories;
        }

        path = VfsPathKit.Combine(pathParts);

        if (considerDirectories)
        {
            if (m_UnderlyingArchive.GetEntry(path + DirectorySeparatorChar) != null)
                return true;
        }

        if (considerFiles)
        {
            if (m_UnderlyingArchive.GetEntry(path) != null)
                return true;
        }

        return false;
    }

    IEnumerable<string> EnumerateEntriesCore(
        string path,
        string? searchPattern,
        SearchOption searchOption,
        bool enumerateFiles,
        bool enumerateDirectories)
    {
        string[]? pathParts = VfsPathKit.Split(path);
        bool directoryExists = pathParts is [];

        if (pathParts is not null)
        {
            bool recursive = searchOption == SearchOption.AllDirectories;

            foreach (var entry in m_UnderlyingArchive.Entries)
            {
                string entryPath = entry.FullName;
                string[]? entryPathParts = VfsPathKit.Split(entryPath);
                if (entryPathParts is null)
                    continue;

                bool feasibleHierarchyLevel =
                    recursive
                        ? entryPathParts.Length > pathParts.Length
                        : entryPathParts.Length == pathParts.Length + 1;
                if (directoryExists && !feasibleHierarchyLevel)
                    continue;

                if (IsDirectoryArchiveEntry(entryPath))
                {
                    // Directory
                    if (enumerateDirectories || !directoryExists)
                    {
                        if (entryPathParts.StartsWith(pathParts, PathComparer))
                        {
                            if (enumerateDirectories && feasibleHierarchyLevel)
                            {
                                string directoryPath = entryPath[..^1];
                                yield return DirectorySeparatorChar + directoryPath;
                            }
                            directoryExists = true;
                        }
                    }
                }
                else
                {
                    // File
                    if (enumerateFiles || !directoryExists)
                    {
                        if (entryPathParts.AsSpan()[..^1].StartsWith(pathParts, PathComparer))
                        {
                            if (enumerateFiles && feasibleHierarchyLevel)
                                yield return DirectorySeparatorChar + entryPath;
                            directoryExists = true;
                        }
                    }
                }
            }
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));
    }

    #endregion

    #region Paths

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path)
    {
        string[]? parts = VfsPathKit.Split(path);
        if (parts is null)
            return path;

        return DirectorySeparatorChar + VfsPathKit.Combine(parts);
    }

    static StringComparer PathComparer => StringComparer.InvariantCulture;

    const char DirectorySeparatorChar = '/';

    #endregion

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        m_UnderlyingArchive.Dispose();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly System.IO.Compression.ZipArchive m_UnderlyingArchive;
}
