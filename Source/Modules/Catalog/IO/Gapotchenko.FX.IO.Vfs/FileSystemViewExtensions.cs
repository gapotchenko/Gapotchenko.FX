// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_STREAM_SPAN
#endif

using Gapotchenko.FX.IO.Vfs.Properties;
using Gapotchenko.FX.IO.Vfs.Utils;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IFileSystemView"/>.
/// </summary>
public static class FileSystemViewExtensions
{
    #region Files

    /// <inheritdoc cref="File.Open(string, FileMode)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode) =>
        OpenFile(view, path, mode, FileAccess.ReadWrite);

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="access"><inheritdoc/></param>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode, FileAccess access)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        return view.OpenFile(path, mode, access, FileShare.None);
    }

    /// <inheritdoc cref="File.OpenText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader OpenTextFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(view.OpenFileForReading(path), Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllBytes(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static byte[] ReadAllBytesFromFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
        {
            throw new ArgumentNullException(nameof(view));
        }
        else if (view is LocalFileSystemView)
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            using var stream = view.OpenFileForReading(path);
            return ReadAllBytesCore(stream);
        }
    }

    static byte[] ReadAllBytesCore(Stream stream)
    {
        long length = stream.CanSeek ? stream.Length : -1;

        if (length == 0)
            return [];
        else if (length == -1)
            return ReadAllBytesUnknownLength(stream);
        else if (length > ArrayHelper.ArrayMaxLength)
            throw new IOException(Resources.FileTooLong2GB);

        int count = (int)length;
        byte[] bytes = new byte[count];
        stream.ReadExactly(bytes, 0, count);
        return bytes;
    }

    static byte[] ReadAllBytesUnknownLength(Stream stream)
    {
        var arrayPool = ArrayPool<byte>.Shared;
        byte[]? rentedArray = null;
        try
        {
#if TFF_STREAM_SPAN
            Span<byte> buffer = stackalloc byte[512];
#else
#if TFF_CER
            try
            {
            }
            finally
#endif
            {
                rentedArray = arrayPool.Rent(512);
            }
            byte[] buffer = rentedArray;
#endif

            for (int bytesRead = 0; ;)
            {
                Debug.Assert(bytesRead < buffer.Length);
                int n =
#if TFF_STREAM_SPAN
                    stream.Read(buffer[bytesRead..]);
#else
                    stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
#endif

                if (n == 0)
                {
                    // The end of stream.
                    return buffer
#if !TFF_STREAM_SPAN
                        .AsSpan()
#endif
                        [..bytesRead]
                        .ToArray();
                }

                bytesRead += n;

                if (bytesRead == buffer.Length)
                {
                    uint newLength = (uint)buffer.Length * 2;
                    if (newLength > ArrayHelper.ArrayMaxLength)
                        newLength = (uint)Math.Max(ArrayHelper.ArrayMaxLength, buffer.Length + 1);

#if TFF_CER
                    try
                    {
                    }
                    finally
#endif
                    {
                        byte[] newRentedArray = arrayPool.Rent((int)newLength);
                        buffer
#if !TFF_STREAM_SPAN
                            .AsSpan()
#endif
                            .CopyTo(newRentedArray);
                        if (rentedArray != null)
                            arrayPool.Return(rentedArray);
                        buffer = rentedArray = newRentedArray;
                    }
                }
            }
        }
        finally
        {
            if (rentedArray != null)
                arrayPool.Return(rentedArray);
        }
    }

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllTextFromFileCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        if (view is LocalFileSystemView)
            return File.ReadAllText(path, encoding);
        else
            return ReadAllTextFromFileCore(view, path, encoding);
    }

    static string ReadAllTextFromFileCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.OpenFileForReading(path), encoding);
        return reader.ReadToEnd();
    }

    #endregion

    #region Paths

    /// <summary>
    /// Concatenates a sequence of paths into a single path.
    /// </summary>
    /// <remarks>
    /// This method simply concatenates the specified <paramref name="paths"/>
    /// and adds a directory separator character between them if one is not already present.
    /// If the length of a specified path component is zero, the method concatenates the remaining parts.
    /// If the length of the resulting concatenated string is zero, the method returns <see cref="string.Empty"/>.
    /// </remarks>
    /// <param name="view">The file system view.</param>
    /// <param name="paths">A sequence of paths.</param>
    /// <returns>The concatenated path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    public static string JoinPaths(
        this IReadOnlyFileSystemView view,
        params IEnumerable<string?> paths)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        char directorySeparatorChar = view.DirectorySeparatorChar;
        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length != 0)
            {
                if (!IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                    !IsDirectorySeparator(path[0], directorySeparatorChar))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    static bool IsDirectorySeparator(char c, char directorySeparatorChar) =>
        c == directorySeparatorChar ||
        c is '/' or '\\';

    #endregion
}
