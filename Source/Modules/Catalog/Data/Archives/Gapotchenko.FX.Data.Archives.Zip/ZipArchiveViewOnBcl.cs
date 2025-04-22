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
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipArchiveViewOnBcl(System.IO.Compression.ZipArchive archive, bool leaveOpen) :
    ZipArchiveBase,
    IZipArchiveView<System.IO.Compression.ZipArchive>
{
    #region Capabilities

    public override bool CanRead => archive.Mode != ZipArchiveMode.Create;

    public override bool CanWrite => archive.Mode != ZipArchiveMode.Read;

    public override bool SupportsLastWriteTime => true;

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return FileExistsCore(path);
    }

    bool FileExistsCore(in StructuredPath path) => EntryExistsCore(path, true, false);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, true, false);

    public override Stream ReadFile(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();
        return StreamView.WithCapabilities(
            GetFileArchiveEntry(path).Open(),
            true, false, true);
    }

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateFileMode(mode);
        VfsValidationKit.Arguments.ValidateFileAccess(access);
        VfsValidationKit.Arguments.ValidateFileShare(share);

        EnsureCanOpenFile(mode, access);

        var (entry, isNew) = GetFileArchiveEntry(path, mode);
        var stream = entry.Open();
        try
        {
            if (!isNew)
            {
                if (mode is FileMode.Create or FileMode.Truncate)
                    stream.SetLength(0);
                else if (mode is FileMode.Append)
                    stream.Seek(0, SeekOrigin.End);
            }

            return StreamView.WithCapabilities(
                stream,
                (access & FileAccess.Read) != 0 && CanRead,
                (access & FileAccess.Write) != 0 && CanWrite,
                true);
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    public override void DeleteFile(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();
        DeleteFileCore(path);
    }

    void DeleteFileCore(in StructuredPath path)
    {
        GetFileArchiveEntry(path).Delete();
    }

    ZipArchiveEntry GetFileArchiveEntry(in StructuredPath path) =>
        GetFileArchiveEntry(path, FileMode.Open).Value;

    (ZipArchiveEntry Value, bool IsNew) GetFileArchiveEntry(in StructuredPath path, FileMode mode)
    {
        bool directoryExists;
        string? entryName = null;

        var filePathParts = path.Parts.Span;
        if (filePathParts == null)
        {
            directoryExists = false;
        }
        else if (filePathParts.Length == 0)
        {
            directoryExists = true;
        }
        else
        {
            entryName = VfsPathKit.Join(filePathParts);
            var entry = archive.GetEntry(entryName);
            if (entry != null)
            {
                // File exists.

                switch (mode)
                {
                    case FileMode.CreateNew:
                        // File already exists but it was requested to be new.
                        throw new IOException(VfsResourceKit.FileAlreadyExists(path.ToString()));

                    case FileMode.Open or FileMode.OpenOrCreate:
                        // Just open the file.
                        break;

                    case FileMode.Create or FileMode.Truncate or FileMode.Append:
                        // Handled down the line.
                        break;

                    default:
                        throw new SwitchExpressionException(mode);
                }

                return (entry, false);
            }

            directoryExists = filePathParts.Length == 1;
            if (!directoryExists)
                directoryExists = archive.GetEntry(VfsPathKit.Join(filePathParts[..^1]) + VfsPathKit.DirectorySeparatorChar) != null;
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));

        // File does not exist.

        switch (mode)
        {
            case FileMode.Open or FileMode.Truncate:
                // Could not find the file.
                break;

            case FileMode.Create or FileMode.OpenOrCreate or FileMode.CreateNew or FileMode.Append:
                if (entryName is null)
                    break; // invalid file name

                // Ensure that there is no existing directory with the same name.
                if (DirectoryExistsCore(path))
                    throw new UnauthorizedAccessException(VfsResourceKit.AccessToPathIsDenied(path.ToString()));

                // Create a new file.
                return (archive.CreateEntry(entryName), true);

            default:
                throw new SwitchExpressionException(mode);
        }

        string? displayPath = path.ToString();
        throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(displayPath), displayPath);
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return DirectoryExistsCore(path);
    }

    bool DirectoryExistsCore(in StructuredPath path) => EntryExistsCore(path, false, true);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, false, true);

    public override void CreateDirectory(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        string[] pathParts =
            VfsPathKit.Split(path) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        if (DirectoryExistsCore(pathParts))
            return;

        int n = pathParts.Length;
        bool parentExists = n == 1;

        for (int i = 1; i <= n; ++i)
        {
            var subPath = new StructuredPath(pathParts.AsMemory()[..i]);

            if (!parentExists && DirectoryExistsCore(subPath))
                continue;

            if (FileExistsCore(subPath))
                throw new IOException(VfsResourceKit.CannotCreateAlreadyExistingEntry(subPath.ToString()));

            archive.CreateEntry(VfsPathKit.Join(subPath.Parts.Span) + VfsPathKit.DirectorySeparatorChar);
            parentExists = true;
        }
    }

    public override void DeleteDirectory(string path, bool recursive)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();
        DeleteDirectoryCore(path, recursive);
    }

    void DeleteDirectoryCore(in StructuredPath path, bool recursive)
    {
        var pathParts = path.Parts.Span;
        if (pathParts.IsEmpty)
            throw new IOException(VfsResourceKit.AccessToPathIsDenied(path.ToString()));

        if (!recursive)
        {
            // Ensure that the directory is empty before it can be deleted.
            if (EnumerateEntriesCore(path).Any())
                throw new IOException(VfsResourceKit.DirectoryIsNotEmpty(path.ToString()));
        }
        else
        {
            // Recursively clean the directory before deleting it.
            var entryPaths = EnumerateEntriesCore(path).ToList();
            foreach (var entryPathParts in entryPaths)
            {
                if (FileExistsCore(entryPathParts))
                    DeleteFileCore(entryPathParts);
                else
                    DeleteDirectoryCore(entryPathParts, true);
            }
        }

        // Try to delete explicit directory entry from the archive if it exists.
        // It may not exist if the directory being deleted is implicit.
        TryGetDirectoryArchiveEntry(path)?.Delete();
    }

    ZipArchiveEntry? TryGetDirectoryArchiveEntry(in StructuredPath path)
    {
        if (VfsPathKit.Join(path.Parts.Span) is not null and var entryPath)
            return archive.GetEntry(entryPath + VfsPathKit.DirectorySeparatorChar);
        else
            return null;
    }

    static bool IsDirectoryArchiveEntry(string fullName) => fullName.EndsWith(VfsPathKit.DirectorySeparatorChar);

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return EntryExistsCore(path, true, true);
    }

    bool EntryExistsCore(
        in StructuredPath path,
        bool considerFiles,
        bool considerDirectories)
    {
        var pathParts = path.Parts.Span;
        if (pathParts == null)
            return false;

        if (pathParts.Length == 0)
        {
            // The root directory always exists.
            // A root file never exists.
            return considerDirectories;
        }

        if (TryGetArchiveEntry(path, considerFiles, considerDirectories) != null)
            return true;

        if (considerDirectories)
        {
            // Directory may be defined implicitly without having a dedicated archive entry.
            // In this case, we try to enumerate its inner entries to determine whether
            // the directory exists.
            return EnumerateEntriesCore(path, throwWhenDirectoryNotFound: false).Any();
        }
        else
        {
            return false;
        }
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, true, true);

    [StackTraceHidden]
    IEnumerable<string> EnumerateEntriesImpl(string path, string searchPattern, SearchOption searchOption, bool enumerateFiles, bool enumerateDirectories)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateSearchOption(searchOption);

        EnsureCanRead();

        VfsSearchKit.AdjustPatternPath(this, ref path, ref searchPattern);

        var structuredPath = new StructuredPath(path);
        return
            EnumerateEntriesCore(
                structuredPath,
                searchPattern,
                MatchType.Win32,
                MatchCasing.PlatformDefault,
                VfsSearchKit.GetMaxRecursionDepth(searchOption),
                enumerateFiles,
                enumerateDirectories)
            .Select(x => GetOriginallyPrefixedPath(structuredPath, x.Span));
    }

    IEnumerable<ReadOnlyMemory<string>> EnumerateEntriesCore(in StructuredPath path, bool throwWhenDirectoryNotFound = true) =>
        EnumerateEntriesCore(path, null, default, default, 0, true, true, throwWhenDirectoryNotFound);

    IEnumerable<ReadOnlyMemory<string>> EnumerateEntriesCore(
        StructuredPath path,
        string? searchPattern,
        MatchType matchType,
        MatchCasing matchCasing,
        int maxRecursionDepth,
        bool enumerateFiles,
        bool enumerateDirectories,
        bool throwWhenDirectoryNotFound = true)
    {
        // The status of the enumerated directory.
        var directoryStatus = throwWhenDirectoryNotFound
            ? EnumeratedDirectoryStatus.None
            : EnumeratedDirectoryStatus.Found; // pretend that the directory is found

        var pathParts = path.Parts;
        if (pathParts.Span != null)
        {
            if (pathParts.Length == 0)
                directoryStatus = EnumeratedDirectoryStatus.Found;

            // Keep track of duplicates because directories can be defined implicitly
            // without presence of dedicated entries in the archive.
            HashSet<ReadOnlyMemory<string>>? enumeratedDirectories = enumerateDirectories
                ? new(MemoryEqualityComparerForPathParts)
                : null;

            var searchExpression = searchPattern is null
                ? VfsSearchExpression.None
                : new(
                    searchPattern,
                    DirectorySeparatorChar,
                    matchType,
                    VfsSearchKit.GetSearchExpressionOptions(this, matchCasing));

            foreach (var entry in GetArchiveEntriesSnapshot())
            {
                string entryPath = entry.FullName;
                string[]? entryPathParts = VfsPathKit.Split(entryPath);
                if (entryPathParts is null)
                    continue;

                foreach (var foundEntryPath in FindEntries(entryPathParts, IsDirectoryArchiveEntry(entryPath)))
                    yield return foundEntryPath;

                if (directoryStatus == EnumeratedDirectoryStatus.Invalid)
                    break;

                IEnumerable<ReadOnlyMemory<string>> FindEntries(ReadOnlyMemory<string> entryPathParts, bool isDirectory)
                {
                    int recursionDepth = entryPathParts.Length - pathParts.Length - 1;

                    bool? parentDirectoryMatch = null; // assists in directory checking optimizations
                    if (enumerateDirectories && recursionDepth > 0)
                    {
                        var parentDirectoryPathParts = entryPathParts[..^1];
                        if (parentDirectoryPathParts.Span.StartsWith(pathParts.Span, m_PathComparer))
                        {
                            directoryStatus = EnumeratedDirectoryStatus.Found;

                            // The parent directory of the current entry may be implicit,
                            // so we must scan it as if it was explicit.
                            foreach (var foundDirectoryPath in FindEntries(parentDirectoryPathParts, true))
                                yield return foundDirectoryPath;

                            parentDirectoryMatch = true;
                        }
                        else
                        {
                            parentDirectoryMatch = false;
                        }
                    }

                    // Check for existence of the initial directory containing the entry.
                    if (directoryStatus == EnumeratedDirectoryStatus.None &&
                        recursionDepth == -1 &&
                        entryPathParts.Span.SequenceEqual(pathParts.Span, m_PathComparer))
                    {
                        if (isDirectory)
                        {
                            directoryStatus = EnumeratedDirectoryStatus.Found;
                        }
                        else
                        {
                            directoryStatus = EnumeratedDirectoryStatus.Invalid;
                            yield break;
                        }
                    }

                    // Check the hierarchy level of the entry.
                    bool feasibleHierarchyLevel = recursionDepth >= 0 && recursionDepth <= maxRecursionDepth;
                    if (directoryStatus != EnumeratedDirectoryStatus.None && !feasibleHierarchyLevel)
                        yield break;

                    // Check the parent directory of the entry.
                    if (!(parentDirectoryMatch ?? entryPathParts.Span[..^1].StartsWith(pathParts.Span, m_PathComparer)))
                        yield break;

                    // Revisit the hierarchy level feasibility.
                    directoryStatus = EnumeratedDirectoryStatus.Found;
                    if (!feasibleHierarchyLevel)
                        yield break;

                    // Match the name of the entry according to the search pattern.
                    if (!searchExpression.IsMatch(entryPathParts.Span[^1].AsSpan()))
                        yield break;

                    if (isDirectory)
                    {
                        // A directory.
                        if (enumeratedDirectories != null && enumeratedDirectories.Add(entryPathParts))
                            yield return entryPathParts;
                    }
                    else
                    {
                        // A file.
                        if (enumerateFiles)
                            yield return entryPathParts;
                    }
                }
            }
        }

        switch (directoryStatus)
        {
            case EnumeratedDirectoryStatus.None:
                throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));

            case EnumeratedDirectoryStatus.Invalid:
                throw new IOException(VfsResourceKit.InvalidDirectoryName(path.ToString()));
        }
    }

    enum EnumeratedDirectoryStatus
    {
        /// <summary>
        /// The directory is not found.
        /// </summary>
        None,

        /// <summary>
        /// The directory has been found.
        /// </summary>
        Found,

        /// <summary>
        /// The directory path points to a file.
        /// </summary>
        Invalid
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<ReadOnlyMemory<string>> MemoryEqualityComparerForPathParts =>
        m_CachedMemoryEqualityComparerForPathParts ??=
        MemoryEqualityComparer.Create<string>(PathComparer);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<ReadOnlyMemory<string>>? m_CachedMemoryEqualityComparerForPathParts;

    public override DateTime GetLastWriteTime(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();

        return GetLastWriteTimeCore(path);

        DateTime GetLastWriteTimeCore(in StructuredPath path)
        {
            return
                TryGetArchiveEntry(path, true, true)?.LastWriteTime.UtcDateTime ??
                EnumerateEntriesCore(path, false)
                .Select(x => GetLastWriteTimeCore(x))
                .MaxOrDefault(DateTime.MinValue);
        }
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        var entry = GetExplicitArchiveEntry(path, true);
        entry.LastWriteTime = lastWriteTime.ToLocalTime();
    }

    ZipArchiveEntry GetExplicitArchiveEntry(in StructuredPath path, bool considerFiles)
    {
        var entry = TryGetArchiveEntry(path, considerFiles, true);
        if (entry != null)
        {
            return entry;
        }
        else if (path.IsDirectory && FileExistsCore(path.Parts))
        {
            // The path represents a directory but points to a file.
            throw new IOException(VfsResourceKit.InvalidDirectoryName(path.ToString()));
        }
        else if (DirectoryExistsCore(path))
        {
            // The path points to an implicit directory which has no archive entry.
            // Let's make that directory explicit by creating an archive entry for it.
            return archive.CreateEntry(VfsPathKit.Join(path.Parts.Span) + VfsPathKit.DirectorySeparatorChar);
        }
        else
        {
            // The path points to a non-existing entry.
            string? displayPath = path.ToString();
            if (path.Parts is var parts && (parts.Length < 2 || DirectoryExistsCore(parts[..^1])))
                throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(displayPath), displayPath);
            else
                throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(displayPath));
        }
    }

    ZipArchiveEntry? TryGetArchiveEntry(
        in StructuredPath path,
        bool considerFiles,
        bool considerDirectories)
    {
        var pathParts = path.Parts.Span;
        if (pathParts.IsEmpty)
            return null;

        if (considerFiles && path.IsDirectory)
        {
            // Make sure that if the path ends in a trailing slash, it's truly a directory.
            considerFiles = false;

            if (!considerDirectories)
                return null;
        }

        string entryPath = VfsPathKit.Join(pathParts);

        if (considerDirectories)
        {
            if (archive.GetEntry(entryPath + VfsPathKit.DirectorySeparatorChar) is not null and var directoryEntry)
                return directoryEntry;
        }

        if (considerFiles)
        {
            if (archive.GetEntry(entryPath) is not null and var fileEntry)
                return fileEntry;
        }

        return null;
    }

    #endregion

    IEnumerable<ZipArchiveEntry> GetArchiveEntriesSnapshot()
    {
        var entries = archive.Entries;
        if (CanWrite)
            return entries.ToList();
        else
            return entries;
    }

    public override void Dispose()
    {
        if (!leaveOpen)
            archive.Dispose();
    }

    public System.IO.Compression.ZipArchive BaseStorage => archive;
}
