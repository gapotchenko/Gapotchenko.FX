// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;
using Gapotchenko.FX.Memory;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

/// <summary>
/// Represents an MS-CFB (Microsoft Compound File Binary) virtual file system.
/// </summary>
public sealed partial class MSCfbFileSystem :
    VirtualFileSystemKit,
    IMSCfbFileSystem,
    IStorageMountableFileSystem<IMSCfbFileSystem, MSCfbFileSystemOptions>
{
    #region Paths

    /// <inheritdoc/>
    public override char DirectorySeparatorChar => base.DirectorySeparatorChar;

    /// <inheritdoc/>
    public override char AltDirectorySeparatorChar => base.AltDirectorySeparatorChar;

    /// <inheritdoc/>
    public override StringComparer PathComparer => StringComparer.OrdinalIgnoreCase;

    /// <inheritdoc/>
    public override StringComparison PathComparison => StringComparison.OrdinalIgnoreCase;

    /// <inheritdoc/>
    public VfsReadOnlyLocation? Location { get; private set; }

    string GetOriginallyPrefixedPath(in StructuredPath prefixPath, ReadOnlySpan<string> parts)
    {
        if (prefixPath.OriginalPath is not null and string originalPath)
        {
            var prefixParts = prefixPath.Parts.Span;
            if (parts.StartsWith(prefixParts, PathComparer))
                return this.JoinPaths(originalPath, VfsPathKit.Join(parts[prefixParts.Length..]));
        }

        return GetFullPathCore(parts);
    }

    /// <inheritdoc/>
    protected override string GetFullPathCore(string path) =>
        GetFullPathCore(VfsPathKit.Split(path)) ??
        throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

    [return: NotNullIfNotNull(nameof(parts))]
    static string? GetFullPathCore(ReadOnlySpan<string> parts) =>
        parts == null
            ? null!
            : (VfsPathKit.DirectorySeparatorChar + VfsPathKit.Join(parts));

    #endregion

    #region Capabilities

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanWrite => m_Writable;

    /// <inheritdoc/>
    public override bool SupportsCreationTime => true;

    /// <inheritdoc/>
    public override bool SupportsLastWriteTime => true;

    #endregion

    #region Files

    /// <inheritdoc/>
    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();

        var structuredPath = new StructuredPath(path);
        var parts = structuredPath.Parts.Span;
        if (parts.IsEmpty)
            return false;

        var entry = TryFindEntry(parts);
        return
            EntryExists(structuredPath, entry) &&
            entry.Type == CfbEntryType.Stream;
    }

    /// <inheritdoc/>
    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, true, false);

    /// <inheritdoc/>
    public override long GetFileSize(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();

        var parts = new StructuredPath(path).Parts.Span;
        if (parts == null || parts.IsEmpty)
            ThrowFileNotFound(path);

        var entry = TryFindEntry(parts);
        if (entry?.Type != CfbEntryType.Stream)
            ThrowFileNotFound(path);

        return entry.Size;
    }

    /// <inheritdoc/>
    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateFileMode(mode);
        VfsValidationKit.Arguments.ValidateFileAccess(access);
        VfsValidationKit.Arguments.ValidateFileShare(share);

        EnsureCanOpenFile(mode, access);

        var parts = new StructuredPath(path).Parts.Span;
        if (parts.IsEmpty)
            ThrowFileNotFound(path);

        var parent = TryNavigateToStorage(parts[..^1]);
        if (parent is null)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        var entry = m_Context.TryFindChild(parent, parts[^1]);
        bool writeable = (access & FileAccess.Write) != 0;

        if (entry is not null)
        {
            if (entry.Type != CfbEntryType.Stream)
                throw new UnauthorizedAccessException(VfsResourceKit.AccessToPathIsDenied(path));

            if (mode == FileMode.CreateNew)
                throw new IOException(VfsResourceKit.FileAlreadyExists(path));

            if (writeable)
                return m_Context.OpenWriteStream(entry, mode, access, false);
            else
                return m_Context.OpenReadStream(entry);
        }
        else
        {
            if (mode is FileMode.Open or FileMode.Truncate)
                ThrowFileNotFound(path);

            if (!writeable)
                ThrowFileNotFound(path);

            // Create the stream entry and return a write stream.
            entry = m_Context.AddChild(parent, parts[^1], CfbEntryType.Stream);
            return m_Context.OpenWriteStream(entry, mode, access, true);
        }
    }

    /// <inheritdoc/>
    public override void DeleteFile(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        var parts = new StructuredPath(path).Parts.Span;
        if (parts.IsEmpty)
            ThrowFileNotFound(path);

        var parent =
            TryNavigateToStorage(parts[..^1]) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        var entry = m_Context.TryFindChild(parent, parts[^1]);
        if (entry is null || entry.Type != CfbEntryType.Stream)
            ThrowFileNotFound(path);

        m_Context.RemoveChild(parent, entry);
    }

    [DoesNotReturn]
    static void ThrowFileNotFound(string? path) =>
        throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);

    #endregion

    #region Directories

    /// <inheritdoc/>
    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();

        var structuredPath = new StructuredPath(path);

        var parts = structuredPath.Parts.Span;
        if (parts == null)
            return false;
        if (parts.IsEmpty)
            return true;

        var entry = TryFindEntry(parts);
        return
            EntryExists(structuredPath, entry) &&
            entry.Type is CfbEntryType.Root or CfbEntryType.Storage;
    }

    /// <inheritdoc/>
    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateEntriesImpl(path, searchPattern, searchOption, false, true);

    /// <inheritdoc/>
    public override void CreateDirectory(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        string[]? parts = VfsPathKit.Split(path);
        if (parts == null)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));
        if (parts.Length == 0)
            return; // Root always exists.

        var parent = m_Context.GetRootEntry();
        foreach (string part in parts)
        {
            var existing = m_Context.TryFindChild(parent, part);
            if (existing == null)
            {
                parent = m_Context.AddChild(parent, part, CfbEntryType.Storage);
            }
            else if (existing.Type is CfbEntryType.Storage or CfbEntryType.Root)
            {
                parent = existing;
            }
            else
            {
                throw new IOException(VfsResourceKit.CannotCreateAlreadyExistingEntry(path));
            }
        }
    }

    /// <inheritdoc/>
    public override void DeleteDirectory(string path, bool recursive)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        var parts = new StructuredPath(path).Parts.Span;
        if (parts == null || parts.IsEmpty)
            throw new IOException(VfsResourceKit.AccessToPathIsDenied(path));

        var entry = TryFindEntry(parts);
        if (entry is null || entry.Type is not CfbEntryType.Storage and not CfbEntryType.Root)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        if (!recursive && m_Context.EnumerateChildren(entry).Any())
            throw new IOException(VfsResourceKit.DirectoryIsNotEmpty(path));

        var parent = TryNavigateToStorage(parts[..^1]);
        if (parent is null)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        DeleteEntryRecursive(parent, entry);
    }

    void DeleteEntryRecursive(CfbEntry parent, CfbEntry entry)
    {
        foreach (var child in EnumerateChildrenSnapshot(entry))
            DeleteEntryRecursive(entry, child);

        m_Context.RemoveChild(parent, entry);
    }

    #endregion

    #region Entries

    IEnumerable<string> EnumerateEntriesImpl(
        string path,
        string searchPattern,
        SearchOption searchOption,
        bool enumerateFiles,
        bool enumerateDirectories)
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

    IEnumerable<ReadOnlyMemory<string>> EnumerateEntriesCore(
        in StructuredPath path,
        string searchPattern,
        MatchType matchType,
        MatchCasing matchCasing,
        int maxRecursionDepth,
        bool enumerateFiles,
        bool enumerateDirectories)
    {
        var pathParts = path.Parts.Span;
        if (pathParts == null)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));

        var storage = TryNavigateToStorage(pathParts);
        if (storage is null)
        {
            // Distinguish "path is a file" (IOException) from "path does not exist" (DirectoryNotFoundException).
            var entry = TryFindEntry(pathParts);
            if (entry?.Type == CfbEntryType.Stream)
                throw new IOException(VfsResourceKit.InvalidDirectoryName(path.ToString()));
            else
                throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path.ToString()));
        }

        var searchExpression = new VfsSearchExpression(
            searchPattern,
            DirectorySeparatorChar,
            matchType,
            VfsSearchKit.GetSearchExpressionOptions(this, matchCasing));

        return EnumerateChildrenCore(
            storage,
            pathParts.ToArray(),
            searchExpression,
            maxRecursionDepth,
            0,
            enumerateFiles,
            enumerateDirectories);
    }

    IEnumerable<ReadOnlyMemory<string>> EnumerateChildrenCore(
        CfbEntry storage,
        string[] storageParts,
        VfsSearchExpression searchExpression,
        int maxRecursionDepth,
        int depth,
        bool enumerateFiles,
        bool enumerateDirectories)
    {
        // Snapshot children before iterating: the caller may remove entries from the tree
        // (e.g. during a directory move) and RemoveChild rebuilds the BST, invalidating live pointers.
        foreach (var child in EnumerateChildrenSnapshot(storage))
        {
            if (child.Type == CfbEntryType.Stream)
            {
                if (enumerateFiles && searchExpression.IsMatch(child.Name))
                    yield return (string[])[.. storageParts, child.Name];
            }
            else if (child.Type == CfbEntryType.Storage)
            {
                if (enumerateDirectories && searchExpression.IsMatch(child.Name))
                    yield return (string[])[.. storageParts, child.Name];

                if (depth < maxRecursionDepth)
                {
                    string[] childParts = [.. storageParts, child.Name];
                    foreach (var entry in EnumerateChildrenCore(child, childParts, searchExpression, maxRecursionDepth, depth + 1, enumerateFiles, enumerateDirectories))
                        yield return entry;
                }
            }
        }
    }

    IEnumerable<CfbEntry> EnumerateChildrenSnapshot(CfbEntry parent)
    {
        var query = m_Context.EnumerateChildren(parent);
        if (m_Writable)
        {
            // Snapshot children before iterating: the caller may remove entries from the tree
            // (e.g. during a directory move) rebuilding the BST and invalidating live pointers.
            query = [.. query];
        }
        return query;
    }

    static bool EntryExists(StructuredPath structuredPath, [NotNullWhen(true)] CfbEntry? entry)
    {
        // A trailing directory separator means the caller is treating the path as a directory.
        // Since a file is not a directory, the lookup should behave as if no entry was found there,
        // which is consistent with how the rest of the API treats paths with trailing separators.
        return
            entry is not null &&
            entry.Type is not CfbEntryType.Unallocated &&
            !(entry.Type is CfbEntryType.Stream && structuredPath.IsDirectory);
    }

    CfbEntry? TryNavigateToStorage(ReadOnlySpan<string> parts)
    {
        var entry = m_Context.GetRootEntry();
        foreach (string part in parts)
        {
            var child = m_Context.TryFindChild(entry, part);
            if (child is null || child.Type is CfbEntryType.Stream)
                return null;
            entry = child;
        }
        return entry;
    }

    CfbEntry? TryFindEntry(ReadOnlySpan<string> parts)
    {
        if (parts.IsEmpty)
            return m_Context.GetRootEntry();

        var parent = TryNavigateToStorage(parts[..^1]);
        if (parent is null)
            return null;

        return m_Context.TryFindChild(parent, parts[^1]);
    }

    #endregion

    #region Timestamps

    /// <inheritdoc/>
    public override DateTime GetLastWriteTime(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();

        var structuredPath = new StructuredPath(path);
        var parts = structuredPath.Parts.Span;
        if (parts == null)
            return DateTime.MinValue;

        var entry = TryFindEntry(parts);
        if (EntryExists(structuredPath, entry))
            return entry.ModificationTime;
        else
            return DateTime.MinValue;
    }

    /// <inheritdoc/>
    public override DateTime GetCreationTime(string path)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanRead();

        var structuredPath = new StructuredPath(path);
        var parts = structuredPath.Parts.Span;
        if (parts == null)
            return DateTime.MinValue;

        var entry = TryFindEntry(parts);
        if (EntryExists(structuredPath, entry))
            return entry.CreationTime;
        else
            return DateTime.MinValue;
    }

    /// <inheritdoc/>
    public override void SetCreationTime(string path, DateTime creationTime)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        var entry = FindEntryForSet(path);
        if (entry.CreationTime != creationTime)
        {
            entry.CreationTime = creationTime;
            m_Context.MarkDirty();
        }
    }

    /// <inheritdoc/>
    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        VfsValidationKit.Arguments.ValidatePath(path);

        EnsureCanWrite();

        var entry = FindEntryForSet(path);
        if (entry.ModificationTime != lastWriteTime)
        {
            entry.ModificationTime = lastWriteTime;
            m_Context.MarkDirty();
        }
    }

    CfbEntry FindEntryForSet(string path)
    {
        var structuredPath = new StructuredPath(path);
        var parts = structuredPath.Parts.Span;
        bool isDirectory = structuredPath.IsDirectory;

        if (parts == null)
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        if (parts.IsEmpty)
            return m_Context.GetRootEntry();

        var parent =
            TryNavigateToStorage(parts[..^1]) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        var entry = m_Context.TryFindChild(parent, parts[^1]);
        if (entry is null)
            ThrowFileNotFound(path);

        if (isDirectory && entry.Type == CfbEntryType.Stream)
            throw new IOException(VfsResourceKit.InvalidDirectoryName(path));

        return entry;
    }

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly CfbContext m_Context;

    readonly bool m_Writable;
}
