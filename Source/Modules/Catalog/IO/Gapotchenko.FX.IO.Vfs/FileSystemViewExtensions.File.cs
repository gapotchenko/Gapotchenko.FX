// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_STREAM_SPAN
#endif

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Properties;
using Gapotchenko.FX.IO.Vfs.Utils;
using Gapotchenko.FX.Threading.Tasks;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    #region Open

    /// <inheritdoc cref="File.OpenWrite(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static Stream WriteFile(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.OpenWrite(path);
        else
            return view.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }

    /// <inheritdoc cref="File.OpenText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader ReadTextFile(this IReadOnlyFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(view.ReadFile(path), Encoding.UTF8);
    }

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
        ArgumentNullException.ThrowIfNull(view);

        return view.OpenFile(path, mode, access, FileShare.None);
    }

    #endregion

    #region Create

    /// <inheritdoc cref="File.Create(string)"/>
    /// <param name="view">The file system view.</param>
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
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamWriter CreateTextFile(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.CreateText(path);
        else
            return new StreamWriter(CreateFileForWritingCore(view, path));
    }

    static StreamWriter CreateTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            CreateFileForWritingCore(view, path),
            encoding);
    }

    static Stream CreateFileForWritingCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read);

    #endregion

    #region Append

    /// <summary>
    /// Creates a <see cref="StreamWriter"/> that appends UTF-8 encoded text to an existing file,
    /// or to a new file if the specified file does not exist.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The path to the file to append to.</param>
    /// <returns>A stream writer that appends UTF-8 encoded text to the specified file or to a new file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to a non-existing directory or to an unmapped drive.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    public static StreamWriter AppendTextFile(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.AppendText(path);
        else
            return new StreamWriter(AppendFileCore(view, path));
    }

    /// <summary>
    /// Asynchronously creates a <see cref="StreamWriter"/> that appends UTF-8 encoded text to an existing file,
    /// or to a new file if the specified file does not exist.
    /// </summary>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<StreamWriter> AppendTextFileAsync(this IFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return await TaskBridge.ExecuteAsync(() => File.AppendText(path), cancellationToken).ConfigureAwait(false);
        else
            return new StreamWriter(await AppendFileCoreAsync(view, path, cancellationToken).ConfigureAwait(false));
    }

    static StreamWriter AppendTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            AppendFileCore(view, path),
            encoding);
    }

    static Stream AppendFileCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Append, FileAccess.Write, FileShare.Read);

    static Task<Stream> AppendFileCoreAsync(IFileSystemView view, string path, CancellationToken cancellationToken) =>
        view.OpenFileAsync(path, FileMode.Append, FileAccess.Write, FileShare.Read, cancellationToken);

    #endregion

    #region Read/write bytes

    /// <inheritdoc cref="File.ReadAllBytes(string)"/>
    /// <param name="view">The file system view.</param>
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
            using var stream = view.ReadFile(path);
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
    /// <param name="view">The file system view.</param>
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
            ArgumentNullException.ThrowIfNull(bytes);

            using var stream = CreateFileForWritingCore(view, path);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    #endregion

    #region Read/write/append text

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllFileTextCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadAllText(path, encoding);
        else
            return ReadAllFileTextCore(view, path, encoding);
    }

    static string ReadAllFileTextCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);
        return reader.ReadToEnd();
    }

    /// <inheritdoc cref="File.WriteAllText(string, string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents)
    {
        ArgumentNullException.ThrowIfNull(view);

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
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

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
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents)
    {
        ArgumentNullException.ThrowIfNull(view);

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

    /// <inheritdoc cref="File.AppendAllText(string, string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents)
    {
        ArgumentNullException.ThrowIfNull(view);

#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            File.AppendAllText(path, contents);
        }
        else
#endif
        {
            using var writer = view.AppendTextFile(path);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

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

    /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void AppendAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            File.AppendAllText(path, contents, encoding);
        }
        else
#endif
        {
            using var writer = view.AppendTextFile(path, encoding);
            writer.Write(contents);
        }
    }

    #endregion

    #region Read/write/append lines

    /// <inheritdoc cref="File.ReadLines(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadLines(path);
        else
            return ReadFileLinesCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadLines(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path, encoding);
        else
            return ReadFileLinesCore(view, path, encoding ?? throw new ArgumentNullException(nameof(encoding)));
    }

    static IEnumerable<string> ReadFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);

        while (reader.ReadLine() is not null and var line)
            yield return line;
    }

    /// <inheritdoc cref="File.ReadAllLines(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path);
        else
            return ReadAllFileLinesCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllLines(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.ReadAllLines(path, encoding);
        else
            return ReadAllFileLinesCore(view, path, encoding ?? throw new ArgumentNullException(nameof(encoding)));
    }

    static string[] ReadAllFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);

        var lines = new List<string>();
        while (reader.ReadLine() is not null and var line)
            lines.Add(line);

        return lines.ToArray();
    }

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = view.CreateTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string}, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contents);

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
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = view.AppendTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string}, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(contents);

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
    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool, VfsCopyOptions)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void CopyFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        CopyFile(view, sourcePath, destinationPath, false);

    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool, VfsCopyOptions)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void CopyFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .CopyFile(sourcePath, destinationPath, overwrite, options);

    /// <summary>
    /// Copies an existing file to a new file in the specified destination location.
    /// Overwriting a file of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyFile(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination) =>
        CopyFile(sourceView, sourcePath, destination, false, VfsCopyOptions.None);

    /// <inheritdoc cref="CopyFile(IReadOnlyFileSystemView, string, VfsLocation)"/>
    [Obsolete("Use CopyFile(string, VfsLocation) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath) =>
        throw new InvalidOperationException();

    /// <summary>
    /// Copies an existing file to a new file in the specified destination location.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the file to copy.</param>
    /// <param name="sourcePath">The path of the file to copy.</param>
    /// <param name="destination">
    /// The destination location to copy the file to.
    /// This cannot be a directory.
    /// </param>
    /// <param name="overwrite">
    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool, VfsCopyOptions)" path="//param[@name='overwrite']"/>
    /// </param>
    /// <param name="options">
    /// <inheritdoc cref="IFileSystemView.CopyFile(string, string, bool, VfsCopyOptions)" path="//param[@name='options']"/>
    /// </param>
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyFileOptimized(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options);
    }

    /// <inheritdoc cref="CopyFile(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    [Obsolete("Use CopyFile(string, VfsLocation, bool, VfsCopyOptions) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void CopyFile(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None) =>
        throw new InvalidOperationException();

    #endregion

    #region Move

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveFile(string, string, bool, VfsMoveOptions)"/>
    /// <param name="view">The file system view to move the file at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    public static void MoveFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath) =>
        MoveFile(view, sourcePath, destinationPath, false);

    /// <inheritdoc cref="IFileSystemView.MoveFile(string, string, bool, VfsMoveOptions)"/>
    /// <param name="view">The file system view to move the file at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void MoveFile(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .MoveFile(sourcePath, destinationPath, overwrite, options);

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="MoveFile(IFileSystemView, string, VfsLocation, bool, VfsMoveOptions)"/>
    public static void MoveFile(
        this IFileSystemView sourceView,
        string sourcePath,
        VfsLocation destination) =>
        MoveFile(sourceView, sourcePath, destination, false, VfsMoveOptions.None);

    /// <inheritdoc cref="IFileSystemView.MoveFile(string, string, bool, VfsMoveOptions)"/>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> of the file to move.</param>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destination">The destination location to move the file to.</param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    public static void MoveFile(
        this IFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveFileOptimized(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options);
    }

    #endregion
}
