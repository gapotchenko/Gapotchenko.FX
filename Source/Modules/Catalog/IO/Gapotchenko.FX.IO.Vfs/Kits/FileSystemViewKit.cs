// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO.Vfs.Utils;
using Gapotchenko.FX.Threading.Tasks;
using System.Security;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IFileSystemView"/> interface.
/// </summary>
/// <remarks>
/// This class provides a solid foundation for a virtual file system (VFS) implementation.
/// While it is not obligatory to use it, doing so may significantly simplify a particular VFS implementation.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class FileSystemViewKit : IFileSystemView
{
    #region Capabilities

    /// <inheritdoc/>
    public abstract bool CanRead { get; }

    /// <inheritdoc/>
    public abstract bool CanWrite { get; }

    /// <inheritdoc/>
    public virtual bool SupportsCreationTime => false;

    /// <inheritdoc/>
    public virtual bool SupportsLastWriteTime => false;

    /// <inheritdoc/>
    public virtual bool SupportsLastAccessTime => false;

    /// <inheritdoc/>
    public virtual bool SupportsAttributes => false;

    /// <summary>
    /// Ensures that the file system supports reading.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support reading.</exception>
    protected void EnsureCanRead()
    {
        if (!CanRead)
            ThrowHelper.CannotReadFS();
    }

    /// <summary>
    /// Ensures that the file system supports writing.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    protected void EnsureCanWrite()
    {
        if (!CanWrite)
            ThrowHelper.CannotWriteFS();
    }

    /// <summary>
    /// Ensures that the file system can open a file with the specified mode and access.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support reading.</exception>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    protected void EnsureCanOpenFile(FileMode mode, FileAccess access) =>
        VfsValidationKit.Capabilities.EnsureCanOpenFile(mode, access, CanRead, CanWrite);

    #endregion

    #region Files

    /// <inheritdoc/>
    public abstract bool FileExists([NotNullWhen(true)] string? path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path) => EnumerateFiles(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public abstract IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateEnumerationOptions(enumerationOptions);

        return EnumerateEntriesCore(path, searchPattern, enumerationOptions, true, false);
    }

    /// <inheritdoc/>
    public virtual Stream ReadFile(string path) =>
        OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);

    /// <inheritdoc/>
    public virtual Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken) =>
        OpenFileAsync(path, FileMode.Open, FileAccess.Read, FileShare.Read, cancellationToken);

    /// <inheritdoc/>
    public abstract Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc/>
    public virtual Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, CancellationToken cancellationToken) =>
        TaskBridge.ExecuteAsync(() => OpenFile(path, mode, access, share), cancellationToken);

    /// <inheritdoc/>
    public abstract void DeleteFile(string path);

    /// <inheritdoc/>
    public virtual void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyFileNaive(
            new(this, sourcePath),
            new(this, destinationPath),
            overwrite,
            options);
    }

    /// <inheritdoc/>
    public virtual void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveFileNaive(new(this, sourcePath), new(this, destinationPath), overwrite, options);
    }

    #endregion

    #region Directories

    /// <inheritdoc/>
    public abstract bool DirectoryExists([NotNullWhen(true)] string? path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path) => EnumerateDirectories(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public abstract IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        VfsValidationKit.Arguments.ValidatePath(path);
        VfsValidationKit.Arguments.ValidateSearchPattern(searchPattern);
        VfsValidationKit.Arguments.ValidateEnumerationOptions(enumerationOptions);

        return EnumerateEntriesCore(path, searchPattern, enumerationOptions, false, true);
    }

    /// <inheritdoc/>
    public abstract void CreateDirectory(string path);

    /// <inheritdoc/>
    public abstract void DeleteDirectory(string path, bool recursive);

    /// <inheritdoc/>
    public virtual void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyDirectoryNaive(new(this, sourcePath), new(this, destinationPath), overwrite, options);
    }

    /// <inheritdoc/>
    public virtual void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveDirectoryNaive(new(this, sourcePath), new(this, destinationPath), overwrite, options);
    }

    #endregion

    #region Entries

    /// <inheritdoc/>
    public virtual bool EntryExists([NotNullWhen(true)] string? path) =>
        FileExists(path) ||
        DirectoryExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path) =>
        EnumerateEntries(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern) =>
        EnumerateEntries(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateFiles(path, searchPattern, searchOption)
        .Concat(EnumerateDirectories(path, searchPattern, searchOption));

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateFiles(path, searchPattern, enumerationOptions)
        .Concat(EnumerateDirectories(path, searchPattern, enumerationOptions));

    IEnumerable<string> EnumerateEntriesCore(
        string path,
        string searchPattern,
        EnumerationOptions options,
        bool enumerateFiles,
        bool enumerateDirectories)
    {
        EnsureCanRead();

        // ----------------------------------------------------------------------
        // Parameters calculation
        // ----------------------------------------------------------------------

        VfsSearchKit.AdjustPatternPath(this, ref path, ref searchPattern);

        int maxRecursionDepth = VfsSearchKit.GetMaxRecursionDepth(options);

        var attributesToSkip = options.AttributesToSkip;
        if (attributesToSkip != 0 && !SupportsAttributes)
            attributesToSkip = 0;

        // ----------------------------------------------------------------------
        // Short circuit handling
        // ----------------------------------------------------------------------

        if (options.MatchType == MatchType.Win32 &&
            VfsSearchKit.IsDefaultMatchCasing(this, options.MatchCasing))
        {
            if (maxRecursionDepth is 0 or int.MaxValue &&
                attributesToSkip == 0 &&
                !options.ReturnSpecialDirectories &&
                !options.IgnoreInaccessible)
            {
                var searchOption = maxRecursionDepth is 0 ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                return EnumerateEntriesMatrix(path, searchPattern, searchOption, enumerateFiles, enumerateDirectories);
            }
        }

        // ----------------------------------------------------------------------
        // Polyfill implementation that uses more basic but available primitives
        // ----------------------------------------------------------------------

        return EnumerateEntriesPolyfillImpl(path, maxRecursionDepth);

        IEnumerable<string> EnumerateEntriesPolyfillImpl(string path, int remainingRecursionDepth)
        {
            var searchExpression = new VfsSearchExpression(
                searchPattern,
                DirectorySeparatorChar,
                options.MatchType,
                VfsSearchKit.GetSearchExpressionOptions(this, options.MatchCasing));

            var pendingHierarchies = new Queue<(string Path, int RemainingRecursionDepth)>();

            for (; ; )
            {
                foreach (string i in EnumerateHierarchy(path, remainingRecursionDepth))
                    yield return i;

                if (pendingHierarchies.TryDequeue(out var next))
                {
                    // Next hierarchy.
                    path = next.Path;
                    remainingRecursionDepth = next.RemainingRecursionDepth;
                }
                else
                {
                    // No hierarchies left.
                    break;
                }
            }

            IEnumerable<string> EnumerateHierarchy(string path, int remainingRecursionDepth)
            {
                IEnumerable<string> query;

                try
                {
                    query = EnumerateEntriesMatrix(
                        path,
                        "*",
                        SearchOption.TopDirectoryOnly,
                        enumerateFiles,
                        enumerateDirectories || remainingRecursionDepth > 0);
                }
                catch (Exception e) when (options.IgnoreInaccessible && e is UnauthorizedAccessException or SecurityException)
                {
                    // Ignore an access error.
                    yield break;
                }

                if (enumerateDirectories && options.ReturnSpecialDirectories)
                {
                    // Emulate special directories.

                    // The current directory ".".
                    yield return this.JoinPaths(path, ".");

                    // The parent directory "..".
                    if (!GetDirectoryName(path.AsSpan()).IsEmpty)
                    {
                        string directoryPath = this.JoinPaths(path, "..");
                        if (!options.IgnoreInaccessible || DirectoryExists(directoryPath))
                            yield return directoryPath;
                    }
                }

                foreach (string entryPath in query)
                {
                    FileAttributes? entryAttributes = null;

                    if (attributesToSkip != 0)
                    {
                        var attributes = GetAttributes(entryPath);
                        if ((attributes & attributesToSkip) != 0)
                            continue;
                        entryAttributes = attributes;
                    }

                    bool fileExists = false;
                    if (enumerateFiles &&
                        ((entryAttributes.HasValue && (entryAttributes.Value & FileAttributes.Directory) == 0) ||
                        (fileExists = FileExists(entryPath))))
                    {
                        if (searchExpression.IsMatch(GetFileName(entryPath.AsSpan())))
                        {
                            if (fileExists || !options.IgnoreInaccessible || FileExists(entryPath))
                                yield return entryPath;
                        }
                        continue;
                    }

                    if (options.IgnoreInaccessible && !DirectoryExists(entryPath))
                        continue;

                    if (enumerateDirectories)
                    {
                        if (searchExpression.IsMatch(GetFileName(entryPath.AsSpan())))
                            yield return entryPath;
                    }

                    if (remainingRecursionDepth > 0)
                        pendingHierarchies.Enqueue((entryPath, remainingRecursionDepth - 1));
                }
            }
        }

        // ----------------------------------------------------------------------
        // Helper functions
        // ----------------------------------------------------------------------

        IEnumerable<string> EnumerateEntriesMatrix(
           string path,
           string searchPattern,
           SearchOption searchOption,
           bool enumerateFiles,
           bool enumerateDirectories)
        {
            return
                (enumerateFiles, enumerateDirectories) switch
                {
                    (true, false) => EnumerateFiles(path, searchPattern, searchOption),
                    (false, true) => EnumerateDirectories(path, searchPattern, searchOption),
                    (true, true) => EnumerateEntries(path, searchPattern, searchOption),
                    (false, false) => []
                };
        }
    }

    /// <inheritdoc/>
    public virtual DateTime GetCreationTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetCreationTime(string path, DateTime creationTime) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual DateTime GetLastWriteTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetLastWriteTime(string path, DateTime lastWriteTime) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual DateTime GetLastAccessTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetLastAccessTime(string path, DateTime lastAccessTime) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual FileAttributes GetAttributes(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetAttributes(string path, FileAttributes attributes) => throw new NotSupportedException();

    #endregion

    #region Paths

    /// <inheritdoc/>
    public virtual char DirectorySeparatorChar => VfsPathKit.DirectorySeparatorChar;

    /// <inheritdoc/>
    public virtual char AltDirectorySeparatorChar => VfsPathKit.AltDirectorySeparatorChar;

    /// <inheritdoc/>
    public abstract StringComparer PathComparer { get; }

    /// <inheritdoc/>
    public abstract StringComparison PathComparison { get; }

    /// <inheritdoc/>
    public virtual string CombinePaths(params IEnumerable<string?> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        char directorySeparatorChar = DirectorySeparatorChar;

        var builder = new StringBuilder();
        foreach (string? path in paths)
            AppendCombinedPath(builder, path, directorySeparatorChar);
        return builder.ToString();
    }

    /// <inheritdoc/>
    public virtual string CombinePaths(params ReadOnlySpan<string?> paths)
    {
        char directorySeparatorChar = DirectorySeparatorChar;

        var builder = new StringBuilder();
        foreach (string? path in paths)
            AppendCombinedPath(builder, path, directorySeparatorChar);
        return builder.ToString();
    }

    void AppendCombinedPath(StringBuilder builder, string? path, char directorySeparatorChar)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (builder.Length != 0)
        {
            if (IsPathRooted(path.AsSpan()))
            {
                builder.Clear();
            }
            else if (!VfsPathKit.IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                !VfsPathKit.IsDirectorySeparator(path[0], directorySeparatorChar))
            {
                builder.Append(directorySeparatorChar);
            }
        }

        builder.Append(path);
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path)
    {
        if (path is null)
        {
            return null;
        }
        else if (path.Length == 0)
        {
            throw new ArgumentException(VfsResourceKit.PathIsEmpty, nameof(path));
        }
        else
        {
            string fullPath = GetFullPathCore(path);

            if (fullPath.Length != 0)
            {
                char directorySeparatorChar = DirectorySeparatorChar;
                if (fullPath[^1] != directorySeparatorChar &&
                    VfsPathKit.IsDirectorySeparator(path[^1], directorySeparatorChar))
                {
                    fullPath += directorySeparatorChar;
                }
            }

            return fullPath;
        }
    }

    /// <inheritdoc cref="GetFullPath(string?)"/>
    protected virtual string GetFullPathCore(string path)
    {
        char directorySeparatorChar = DirectorySeparatorChar;

        string[] parts =
            VfsPathKit.Split(path, directorySeparatorChar) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        return directorySeparatorChar + VfsPathKit.Join(parts, directorySeparatorChar);
    }

    /// <inheritdoc/>
    public virtual string? GetDirectoryName(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else if (GetDirectoryName(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path) =>
        VfsPathKit.GetDirectoryName(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? GetFileName(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var result = GetFileName(path.AsSpan());
        if (path.Length == result.Length)
            return path;

        return result.ToString();
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) =>
        VfsPathKit.GetFileName(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    public virtual bool IsPathRooted([NotNullWhen(true)] string? path) => IsPathRooted(path.AsSpan());

    /// <inheritdoc/>
    public virtual bool IsPathRooted(ReadOnlySpan<char> path) => !GetPathRoot(path).IsEmpty;

    /// <inheritdoc/>
    public virtual string? GetPathRoot(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else if (GetPathRoot(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path) =>
        VfsPathKit.GetPathRoot(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? TrimEndingDirectorySeparator(string? path) =>
        path != null && EndsInDirectorySeparator(path) && !IsRootPath(path.AsSpan()) ?
            path[..^1] :
            path;

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
        EndsInDirectorySeparator(path) && !IsRootPath(path) ?
            path[..^1] :
            path;

    bool IsRootPath(ReadOnlySpan<char> path) => path.Length == GetPathRoot(path).Length;

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) =>
        EndsInDirectorySeparator(path.AsSpan());

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator(ReadOnlySpan<char> path) =>
        path.Length > 0 &&
        VfsPathKit.IsDirectorySeparator(path[^1], DirectorySeparatorChar);

    #endregion
}
