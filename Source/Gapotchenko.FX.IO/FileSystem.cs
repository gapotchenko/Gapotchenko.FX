﻿using Gapotchenko.FX.IO.Pal;
using Gapotchenko.FX.IO.Properties;
using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
#if TFF_TRANSACTIONS
using System.Transactions;
#endif

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides file system functionality for the host operating system.
/// </summary>
public static class FileSystem
{
    /// <summary>
    /// Gets file path string comparison.
    /// </summary>
    /// <value>File path string comparison.</value>
    public static StringComparison PathComparison =>
        IsCaseSensitive ?
            StringComparison.InvariantCulture :
            StringComparison.InvariantCultureIgnoreCase;

    /// <summary>
    /// Gets file path string comparer.
    /// </summary>
    /// <remarks>
    /// The returned comparer instance does not apply path normalization or equivalence checks.
    /// </remarks>
    /// <value>File path string comparer.</value>
    public static StringComparer PathComparer =>
        IsCaseSensitive ?
            StringComparer.InvariantCulture :
            StringComparer.InvariantCultureIgnoreCase;

    /// <summary>
    /// Gets file path equivalence comparer.
    /// </summary>
    /// <remarks>
    /// The returned comparer instance applies path normalization and equivalence checks.
    /// </remarks>
    /// <value>File path equivalence comparer.</value>
    public static StringComparer PathEquivalenceComparer => IO.PathEquivalenceComparer.Instance;

    /// <summary>
    /// Gets a value indicating whether the file system under current operating system is case sensitive.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the file system is case sensitive; otherwise, <see langword="false"/>.
    /// </value>
    public static bool IsCaseSensitive { get; } = IsCaseSensitiveCore();

    static bool IsCaseSensitiveCore()
    {
        var pal = PalServices.AdapterOrDefault;
        if (pal != null)
            return pal.IsCaseSensitive;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
            return false;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return true;
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
            return true;
        else
            return false; // a sane default
    }

    /// <summary>
    /// Determines whether given paths are equivalent, e.g. point to the same file system entry.
    /// </summary>
    /// <param name="a">The file path A.</param>
    /// <param name="b">The file path B.</param>
    /// <returns><see langword="true"/> when file paths are equivalent; otherwise, <see langword="false"/>.</returns>
    public static bool PathsAreEquivalent(string? a, string? b)
    {
        var pathComparison = PathComparison;

        if (string.Equals(a, b, pathComparison))
            return true;
        if (a == null || b == null)
            return false;

        a = NormalizePath(a, false);
        b = NormalizePath(b, false);

        if (a.Equals(b, pathComparison))
        {
            // Fast shortcut.
            return true;
        }

        a = Path.GetFullPath(a);
        b = Path.GetFullPath(b);

        return a.Equals(b, pathComparison);
    }

    /// <summary>
    /// Determines whether the beginning of the path matches the specified value in terms of file system equivalence.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="value">The value.</param>
    /// <returns><see langword="true"/> when the beginning of the path matches the specified value in terms of file system equivalence; otherwise, <see langword="false"/>.</returns>
    public static bool PathStartsWith(string? path, string? value)
    {
        var pathComparison = PathComparison;

        if (string.Equals(path, value, pathComparison))
            return true;
        if (path == null || value == null)
            return false;

        path = NormalizePath(path, true);
        value = NormalizePath(value, true);

        path = Path.GetFullPath(path);
        value = Path.GetFullPath(value);

        return path.StartsWith(value, pathComparison);
    }

    [return: NotNullIfNotNull(nameof(path))]
    internal static string? NormalizePath(string? path, bool? trailingSlash = null)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        static string RemoveAdjacentChars(string value, char c, int startIndex)
        {
            int n = value.Length;

            ++startIndex;
            if (startIndex >= n)
                return value;

            var sb = new StringBuilder(value, 0, startIndex, n);

            char prevChar = value[startIndex - 1];
            for (int i = startIndex; i != n; ++i)
            {
                char ch = value[i];
                if (ch == c && ch == prevChar)
                    continue;
                prevChar = ch;
                sb.Append(ch);
            }

            return sb.ToString();
        }

        path = RemoveAdjacentChars(path, Path.DirectorySeparatorChar, 1);

        if (trailingSlash.HasValue)
        {
            if (trailingSlash.Value)
            {
                if (!path.EndsWith(Path.DirectorySeparatorChar))
                    path += Path.DirectorySeparatorChar;
            }
            else
            {
                if (path.EndsWith(Path.DirectorySeparatorChar))
                    path = path.Substring(0, path.Length - 1);
            }
        }

