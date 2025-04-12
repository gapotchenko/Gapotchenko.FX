// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Memory;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a virtual file-system view of the local file system.
/// </summary>
sealed class LocalFileSystemView : FileSystemViewKit
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static LocalFileSystemView Instance { get; } = new();

    #region Capabilities

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool SupportsCreationTime => true;

    public override bool SupportsLastWriteTime => true;

    public override bool SupportsLastAccessTime => true;

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path) => File.Exists(path);

    public override IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFiles(path, searchPattern, searchOption);

    public override Stream ReadFile(string path) => File.OpenRead(path);

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share) => File.Open(path, mode, access, share);

    public override void DeleteFile(string path) => File.Delete(path);

    public override void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        const VfsCopyOptions supportedOptions = VfsCopyOptions.None;
        if ((options & ~supportedOptions) == 0)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }
        else
        {
            base.CopyFile(sourcePath, destinationPath, overwrite, options);
        }
    }

    public override void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        const VfsMoveOptions supportedOptions = VfsMoveOptions.None;
        if ((options & ~supportedOptions) == 0)
        {
#if NETCOREAPP3_0_OR_GREATER
            File.Move(sourcePath, destinationPath, overwrite);
#else
            if (overwrite && File.Exists(destinationPath))
                File.Delete(destinationPath);
            File.Move(sourcePath, destinationPath);
#endif
        }
        else
        {
            base.MoveFile(sourcePath, destinationPath, overwrite, options);
        }
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path) => Directory.Exists(path);

    public override IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateDirectories(path, searchPattern, searchOption);

    public override void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public override void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);

    public override void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        const VfsMoveOptions supportedOptions = VfsMoveOptions.None;
        if ((options & ~supportedOptions) == 0 &&
            !(overwrite && Directory.Exists(destinationPath)) &&
            Directory.Exists(sourcePath))
        {
            const int COR_E_IO = unchecked((int)0x80131620);

            // Trying to use an OS accelerated operation.
            try
            {
                // The call below can also handle files but this would violate
                // the semantic of the method being implemented.
                Directory.Move(sourcePath, destinationPath);
            }
            catch (IOException e) when
                (e.HResult == COR_E_IO &&
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                !PathComparer.Equals(GetPathRoot(sourcePath), GetPathRoot(destinationPath)))
            {
                // Source and destination path must have identical roots.
                // Move will not work across volumes.

                // Downgrade to the base implementation.
                base.MoveDirectory(sourcePath, destinationPath, false, options);
            }

            static string? GetPathRoot(string path) => Path.GetPathRoot(Path.GetFullPath(path));
        }
        else
        {
            // No acceleration is available.
            base.MoveDirectory(sourcePath, destinationPath, overwrite, options);
        }
    }

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path) => FileSystem.EntryExists(path);

    public override IEnumerable<string> EnumerateEntries(string path) => Directory.EnumerateFileSystemEntries(path);

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    public override DateTime GetCreationTime(string path)
    {
        return ConvertUtcTimeToVfs(Directory.GetCreationTimeUtc(path));
    }

    public override void SetCreationTime(string path, DateTime creationTime)
    {
        Directory.SetCreationTime(path, ConvertUtcTimeFromVfs(creationTime));
    }

    public override DateTime GetLastWriteTime(string path)
    {
        return ConvertUtcTimeToVfs(Directory.GetLastWriteTimeUtc(path));
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        Directory.SetLastWriteTimeUtc(path, ConvertUtcTimeFromVfs(lastWriteTime));
    }

    public override DateTime GetLastAccessTime(string path)
    {
        return ConvertUtcTimeToVfs(Directory.GetLastAccessTimeUtc(path));
    }

    public override void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        Directory.SetLastAccessTimeUtc(path, ConvertUtcTimeFromVfs(lastAccessTime));
    }

    static DateTime ConvertUtcTimeToVfs(DateTime time) =>
        time == m_NonExistentEntryTime ? DateTime.MinValue : time;

    static DateTime ConvertUtcTimeFromVfs(DateTime time) => time.ToUniversalTime();

    /// <summary>
    /// <para>
    /// The value returned by the .NET file system APIs to represent a timestamp of a non-existent file-system entry.
    /// </para>
    /// <para>
    /// 12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC)
    /// </para>
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static readonly DateTime m_NonExistentEntryTime = DateTime.FromFileTimeUtc(0);

    #endregion

    #region Paths

    public override char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    public override char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

    public override StringComparer PathComparer => FileSystem.PathComparer;

    public override string CombinePaths(params IEnumerable<string?> paths)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        string?[] arr = EnumerableEx.AsArray(paths);

        if (Array.IndexOf(arr, null) is not -1 and var index)
        {
            int n = arr.Length;
            int startIndex = index + 1;
            int count = index;

            for (int i = startIndex; i < n; ++i)
            {
                if (arr[i] is not null)
                    ++count;
            }

            string[] effectivePaths = new string[count];

            Array.Copy(arr, effectivePaths, index);

            for (int si = startIndex, di = index; si < n; ++si)
            {
                if (arr[si] is not null and var path)
                    effectivePaths[di++] = path;
            }

            return Path.Combine(effectivePaths);
        }
        else
        {
            return Path.Combine(arr!);
        }
    }

    public override string CombinePaths(params ReadOnlySpan<string?> paths)
    {
        if (ReadOnlySpanPolyfills.IndexOf(paths, null) is not -1 and var index)
        {
            int n = paths.Length;
            int startIndex = index + 1;
            int count = index;

            for (int i = startIndex; i < n; ++i)
            {
                if (paths[i] is not null)
                    ++count;
            }

            string[] effectivePaths = new string[count];

            paths[..index].CopyTo(effectivePaths.AsSpan()!);

            for (int si = startIndex, di = index; si < n; ++si)
            {
                if (paths[si] is not null and var path)
                    effectivePaths[di++] = path;
            }

            return Path.Combine(effectivePaths);
        }
        else
        {
#if NET9_0_OR_GREATER
            return Path.Combine(paths!);
#else
            return Path.Combine(paths.ToArray()!);
#endif
        }
    }

    protected override string GetFullPathCore(string path) => Path.GetFullPath(path);

    public override string? GetDirectoryName(string? path)
    {
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
        // .NET Framework and .NET Core versions older than 2.1:
        // argument exception is thrown when the path is empty.
        if (path?.Length == 0)
            return null;
#endif
        return Path.GetDirectoryName(path);
    }

    public override ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return Path.GetDirectoryName(path);
