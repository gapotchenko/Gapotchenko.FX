using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
    /// <c>true</c> if the file system is case sensitive; otherwise, <c>false</c>.
    /// </value>
    public static bool IsCaseSensitive { get; } = _IsCaseSensitiveCore();

    static bool _IsCaseSensitiveCore()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))  // HFS+ (the Mac file-system) is usually configured to be case insensitive.
        {
            return false;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return true;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return true;
        }
        else
        {
            // A sane default.
            return false;
        }
    }

    /// <summary>
    /// Determines whether given paths are equivalent, e.g. point to the same file system entry.
    /// </summary>
    /// <param name="a">The file path A.</param>
    /// <param name="b">The file path B.</param>
    /// <returns><c>true</c> when file paths are equivalent; otherwise, <c>false</c>.</returns>
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
    /// <returns><c>true</c> when the beginning of the path matches the specified value in terms of file system equivalence; otherwise, <c>false</c>.</returns>
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

    [return: NotNullIfNotNull("path")]
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
    /// Gets a short version of a specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A short version of the file path.</returns>
    [return: NotNullIfNotNull("filePath")]
    public static string? GetShortPath(string? filePath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;

            int bufferSize = filePath.Length;

            var sb = new StringBuilder(bufferSize);
            int r = NativeMethods.GetShortPathName(filePath, sb, bufferSize);
            if (r == 0 || r > bufferSize)
                return filePath;

            return sb.ToString();
        }
        else
        {
            return filePath;
        }
    }

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
    /// Please use <see cref="WaitForFileAccess(string, FileAccess)"/> method to specify the exact file access mode.
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
    /// Please use <see cref="WaitForFileAccess(string, FileAccess, FileShare)"/> method to specify the exact file access and share modes.
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
                // TODO: Use AppContext.TryGetSwitch to turn that behavior on.
                // Suggested switch name is Switch.Gapotchenko.FX.IO.UseGCForFileAccess

                // Try to close open file streams that weren't properly disposed.
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();

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
    /// Enlists file in the current <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The file path.</param>
    public static void EnlistFileInTransaction(string path) => EnlistFileInTransaction(path, null);

    /// <summary>
    /// Enlists file in a specified <see cref="Transaction"/>.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="transaction">The transaction. If the value is <c>null</c> then the current transaction is used.</param>
    public static void EnlistFileInTransaction(string path, Transaction? transaction)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        if (transaction == null)
        {
            transaction = Transaction.Current;
            if (transaction == null)
                throw new InvalidOperationException("There is no current transaction.");
        }

        FileSystemTransactionManager.EnlistFileInTransaction(path, transaction);
    }

#endif

    /// <summary>
    /// Enumerates the content of a binary file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The sequence of bytes representing the content of a binary file.</returns>
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
}
