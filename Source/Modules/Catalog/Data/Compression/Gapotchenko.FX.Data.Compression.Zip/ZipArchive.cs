// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
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

    /// <inheritdoc/>
    public bool CanWrite { get; }

    void ValidateWrite()
    {
        if (!CanWrite)
            throw new NotSupportedException("Archive does not support writing.");
    }

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

        var filePathModel = new VfsPathModel(path);
        if (filePathModel.IsNil)
        {
            directoryExists = false;
        }
        else if (filePathModel.IsRoot)
        {
            directoryExists = true;
        }
        else
        {
            var entry = m_UnderlyingArchive.GetEntry(filePathModel.Path);
            if (entry != null)
                return entry;

            directoryExists = filePathModel.Depth == 1;
            if (!directoryExists)
            {
                var directoryPathModel = filePathModel.Clone();
                directoryPathModel.PopBack();

                directoryExists = m_UnderlyingArchive.GetEntry(directoryPathModel.Path + '/') != null;
            }
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
    public void DeleteDirectory(string path) => DeleteDirectory(path, false);

    /// <inheritdoc/>
    public void DeleteDirectory(string path, bool recursive)
    {
        var pathModel = new VfsPathModel(path);
        if (pathModel.IsRoot)
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

        GetDirectoryArchiveEntry(pathModel).Delete();
    }

    ZipArchiveEntry GetDirectoryArchiveEntry(VfsPathModel pathModel)
    {
        if (!pathModel.IsNil)
        {
            var entry = m_UnderlyingArchive.GetEntry(pathModel.Path + '/');
            if (entry != null)
                return entry;
        }

        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(pathModel.ToString()));
    }

    static bool IsDirectoryArchiveEntry(string fullName) => fullName.EndsWith('/');

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
        var pathModel = new VfsPathModel(path);
        if (pathModel.IsNil)
            return false;

        if (considerDirectories)
        {
            if (pathModel.IsRoot)
                return true;
            if (m_UnderlyingArchive.GetEntry(pathModel.Path + "/") != null)
                return true;
        }

        if (considerFiles)
        {
            if (!pathModel.IsRoot && m_UnderlyingArchive.GetEntry(pathModel.Path) != null)
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
        var pathModel = new VfsPathModel(path);
        bool directoryExists = pathModel.IsRoot;

        if (!pathModel.IsNil)
        {
            bool recursive = searchOption == SearchOption.AllDirectories;

            foreach (var entry in m_UnderlyingArchive.Entries)
            {
                string entryPath = entry.FullName;
                var entryPathModel = new VfsPathModel(entryPath);

                bool feasibleHierarchyDepth =
                    recursive
                        ? entryPathModel.Depth > pathModel.Depth
                        : entryPathModel.Depth == pathModel.Depth + 1;
                if (directoryExists && !feasibleHierarchyDepth)
                    continue;

                if (IsDirectoryArchiveEntry(entryPath))
                {
                    // Directory
                    if (enumerateDirectories || !directoryExists)
                    {
                        if (entryPathModel.StartsWith(pathModel))
                        {
                            if (enumerateDirectories && feasibleHierarchyDepth)
                            {
                                string directoryPath = entryPath[..^1];
                                yield return '/' + directoryPath;
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
                        string? fileName = entryPathModel.TryPopBack();
                        if (fileName is null)
                            continue;

                        if (entryPathModel.StartsWith(pathModel))
                        {
                            if (enumerateFiles && feasibleHierarchyDepth)
                                yield return '/' + entryPath;
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
        var model = new VfsPathModel(path);
        if (model.IsNil)
            return path;

        return "/" + model.ToString();
    }

    #endregion

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        m_UnderlyingArchive.Dispose();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly System.IO.Compression.ZipArchive m_UnderlyingArchive;
}