#else
        if (path.IsEmpty)
            return null;
        else
            return Path.GetDirectoryName(path.ToString()).AsSpan();
#endif
    }

    public override string? GetFileName(string? path)
    {
        return Path.GetFileName(path);
    }

    public override ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return Path.GetFileName(path);
#else
        if (path.IsEmpty)
            return path;
        else
            return Path.GetFileName(path.ToString()).AsSpan();
#endif
    }

    public override bool IsPathRooted([NotNullWhen(true)] string? path)
    {
        return Path.IsPathRooted(path);
    }

    public override bool IsPathRooted(ReadOnlySpan<char> path)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return Path.IsPathRooted(path);
#else
        return Path.IsPathRooted(path.ToString());
#endif
    }

    public override string? GetPathRoot(string? path)
    {
#if !(NETCOREAPP || NETSTANDARD2_1_OR_GREATER)
        // .NET Framework only: argument exception is thrown when the path is empty.
        if (path?.Length == 0)
            return null;
#endif
        return Path.GetPathRoot(path);
    }

    public override ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        var result = Path.GetPathRoot(path);
        if (result == null && !path.IsEmpty)
        {
            // Change an empty span signifying null to
            // an empty span signifying an empty string.
            result = string.Empty.AsSpan();
        }
        return result;
#else
        if (path.IsEmpty)
            return null;
        else
            return Path.GetPathRoot(path.ToString()).AsSpan();
#endif
    }

    [return: NotNullIfNotNull(nameof(path))]
    public override string? TrimEndingDirectorySeparator(string? path) => PathEx.TrimEndingDirectorySeparator(path);

    public override ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) => PathEx.TrimEndingDirectorySeparator(path);

    public override bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) => PathEx.EndsInDirectorySeparator(path);

    public override bool EndsInDirectorySeparator(ReadOnlySpan<char> path) => PathEx.EndsInDirectorySeparator(path);

    #endregion
}
