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

partial class FileSystemViewExtensions
{
    #region Open

    /// <inheritdoc cref="File.OpenWrite(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static Stream OpenFileWrite(this IFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.OpenWrite(path);
        else
            return view.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }

    /// <inheritdoc cref="File.OpenText(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader OpenTextFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(view.OpenFileRead(path), Encoding.UTF8);
    }

    /// <inheritdoc cref="File.Open(string, FileMode)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode) =>
        OpenFile(view, path, mode, FileAccess.ReadWrite);

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="access"><inheritdoc/></param>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode, FileAccess access)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        return view.OpenFile(path, mode, access, FileShare.None);
    }

    #endregion

    #region Create

    /// <inheritdoc cref="File.Create(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static Stream CreateFile(this IFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.Create(path);
        else
            return view.OpenFile(path, FileMode.Create);
    }

    /// <inheritdoc cref="File.CreateText(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamWriter CreateTextFile(this IFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.CreateText(path);
        else
            return new StreamWriter(CreateFileForWritingCore(view, path));
    }

    static StreamWriter CreateTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        return new StreamWriter(
            CreateFileForWritingCore(view, path),
            encoding);
    }

    static Stream CreateFileForWritingCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read);

    #endregion

    #region Append

    /// <inheritdoc cref="File.AppendText(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamWriter AppendTextFile(this IFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.AppendText(path);
        else
            return new StreamWriter(AppendFileCore(view, path));
    }

    static StreamWriter AppendTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        return new StreamWriter(
            AppendFileCore(view, path),
            encoding);
    }

    static Stream AppendFileCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Append, FileAccess.Write, FileShare.Read);

    #endregion

    #region Read/write bytes

    /// <inheritdoc cref="File.ReadAllBytes(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static byte[] ReadAllFileBytes(this IReadOnlyFileSystemView view, string path)
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
            using var stream = view.OpenFileRead(path);
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

    /// <inheritdoc cref="File.WriteAllBytes(string, byte[])"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="bytes"><inheritdoc/></param>
    public static void WriteAllFileBytes(this IFileSystemView view, string path, byte[] bytes)
    {
        if (view is null)
        {
            throw new ArgumentNullException(nameof(view));
        }
        else if (view is LocalFileSystemView)
        {
            File.WriteAllBytes(path, bytes);
        }
        else
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            using var stream = CreateFileForWritingCore(view, path);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    #endregion

    #region Read/write/append text

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllFileTextCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadAllText(path, encoding);
        else
            return ReadAllFileTextCore(view, path, encoding);
    }

    static string ReadAllFileTextCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.OpenFileRead(path), encoding);
        return reader.ReadToEnd();
    }

    /// <inheritdoc cref="File.WriteAllText(string, string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.WriteAllText(path, contents);
        }
        else
        {
            using var writer = view.CreateTextFile(path);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="File.WriteAllText(string, string, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.WriteAllText(path, contents, encoding);
        }
        else
        {
            using var writer = view.CreateTextFile(path, encoding);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="File.AppendAllText(string, string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.AppendAllText(path, contents);
        }
        else
        {
            using var writer = view.AppendTextFile(path);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.AppendAllText(path, contents, encoding);
        }
        else
        {
            using var writer = view.AppendTextFile(path, encoding);
            writer.Write(contents);
        }
    }

    #endregion

    #region Read/write/append lines

    /// <inheritdoc cref="File.ReadLines(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadLines(path);
        else
            return ReadFileLinesCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadLines(string, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path, encoding);
        else
            return ReadFileLinesCore(view, path, encoding ?? throw new ArgumentNullException(nameof(encoding)));
    }

    static IEnumerable<string> ReadFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.OpenFileRead(path), encoding);

        while (reader.ReadLine() is not null and var line)
            yield return line;
    }

    /// <inheritdoc cref="File.ReadAllLines(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path);
        else
            return ReadAllFileLinesCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllLines(string, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path, encoding);
        else
            return ReadAllFileLinesCore(view, path, encoding ?? throw new ArgumentNullException(nameof(encoding)));
    }

    static string[] ReadAllFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.OpenFileRead(path), encoding);

        var lines = new List<string>();
        while (reader.ReadLine() is not null and var line)
            lines.Add(line);

        return lines.ToArray();
    }

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!);
        }
        else
        {
            if (contents is null)
                throw new ArgumentNullException(nameof(contents));

            using var writer = view.CreateTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string}, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!, encoding);
        }
        else
        {
            if (contents is null)
                throw new ArgumentNullException(nameof(contents));

            using var writer = view.CreateTextFile(path, encoding);
            WriteAllLinesCore(writer, contents);
        }
    }

    static void WriteAllLinesCore(StreamWriter writer, IEnumerable<string?> contents)
    {
        foreach (string? line in contents)
            writer.WriteLine(line);
    }

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string})"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!);
        }
        else
        {
            if (contents is null)
                throw new ArgumentNullException(nameof(contents));

            using var writer = view.AppendTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string}, Encoding)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!, encoding);
        }
        else
        {
            if (contents is null)
                throw new ArgumentNullException(nameof(contents));

            using var writer = view.AppendTextFile(path, encoding);
            WriteAllLinesCore(writer, contents);
        }
    }

    #endregion

    #region Copy

    /// <summary>
    /// Copies an existing file to a new file.
    /// Overwriting a file of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void CopyFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .CopyFile(sourcePath, destinationPath, false);

    /// <summary>
    /// Copies an existing file to a new file in the specified destination <see cref="IFileSystemView"/>.
    /// Overwriting a file of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyFile(IReadOnlyFileSystemView, string, IFileSystemView, string, bool)"/>
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath) =>
        CopyFile(sourceView, sourcePath, destinationView, destinationPath, false);

    /// <summary>
    /// Copies an existing file to a new file in the specified destination <see cref="IFileSystemView"/>.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the file to copy.</param>
    /// <param name="sourcePath">The path of the file to copy.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to copy the file to.</param>
    /// <param name="destinationPath">
    /// The path of the destination file in the specified <see cref="IFileSystemView"/>.
    /// This cannot be a directory.
    /// </param>
    /// <param name="overwrite">
    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool)" path="//param[@name='overwrite']"/>
    /// </param>
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));

        IOHelper.CopyFileOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite);
    }

    #endregion

    #region Move

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveFile(string, string, bool)"/>
    /// <param name="view">The file-system view to move the file at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void MoveFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .MoveFile(sourcePath, destinationPath, false);

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="MoveFile(IFileSystemView, string, IFileSystemView, string, bool)"/>
    public static void MoveFile(
        this IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath) =>
        MoveFile(sourceView, sourcePath, destinationView, destinationPath, false);

    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the file to move.</param>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destinationView">The destination <see cref="IFileSystemView"/> to move the file to.</param>
    /// <param name="destinationPath">
    /// The path of the destination file in the specified <see cref="IFileSystemView"/>.
    /// This cannot be a directory.
    /// </param>
    /// <inheritdoc cref="IFileSystemView.MoveFile(string, string, bool)"/>
    /// <param name="overwrite"><inheritdoc/></param>
    public static void MoveFile(
        this IFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite)
    {
        if (sourceView is null)
            throw new ArgumentNullException(nameof(sourceView));
        if (destinationView is null)
            throw new ArgumentNullException(nameof(destinationView));

        IOHelper.MoveFileOptimized(
            sourceView, sourcePath,
            destinationView, destinationPath,
            overwrite);
    }

    #endregion
}
