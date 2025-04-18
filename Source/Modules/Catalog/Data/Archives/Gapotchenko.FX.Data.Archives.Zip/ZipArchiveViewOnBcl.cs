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
        if (pathParts != null && pathParts.Length == 0)
            throw new IOException(VfsResourceKit.AccessToPathIsDenied(path.ToString()));

        if (!recursive)
        {
            if (EnumerateEntriesCore(path).Any())
                throw new IOException(VfsResourceKit.DirectoryIsNotEmpty(path.ToString()));
        }
        else
        {
            var entryPaths = EnumerateEntriesCore(path).ToList();
            foreach (var entryPathParts in entryPaths)
            {
                if (FileExistsCore(entryPathParts))
                    DeleteFileCore(entryPathParts);
                else
                    DeleteDirectoryCore(entryPathParts, true);
            }
        }

        GetDirectoryArchiveEntry(path).Delete();
    }

    ZipArchiveEntry GetDirectoryArchiveEntry(in StructuredPath path)
    {
        string? entryPath = VfsPathKit.Join(path.Parts.Span);
        if (entryPath != null)
        {
            var entry = archive.GetEntry(entryPath + VfsPathKit.DirectorySeparatorChar);
            if (entry != null)
                return entry;
        }

        string? displayPath = path.OriginalPath ?? entryPath;
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(displayPath));
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
            // The root file never exists.
            return considerDirectories;
        }

        if (TryGetArchiveEntry(path, considerFiles, considerDirectories) != null)
            return true;

        if (considerDirectories)
        {
            // Directory may be implicit without having a dedicated archive entry.
            return EnumerateEntriesCore(path, throwWhenDirectoryNotFound: false).Any();
        }
        else
        {
            return false;
        }
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, true, true);

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
        EnumerateEntriesCore(path, null, MatchType.Simple, MatchCasing.CaseSensitive, 0, true, true, throwWhenDirectoryNotFound);

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
        var pathParts = path.Parts;
        bool directoryFound = !throwWhenDirectoryNotFound;

        if (pathParts.Span != null)
        {
            directoryFound |= pathParts.Length == 0;

            // Keep track of duplicates because directories can be defined implicitly
            // as a matter of fact without dedicated entries being present in the archive.
            HashSet<ReadOnlyMemory<string>>? enumeratedDirectories = enumerateDirectories ? new(PathMemoryEqualityComparer!) : null;

            var searchExpression = new VfsSearchExpression(
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

                foreach (var foundEntryPath in FindEntries(entryPathParts))
                    yield return foundEntryPath;

                IEnumerable<ReadOnlyMemory<string>> FindEntries(
                    ReadOnlyMemory<string> entryPathParts,
                    bool assumeDirectory = false)
                {
                    int recursionDepth = entryPathParts.Length - pathParts.Length - 1;

                    if (enumerateDirectories && recursionDepth > 0)
                    {
                        // The parent directory of the current entry may be implicit,
                        // so we must scan it as if it was explicit.
                        foreach (var foundDirectoryPath in FindEntries(entryPathParts[..^1], true))
                            yield return foundDirectoryPath;
                    }

                    bool feasibleHierarchyLevel = recursionDepth >= 0 && recursionDepth <= maxRecursionDepth;
                    if (directoryFound && !feasibleHierarchyLevel)
                        yield break;

                    if (assumeDirectory || IsDirectoryArchiveEntry(entryPath))
                    {
                        // Directory
                        if (enumerateDirectories || !directoryFound)
                        {
                            if (entryPathParts.Span.StartsWith(pathParts.Span, m_PathComparer))
                            {
                                if (enumeratedDirectories != null && feasibleHierarchyLevel)
                                {
                                    if (searchExpression.IsMatch(entryPathParts.Span[^1].AsSpan()))
                                    {
                                        if (enumeratedDirectories.Add(entryPathParts))
                                            yield return entryPathParts;
                                    }
                                }
                                directoryFound = true;
                            }
                        }
                    }
                    else
                    {
                        // File
                        if (enumerateFiles || !directoryFound)
                        {
                            if (entryPathParts.Span[..^1].StartsWith(pathParts.Span, m_PathComparer))
                            {
                                if (enumerateFiles && feasibleHierarchyLevel)
                                {
                                    if (searchExpression.IsMatch(entryPathParts.Span[^1].AsSpan()))
                                        yield return entryPathParts;
                                }
                                directoryFound = true;
                            }
                        }
                    }
                }
            }
        }

        if (!directoryFound)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<ReadOnlyMemory<string?>> PathMemoryEqualityComparer =>
        m_CachedPathMemoryEqualityComparer ??=
        MemoryEqualityComparer.Create(PathComparer);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<ReadOnlyMemory<string?>>? m_CachedPathMemoryEqualityComparer;

    public override DateTime GetLastWriteTime(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        return
            TryGetArchiveEntry(path, true, true)?.LastWriteTime.UtcDateTime ??
            DateTime.MinValue;
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        var entry =
            TryGetArchiveEntry(path, true, true) ??
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);

        entry.LastWriteTime = lastWriteTime.ToLocalTime();
    }

    ZipArchiveEntry? TryGetArchiveEntry(
        in StructuredPath path,
        bool considerFiles,
        bool considerDirectories)
    {
        var pathParts = path.Parts.Span;
        if (pathParts.IsEmpty)
            return null;

        if (considerFiles &&
            path.OriginalPath is not null and var originalPath &&
            originalPath.Length > 0 &&
            VfsPathKit.IsDirectorySeparator(originalPath[^1]))
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
