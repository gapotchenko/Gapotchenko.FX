// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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
public class ZipArchive : FileSystemViewKit, IDataArchive, IDisposable
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
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanWrite { get; }

    #endregion

    #region Files

    /// <inheritdoc/>
    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return FileExistsCore(path);
    }

    bool FileExistsCore(in StructuredPath path) => EntryExistsCore(path, true, false);

    /// <inheritdoc/>
    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateSearchOption(searchOption);

        EnsureCanRead();
        return
            EnumerateEntriesCore(path, searchPattern, searchOption, true, false)
            .Select(x => GetFullPathCore(x));
    }

    /// <inheritdoc/>
    public override Stream OpenFileForReading(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();
        return StreamView.WithCapabilities(
            GetFileArchiveEntry(path).Open(),
            true, false, true);
    }

    /// <inheritdoc/>
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

    ZipArchiveEntry GetFileArchiveEntry(in StructuredPath path)
    {
        bool directoryExists;

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
            var entry = m_UnderlyingArchive.GetEntry(VfsPathKit.Join(filePathParts, C_DirectorySeparatorChar));
            if (entry != null)
                return entry;

            directoryExists = filePathParts.Length == 1;
            if (!directoryExists)
                directoryExists = m_UnderlyingArchive.GetEntry(VfsPathKit.Join(filePathParts[..^1], C_DirectorySeparatorChar) + C_DirectorySeparatorChar) != null;
        }

        string? displayPath = path.ToString();
        if (!directoryExists)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(displayPath));
        else
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(displayPath), displayPath);
    }

    #endregion

    #region Directories

    /// <inheritdoc/>
    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return DirectoryExistsCore(path);
    }

    bool DirectoryExistsCore(in StructuredPath path) => EntryExistsCore(path, false, true);

    /// <inheritdoc/>
    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateSearchOption(searchOption);

        EnsureCanRead();
        return
            EnumerateEntriesCore(path, searchPattern, searchOption, false, true)
            .Select(x => GetFullPathCore(x));
    }

    /// <inheritdoc/>
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
            var subPath = pathParts.AsMemory()[..i];

            if (!parentExists && DirectoryExistsCore(subPath))
                continue;

            m_UnderlyingArchive.CreateEntry(VfsPathKit.Join(subPath.Span, C_DirectorySeparatorChar) + C_DirectorySeparatorChar);
            parentExists = true;
        }
    }

    /// <inheritdoc/>
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
            foreach (string[] entryPathParts in entryPaths)
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
        string? entryPath = VfsPathKit.Join(path.Parts.Span, C_DirectorySeparatorChar);
        if (entryPath != null)
        {
            var entry = m_UnderlyingArchive.GetEntry(entryPath + C_DirectorySeparatorChar);
            if (entry != null)
                return entry;
        }

        string? displayPath = path.OriginalPath ?? entryPath;
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(displayPath));
    }

    static bool IsDirectoryArchiveEntry(string fullName) => fullName.EndsWith(C_DirectorySeparatorChar);

    #endregion

    #region Entries

    /// <inheritdoc/>
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

        string entryPath = VfsPathKit.Join(pathParts, C_DirectorySeparatorChar);

        if (considerDirectories)
        {
            if (m_UnderlyingArchive.GetEntry(entryPath + C_DirectorySeparatorChar) != null)
                return true;
        }

        if (considerFiles)
        {
            if (m_UnderlyingArchive.GetEntry(entryPath) != null)
                return true;
        }

        return false;
    }

    IEnumerable<string[]> EnumerateEntriesCore(in StructuredPath path) =>
        EnumerateEntriesCore(path, null, SearchOption.TopDirectoryOnly, true, true);

    /// <inheritdoc/>
    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateSearchOption(searchOption);

        EnsureCanRead();
        return
            EnumerateEntriesCore(path, searchPattern, searchOption, true, true)
            .Select(x => GetFullPathCore(x));
    }

    IEnumerable<string[]> EnumerateEntriesCore(
        StructuredPath path,
        string? searchPattern,
        SearchOption searchOption,
        bool enumerateFiles,
        bool enumerateDirectories)
    {
        searchPattern = Empty.Nullify(searchPattern, "*", StringComparison.Ordinal);

        var pathParts = path.Parts;
        bool directoryExists = false;

        if (pathParts.Span != null)
        {
            directoryExists = pathParts.Length == 0;

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
                        if (entryPathParts.AsSpan().StartsWith(pathParts.Span, m_PathComparer))
                        {
                            if (enumerateDirectories && feasibleHierarchyLevel)
                                yield return entryPathParts;
                            directoryExists = true;
                        }
                    }
                }
                else
                {
                    // File
                    if (enumerateFiles || !directoryExists)
                    {
                        if (entryPathParts.AsSpan()[..^1].StartsWith(pathParts.Span, m_PathComparer))
                        {
                            if (enumerateFiles && feasibleHierarchyLevel)
                                yield return entryPathParts;
                            directoryExists = true;
                        }
                    }
                }
            }
        }

        if (!directoryExists)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));
    }

    #endregion

    #region Paths

    /// <inheritdoc/>
    public sealed override char DirectorySeparatorChar => C_DirectorySeparatorChar;

    /// <inheritdoc/>
    public sealed override StringComparer PathComparer => m_PathComparer;

    static StringComparer m_PathComparer => StringComparer.InvariantCulture;

    /// <inheritdoc/>
    protected sealed override string GetFullPathCore(string path) =>
        GetFullPathCore(VfsPathKit.Split(path)) ??
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

    [return: NotNullIfNotNull(nameof(parts))]
    static string? GetFullPathCore(ReadOnlySpan<string> parts) =>
        parts == null
            ? null!
            : (C_DirectorySeparatorChar + VfsPathKit.Join(parts, C_DirectorySeparatorChar));

    internal const char C_DirectorySeparatorChar = '/';

    #endregion

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        m_UnderlyingArchive.Dispose();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly System.IO.Compression.ZipArchive m_UnderlyingArchive;
}
