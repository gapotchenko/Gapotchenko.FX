using Gapotchenko.FX.Data.Compression.Zip.Properties;
using Gapotchenko.FX.IO.Vfs;
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
        ZipArchiveMode mode;
        if (writable)
        {
            if (stream.Length == 0)
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

    /// <inheritdoc/>
    public bool CanWrite { get; }

    #region File

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

    ZipArchiveEntry GetFileArchiveEntry(string path)
    {
        bool directoryExists;

        var pathModel = new PathModel(path);
        if (pathModel.IsNil || pathModel.IsRoot)
        {
            directoryExists = true;
        }
        else
        {
            directoryExists = pathModel.HierarchyLevel == 1;

            PathModel directoryModel;
            if (directoryExists)
            {
                directoryModel = new();
            }
            else
            {
                directoryModel = pathModel.Clone();
                directoryModel.TryUp();
            }

            foreach (var entry in m_UnderlyingArchive.Entries)
            {
                string entryPath = entry.FullName;
                if (IsDirectoryArchiveEntry(entryPath))
                {
                    // Directory
                    if (!directoryExists)
                    {
                        var entryPathModel = new PathModel(entryPath);
                        directoryExists = entryPathModel.StartsWith(directoryModel);
                    }
                }
                else
                {
                    // File
                    var entryPathModel = new PathModel(entryPath);
                    if (entryPathModel.Equals(pathModel))
                        return entry;

                    if (!directoryExists)
                    {
                        if (entryPathModel.TryUp() is not null)
                            directoryExists = entryPathModel.StartsWith(directoryModel);
                    }
                }
            }
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(string.Format(Resources.CouldNotFindPartOfPathX, path));
        else
            throw new FileNotFoundException(string.Format(Resources.CouldNotFindFileX, path), path);
    }

    #endregion

    #region Path

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path)
    {
        var model = new PathModel(path);
        if (model.IsNil)
            return path;

        return "/" + model.ToString();
    }

    #endregion

    #region Directory

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

    static bool IsDirectoryArchiveEntry(string fullName) => fullName.EndsWith('/');

    #endregion

    #region Entry

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
        var pathModel = new PathModel(path);
        if (pathModel.IsNil)
            return false;

        if (considerDirectories && pathModel.IsRoot)
            return true;

        foreach (var entry in m_UnderlyingArchive.Entries)
        {
            string entryPath = entry.FullName;
            var entryPathModel = new PathModel(entryPath);

            if (IsDirectoryArchiveEntry(entryPath))
            {
                // Directory
                if (considerDirectories && entryPathModel.StartsWith(pathModel))
                    return true;
            }
            else if (considerFiles)
            {
                // File
                string? fileName = entryPathModel.TryUp();
                if (fileName == null)
                    continue;

                if (entryPathModel.StartsWith(pathModel))
                    return true;
            }
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
        var pathModel = new PathModel(path);
        bool directoryExists = pathModel.IsRoot;

        if (!pathModel.IsNil)
        {
            bool recursive = searchOption == SearchOption.AllDirectories;

            foreach (var entry in m_UnderlyingArchive.Entries)
            {
                string entryPath = entry.FullName;
                var entryPathModel = new PathModel(entryPath);

                bool feasibleHierarchyLevel =
                    recursive
                        ? entryPathModel.HierarchyLevel > pathModel.HierarchyLevel
                        : entryPathModel.HierarchyLevel == pathModel.HierarchyLevel + 1;
                if (directoryExists && !feasibleHierarchyLevel)
                    continue;

                if (IsDirectoryArchiveEntry(entryPath))
                {
                    // Directory
                    if (enumerateDirectories || !directoryExists)
                    {
                        if (entryPathModel.StartsWith(pathModel))
                        {
                            if (enumerateDirectories && feasibleHierarchyLevel)
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
                        string? fileName = entryPathModel.TryUp();
                        if (fileName is null)
                            continue;

                        if (entryPathModel.StartsWith(pathModel))
                        {
                            if (enumerateFiles && feasibleHierarchyLevel)
                                yield return '/' + entryPath;
                            directoryExists = true;
                        }
                    }
                }
            }
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(string.Format(Resources.CouldNotFindPartOfPathX, path));
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