        return path;
    }

    /// <summary>
    /// <para>
    /// Canonicalizes a specified path.
    /// </para>
    /// <para>
    /// The alternative directory separators are replaced with native ones;
    /// the duplicate adjacent separators are removed.
    /// </para>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The canonicalized path.</returns>
    [return: NotNullIfNotNull("path")]
    public static string? CanonicalizePath(string? path) => NormalizePath(path);

    /// <summary>
    /// Gets a short version of the specified file system entry path.
    /// </summary>
    /// <param name="path">The path of a file system entry.</param>
    /// <returns>
    /// A short version of the specified file system entry path,
    /// or unmodified path if its short version is unavailable.
    /// </returns>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetShortPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var pal = PalServices.AdapterOrDefault;
        if (pal != null)
            return pal.GetShortPath(path);
        else
            return path;
    }

    /// <summary>
    /// Enumerates subpaths of a path.
    /// </summary>
    /// <remarks>
    /// For example, the subpaths of <c>C:\Users\Tester\Documents</c> path are:
    /// <list type="bullet">
    /// <item><c>C:\Users\Tester\Documents</c></item>
    /// <item><c>C:\Users\Tester</c></item>
    /// <item><c>C:\Users</c></item>
    /// <item><c>C:\</c></item>
    /// </list>
    /// </remarks>
    /// <param name="path">The path.</param>
    /// <returns>The sequence of subpaths.</returns>
    public static IEnumerable<string> EnumerateSubpaths(string? path)
    {
        for (var i = path; !string.IsNullOrEmpty(i); i = Path.GetDirectoryName(i))
            yield return i
#if !NET
                !
#endif
                ;
    }

    /// <summary>
    /// Splits a specified path into a sequence of file system entry names.
    /// </summary>
    /// <remarks>
    /// For example, the entry names of <c>C:\Users\Tester\Documents</c> path are:
    /// <list type="bullet">
    /// <item><c>C:\</c></item>
    /// <item><c>Users</c></item>
    /// <item><c>Tester</c></item>
    /// <item><c>Documents</c></item>
    /// </list>
    /// </remarks>
    /// <param name="path">The path.</param>
    /// <returns>The sequence of file system entry names.</returns>
    public static IEnumerable<string> SplitPath(string? path) =>
        EnumerateSubpaths(path)
        .Reverse()
        .Select(subpath => Path.GetFileName(subpath));

    /// <summary>
    /// Inserts a subpath into the path at the specified index.
    /// </summary>
    /// <param name="path">The path to insert the subpath to.</param>
    /// <param name="index">The index to insert the subpath at.</param>
    /// <param name="subpath">The subpath to insert.</param>
    /// <returns>The path with the inserted subpath.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="subpath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The specified index is out of range.</exception>
    public static string InsertSubpath(string path, Index index, string subpath)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (subpath == null)
            throw new ArgumentNullException(nameof(subpath));

        switch ((index.IsFromEnd, index.Value))
        {
            case (true, 0):
                return Path.Combine(path, subpath);

            case (true, 1):
                {
                    var directory =
                        Path.GetDirectoryName(path) ??
                        throw new ArgumentOutOfRangeException(nameof(index));
                    directory = Path.Combine(directory, subpath);
                    return Path.Combine(directory, Path.GetFileName(path));
                }

            case (false, 0):
                return Path.Combine(subpath, path);

            default:
                {
                    var parts = new List<string>();
                    for (string? i = path; !string.IsNullOrEmpty(i); i = Path.GetDirectoryName(i))
                        parts.Add(Path.GetFileName(i));

                    int n = parts.Count;
                    int offset = index.GetOffset(n);
                    if (offset < 0 || offset > n)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    parts.Reverse();
                    parts.Insert(offset, subpath);
                    return Path.Combine(parts.ToArray());
                }
        }
    }

    /// <summary>
    /// Determines whether the given path refers to an existing file or directory on disk.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>
    /// <see langword="true"/> if path refers to an existing file or directory;
    /// <see langword="false"/> if neither directory nor file exists or an error occurs when trying to determine if the specified file system entry exists.
    /// </returns>
    public static bool EntryExists([NotNullWhen(true)] string? path) =>
        File.Exists(path) || Directory.Exists(path);

    /// <summary>
    /// Waits for a read access to the file.
    /// </summary>
    /// <param name="path">The file path.</param>
    public static void WaitForFileReadAccess(string path) => WaitForFileAccess(path, FileAccess.Read, FileShare.Read);

    /// <summary>
    /// Waits for a write access to the file.
    /// Anti-virus tools, search engines, file synchronization applications can lock the files for short random time spans.
    /// Use this method to provide reliable file write operations in a noisy operating system environment.
    /// </summary>
    /// <remarks>
    /// Please note that the actual access check is read-write despite the name of the method.
    /// This is done to cover a common sense perception of a file write operation,
    /// e.g. a write means not only file creation/overwrite but also an edit.
    /// Please use <see cref="WaitForFileAccess(string, FileAccess)"/> method if you want to specify the exact file access mode.
    /// </remarks>
    /// <param name="path">The file path.</param>
    public static void WaitForFileWriteAccess(string path) => WaitForFileAccess(path, FileAccess.ReadWrite, FileShare.None);

    /// <summary>
    /// Waits for a specified access to the file.
    /// </summary>
    /// <remarks>
    /// File share mode is automatically deducted from the given file access mode.
    /// If <see cref="FileAccess.Write"/> access flag is set then <see cref="FileShare.None"/> share mode is used.
    /// Otherwise, the <see cref="FileShare.Read"/> share mode is used.
    /// Please use <see cref="WaitForFileAccess(string, FileAccess, FileShare)"/> method if you want to specify the exact file access and share modes.
    /// </remarks>
    /// <param name="path">The file path.</param>
    /// <param name="access">The file access.</param>
    public static void WaitForFileAccess(string path, FileAccess access)
    {
        FileShare fileShare;
        if ((access & FileAccess.Write) != 0)
            fileShare = FileShare.None;
        else
            fileShare = FileShare.Read;

        WaitForFileAccess(path, access, fileShare);
    }

    /// <summary>
    /// Waits for a specified access to the file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="access">The file access.</param>
    /// <param name="share">A constant that determines how the file can be shared among concurrent accessors.</param>
    public static void WaitForFileAccess(string path, FileAccess access, FileShare share)
    {
        if (!File.Exists(path))
            return;

        Stopwatch? sw = null;
        for (; ; )
        {
            bool accessAllowed = false;

            try
            {
                using (new FileStream(path, FileMode.Open, access, share))
                {
                }
                accessAllowed = true;
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (FileNotFoundException)
            {
                // Don't waste time if the file does not exist anymore.
                break;
            }
            catch (IOException)
            {
            }

            if (accessAllowed)
                break;

            if (sw == null)
            {
                if (AppContext.TryGetSwitch("Switch.Gapotchenko.FX.IO.UseGCForFileAccess", out var gcEnabled) &&
                    gcEnabled)
                {
                    // Try to close open file streams that weren't properly disposed.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                sw = Stopwatch.StartNew();
            }
            else
            {
                if (sw.ElapsedMilliseconds > 10000)
                    break;
            }

            Thread.Sleep(200);
        }
    }

#if TFF_TRANSACTIONS

    /// <summary>
    /// Enlists file in a specified <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="transaction">
    /// The transaction.
    /// If the value is <see langword="null"/> then the current transaction is used.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">There is no current transaction.</exception>
    public static void EnlistFileInTransaction(string path, Transaction? transaction) =>
        FileSystemTransactionManager.EnlistFileInTransaction(
            path ?? throw new ArgumentNullException(nameof(path)),
            transaction ?? GetCurrentTransaction());

    /// <summary>
    /// Enlists file in the current <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">There is no current transaction.</exception>
    public static void EnlistFileInTransaction(string path) => EnlistFileInTransaction(path, null);

    /// <summary>
    /// Enlists directory in a specified <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="transaction">
    /// The transaction.
    /// If the value is <see langword="null"/> then the current transaction is used.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">There is no current transaction.</exception>
    public static void EnlistDirectoryInTransaction(string path, Transaction? transaction) =>
        FileSystemTransactionManager.EnlistDirectoryInTransaction(
            path ?? throw new ArgumentNullException(nameof(path)),
            transaction ?? GetCurrentTransaction());

    /// <summary>
    /// Enlists directory in the current <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">There is no current transaction.</exception>
    public static void EnlistDirectoryInTransaction(string path) => EnlistDirectoryInTransaction(path, null);

    static Transaction GetCurrentTransaction() =>
        Transaction.Current ??
        throw new InvalidOperationException("There is no current transaction.");

#endif

    /// <summary>
    /// Enumerates content of the specified binary file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>A sequence of bytes representing the binary file content.</returns>
    public static IEnumerable<byte> EnumerateFileBytes(string path)
    {
        using var stream = File.OpenRead(path);
        foreach (var i in stream.AsEnumerable())
            yield return i;
    }

    /// <summary>
    /// Gets the size of a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The size of a file.</returns>
    public static long GetFileSize(string path)
    {
        // TODO: Use native API to eliminate object allocation.
        // GetFileAttributesEx is the fastest candidate on Windows.
        return new FileInfo(path).Length;
    }

    /// <summary>
    /// Gets a canonicalized absolute path of the specified file system entry.
    /// </summary>
    /// <remarks>
    /// This method expands all symbolic links and resolves references to
    /// <c>.</c> and <c>..</c> special directories.
    /// It also normalizes consequent directory separators to the canonical form.
    /// </remarks>
    /// <param name="path">The path of a file system entry.</param>
    /// <returns>A canonicalized absolute path of the specified file system entry.</returns>
    /// <exception cref="IOException">Could not find the specified file or directory. File system entry does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="Win32Exception">An operating system error.</exception>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetRealPath(string? path)
    {
        if (path is null)
            return null;

        var pal = PalServices.AdapterOrDefault;
        if (pal != null)
        {
            return pal.GetRealPath(path);
        }
        else
        {
            if (!EntryExists(path))
                throw new IOException(string.Format(Resources.FileSystemEntryXDoesNotExsit, path));

            return Path.GetFullPath(PathEx.TrimEndingDirectorySeparator(path));
        }
    }
}
