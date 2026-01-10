// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Properties;
using Gapotchenko.FX.IO.Vfs.Utils;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    #region Open

    /// <summary>
    /// Opens an existing file or creates a new file for writing.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to be opened for writing.</param>
    /// <returns>An unshared <see cref="Stream"/> object on the specified path with <see cref="FileAccess.Write"/> access.</returns>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static Stream WriteFile(this IFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.OpenWrite(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            return view.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }
    }

    /// <summary>
    /// Asynchronously opens an existing file or creates a new file for writing.
    /// </summary>
    /// <inheritdoc cref="WriteFile(IFileSystemView, string)"/>
    public static Task<Stream> WriteFileAsync(this IFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.OpenFileAsync(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, cancellationToken);
    }

    /// <summary>
    /// Opens an existing UTF-8 encoded text file for reading.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to be opened for reading.</param>
    /// <inheritdoc cref="IReadOnlyFileSystemView.ReadFile(string)" path="/exception" />
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static StreamReader ReadTextFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.OpenText(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            return new StreamReader(view.ReadFile(path), Encoding.UTF8);
        }
    }

    /// <summary>
    /// Asynchronously opens an existing UTF-8 encoded text file for reading.
    /// </summary>
    /// <inheritdoc cref="ReadTextFile(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<StreamReader> ReadTextFileAsync(
        this IReadOnlyFileSystemView view,
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return new StreamReader(
            await view.ReadFileAsync(path, cancellationToken).ConfigureAwait(false),
            Encoding.UTF8);
    }

    /// <summary>
    /// Opens a <see cref="Stream"/> on the specified path with read/write access with no sharing.
    /// </summary>
    /// <returns>
    /// A <see cref="Stream"/> opened in the specified mode and path, with read/write access and not shared.
    /// </returns>
    /// <inheritdoc cref="OpenFile(IFileSystemView, string, FileMode, FileAccess)"/>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode) =>
        OpenFile(view, path, mode, FileAccess.ReadWrite);

    /// <summary>
    /// Asynchronously opens a <see cref="Stream"/> on the specified path with read/write access with no sharing.
    /// </summary>
    /// <inheritdoc cref="OpenFile(IFileSystemView, string, FileMode)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<Stream> OpenFileAsync(this IFileSystemView view, string path, FileMode mode, CancellationToken cancellationToken = default) =>
        OpenFileAsync(view, path, mode, FileAccess.ReadWrite, cancellationToken);

    /// <summary>
    /// Opens a <see cref="Stream"/> on the specified path, with the specified mode and access with no sharing.
    /// </summary>
    /// <returns>
    /// An unshared <see cref="Stream"/> that provides access to the specified file, with the specified mode and access.
    /// </returns>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" />
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="access"><inheritdoc/></param>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static Stream OpenFile(this IFileSystemView view, string path, FileMode mode, FileAccess access)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.OpenFile(path, mode, access, FileShare.None);
    }

    /// <summary>
    /// Asynchronously opens a <see cref="Stream"/> on the specified path, with the specified mode and access with no sharing.
    /// </summary>
    /// <inheritdoc cref="OpenFile(IFileSystemView, string, FileMode, FileAccess)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="access"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<Stream> OpenFileAsync(
        this IFileSystemView view,
        string path,
        FileMode mode,
        FileAccess access,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.OpenFileAsync(path, mode, access, FileShare.None, cancellationToken);
    }

    #endregion

    #region Create

    /// <summary>
    /// Creates, or truncates and overwrites, a file in the specified path.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The path and name of the file to create.</param>
    /// <returns>A <see cref="Stream"/> that provides read/write access to the file specified in <paramref name="path"/>.</returns>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
    public static Stream CreateFile(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.Create(path);
        else
            return view.OpenFile(path, FileMode.Create);
    }

    /// <summary>
    /// Asynchronously creates, or truncates and overwrites, a file in the specified path.
    /// </summary>
    /// <inheritdoc cref="CreateFile(IFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<Stream> CreateFileAsync(this IFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.OpenFileAsync(path, FileMode.Create, cancellationToken);
    }

    /// <summary>
    /// Creates or opens a file for writing UTF-8 encoded text.
    /// If the file already exists, its contents are replaced.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to be opened for writing.</param>
    /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using UTF-8 encoding.</returns>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static StreamWriter CreateTextFile(this IFileSystemView view, string path)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view is LocalFileSystemView)
            return File.CreateText(path);
        else
            return new StreamWriter(CreateFileForWritingCore(view, path));
    }

    /// <summary>
    /// Asynchronously creates or opens a file for writing UTF-8 encoded text.
    /// If the file already exists, its contents are replaced.
    /// </summary>
    /// <inheritdoc cref="CreateTextFile(IFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<StreamWriter> CreateTextFileAsync(
        this IFileSystemView view,
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return new StreamWriter(await CreateFileForWritingCoreAsync(view, path, cancellationToken).ConfigureAwait(false));
    }

    static StreamWriter CreateTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            CreateFileForWritingCore(view, path),
            encoding);
    }

    static async Task<StreamWriter> CreateTextFileAsync(
        this IFileSystemView view,
        string path,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            await CreateFileForWritingCoreAsync(view, path, cancellationToken).ConfigureAwait(false),
            encoding);
    }

    static Stream CreateFileForWritingCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read);

    static Task<Stream> CreateFileForWritingCoreAsync(IFileSystemView view, string path, CancellationToken cancellationToken) =>
        view.OpenFileAsync(path, FileMode.Create, FileAccess.Write, FileShare.Read, cancellationToken);

    #endregion

    #region Append

    /// <summary>
    /// Creates a <see cref="StreamWriter"/> that appends UTF-8 encoded text to an existing file,
    /// or to a new file if the specified file does not exist.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The path to the file to append to.</param>
    /// <returns>A stream writer that appends UTF-8 encoded text to the specified file or to a new file.</returns>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
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

        return new StreamWriter(await AppendFileCoreAsync(view, path, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Creates a <see cref="StreamWriter"/> that appends text using the specified encoding to an existing file,
    /// or to a new file if the specified file does not exist.
    /// </summary>
    /// <returns>A stream writer that appends text using the specified encoding to the specified file or to a new file.</returns>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    public static StreamWriter AppendTextFile(this IFileSystemView view, string path, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            AppendFileCore(view, path),
            encoding);
    }

    static Stream AppendFileCore(IFileSystemView view, string path) =>
        view.OpenFile(path, FileMode.Append, FileAccess.Write, FileShare.Read);

    /// <summary>
    /// Asynchronously creates a <see cref="StreamWriter"/> that appends text using the specified encoding to an existing file,
    /// or to a new file if the specified file does not exist.
    /// </summary>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<StreamWriter> AppendTextFileAsync(
        this IFileSystemView view,
        string path,
        Encoding encoding,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(encoding);

        return new StreamWriter(
            await AppendFileCoreAsync(view, path, cancellationToken).ConfigureAwait(false),
            encoding);
    }

    static Task<Stream> AppendFileCoreAsync(IFileSystemView view, string path, CancellationToken cancellationToken) =>
        view.OpenFileAsync(path, FileMode.Append, FileAccess.Write, FileShare.Read, cancellationToken);

    #endregion

    #region Read/write bytes

    #region Read bytes

    /// <summary>
    /// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to read.</param>
    /// <returns>A byte array containing the contents of the file.</returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.ReadFile(string)" path="/exception" />
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static byte[] ReadAllFileBytes(this IReadOnlyFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            using var stream = view.ReadFile(path);
            return ReadAllBytesCore(stream);
        }
    }

    /// <summary>
    /// Asynchronously opens a binary file, reads the contents of the file into a byte array, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileBytes(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<byte[]> ReadAllFileBytesAsync(this IReadOnlyFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadAllBytesAsync(path, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            return CoreImpl(view, path, cancellationToken);

            static async Task<byte[]> CoreImpl(IReadOnlyFileSystemView view, string path, CancellationToken cancellationToken)
            {
                using var stream = await view.ReadFileAsync(path, cancellationToken).ConfigureAwait(false);
                return await ReadAllBytesCoreAsync(stream, cancellationToken).ConfigureAwait(false);
            }
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

    static async ValueTask<byte[]> ReadAllBytesCoreAsync(Stream stream, CancellationToken cancellationToken)
    {
        long length = stream.CanSeek ? stream.Length : -1;

        if (length == 0)
            return [];
        else if (length == -1)
            return await ReadAllBytesUnknownLengthAsync(stream, cancellationToken).ConfigureAwait(false);
        else if (length > ArrayHelper.ArrayMaxLength)
            throw new IOException(Resources.FileTooLong2GB);

        int count = (int)length;
        byte[] bytes = new byte[count];
        await stream.ReadExactlyAsync(bytes, 0, count, cancellationToken).ConfigureAwait(false);
        return bytes;
    }

    static byte[] ReadAllBytesUnknownLength(Stream stream)
    {
        var arrayPool = ArrayPool<byte>.Shared;
        byte[]? rentedArray = null;
        try
        {
            const int initialBufferSize = 512;
#if TFF_STREAM_SPAN
            Span<byte> buffer = stackalloc byte[initialBufferSize];
#else
#if TFF_CER
            try
            {
            }
            finally
#endif
            {
                rentedArray = arrayPool.Rent(initialBufferSize);
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

    static async ValueTask<byte[]> ReadAllBytesUnknownLengthAsync(Stream stream, CancellationToken cancellationToken)
    {
        var arrayPool = ArrayPool<byte>.Shared;

        byte[] buffer = arrayPool.Rent(512);
        try
        {
            for (int bytesRead = 0; ;)
            {
                Debug.Assert(bytesRead < buffer.Length);
                int n =
                    await stream
                    .ReadAsync(
#if TFF_STREAM_SPAN
                        buffer.AsMemory(bytesRead, buffer.Length - bytesRead),
#else
                        buffer, bytesRead, buffer.Length - bytesRead,
#endif
                        cancellationToken)
                    .ConfigureAwait(false);

                if (n == 0)
                {
                    // The end of stream.
                    return buffer.AsSpan(0, bytesRead).ToArray();
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
                        byte[] newBuffer = arrayPool.Rent((int)newLength);
                        buffer.AsSpan().CopyTo(newBuffer);
                        arrayPool.Return(buffer);
                        buffer = newBuffer;
                    }
                }
            }
        }
        finally
        {
            if (buffer != null)
                arrayPool.Return(buffer);
        }
    }

    #endregion

    #region Write bytes

    /// <inheritdoc cref="WriteAllFileBytes(IFileSystemView, string, ReadOnlySpan{byte})"/>
    /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is <see langword="null"/>.</exception>
    public static void WriteAllFileBytes(this IFileSystemView view, string path, byte[] bytes)
    {
        if (view is LocalFileSystemView)
        {
            File.WriteAllBytes(path, bytes);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(bytes);

            using var stream = CreateFileForWritingCore(view, path);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    /// <inheritdoc cref="WriteAllFileBytesAsync(IFileSystemView, string, ReadOnlyMemory{byte}, CancellationToken)"/>
    public static async Task WriteAllFileBytesAsync(this IFileSystemView view, string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllBytesAsync(path, bytes, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(bytes);

            using var stream = await CreateFileForWritingCoreAsync(view, path, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(bytes.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified bytes to the file, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="bytes">The bytes to write to the file.</param>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static void WriteAllFileBytes(this IFileSystemView view, string path, ReadOnlySpan<byte> bytes)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            File.WriteAllBytes(path, bytes);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            using var stream = CreateFileForWritingCore(view, path);
            stream.Write(bytes);
        }
    }

    /// <summary>
    /// Asynchronously creates a new file, writes the specified bytes to the file, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileBytes(IFileSystemView, string, ReadOnlySpan{byte})"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="bytes"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllFileBytesAsync(
        this IFileSystemView view,
        string path,
        ReadOnlyMemory<byte> bytes,
        CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllBytesAsync(path, bytes, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            using var stream = await CreateFileForWritingCoreAsync(view, path, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #endregion

    #region Read/write/append text

    #region Read text

    /// <summary>
    /// Opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to read.</param>
    /// <returns>A string containing all the text in the file.</returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.ReadFile(string)" path="/exception" />
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllText(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadAllFileTextCore(view, path, Encoding.UTF8);
        }
    }

    /// <summary>
    /// Asynchronously opens a file, reads all text in the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileText(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation, which contains the contents of the file.</returns>
    public static Task<string> ReadAllFileTextAsync(this IReadOnlyFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadAllTextAsync(path, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadAllFileTextCoreAsync(view, path, Encoding.UTF8, cancellationToken);
        }
    }

    /// <summary>
    /// Opens a file, reads all text in the file with the specified encoding, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileText(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    public static string ReadAllFileText(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllText(path, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadAllFileTextCore(view, path, encoding);
        }
    }

    /// <summary>
    /// Asynchronously opens a file, reads all text in the file with the specified encoding, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileTextAsync(IReadOnlyFileSystemView, string, CancellationToken)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    /// <returns>A task that represents the asynchronous read operation, which contains the contents of the file.</returns>
    public static Task<string> ReadAllFileTextAsync(this IReadOnlyFileSystemView view, string path, Encoding encoding, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadAllTextAsync(path, encoding, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadAllFileTextCoreAsync(view, path, encoding, cancellationToken);
        }
    }

    static string ReadAllFileTextCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);
        return reader.ReadToEnd();
    }

    static async Task<string> ReadAllFileTextCoreAsync(IReadOnlyFileSystemView view, string path, Encoding encoding, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(
            await view.ReadFileAsync(path, cancellationToken).ConfigureAwait(false),
            encoding);

        return await reader.ReadToEndAsync(
#if NET7_0_OR_GREATER
            cancellationToken
#endif
            ).ConfigureAwait(false);
    }

    #endregion

    #region Write text

    /// <summary>
    /// Creates a new file, write the contents to the file, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The text to write to the file.</param>
    /// <inheritdoc cref="IFileSystemView.OpenFile(string, FileMode, FileAccess, FileShare)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents)
    {
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

    /// <summary>
    /// Asynchronously creates a new file, write the contents to the file, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileText(IFileSystemView, string, string?)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllFileTextAsync(this IFileSystemView view, string path, string? contents, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllTextAsync(path, contents, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.CreateTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="WriteAllFileText(IFileSystemView, string, string?)"/>
    public static void WriteAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            File.WriteAllText(path, contents);
        }
        else
#endif
        {
            using var writer = view.CreateTextFile(path);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="WriteAllFileTextAsync(IFileSystemView, string, string?, CancellationToken)"/>
    public static async Task WriteAllFileTextAsync(
        this IFileSystemView view,
        string path,
        ReadOnlyMemory<char> contents,
        CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllTextAsync(path, contents, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.CreateTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileText(IFileSystemView, string, string?)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    public static void WriteAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
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

    /// <summary>
    /// Asynchronously creates a new file, writes the specified string to the file using the specified encoding, and then closes the file.
    /// If the target file already exists, it is truncated and overwritten.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileText(IFileSystemView, string, string?, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllFileTextAsync(
        this IFileSystemView view,
        string path,
        string? contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllTextAsync(path, contents, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.CreateTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="WriteAllFileText(IFileSystemView, string, string?, Encoding)"/>
    public static void WriteAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents, Encoding encoding)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            File.WriteAllText(path, contents, encoding);
        }
        else
#endif
        {
            using var writer = view.CreateTextFile(path, encoding);
            writer.Write(contents);
        }
    }

    /// <inheritdoc cref="WriteAllFileTextAsync(IFileSystemView, string, string?, Encoding, CancellationToken)"/>
    public static async Task WriteAllFileTextAsync(
        this IFileSystemView view,
        string path,
        ReadOnlyMemory<char> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllTextAsync(path, contents, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.CreateTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #region Append text

    /// <summary>
    /// Opens a file, appends the specified string to the file, and then closes the file.
    /// If the file does not exist, this method creates a file, writes the specified string to the file, then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to append to.</param>
    /// <param name="contents">The text to append to the file.</param>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string)" path="/exception"/>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents)
    {
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

    /// <inheritdoc cref="AppendAllFileText(IFileSystemView, string, string?)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task AppendAllFileTextAsync(this IFileSystemView view, string path, string? contents, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllTextAsync(path, contents, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.AppendTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                writer.WriteAsync(contents.AsMemory(), cancellationToken)
#else
                writer.WriteAsync(contents)
#endif
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="AppendAllFileText(IFileSystemView, string, string?)"/>
    public static void AppendAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents)
    {
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

    /// <inheritdoc cref="AppendAllFileTextAsync(IFileSystemView, string, string?, CancellationToken)"/>
    public static async Task AppendAllFileTextAsync(this IFileSystemView view, string path, ReadOnlyMemory<char> contents, CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllTextAsync(path, contents, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.AppendTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="AppendAllFileText(IFileSystemView, string, string?)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    public static void AppendAllFileText(this IFileSystemView view, string path, string? contents, Encoding encoding)
    {
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

    /// <inheritdoc cref="AppendAllFileText(IFileSystemView, string, string?, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task AppendAllFileTextAsync(
        this IFileSystemView view,
        string path,
        string? contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllTextAsync(path, contents, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.AppendTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="AppendAllFileText(IFileSystemView, string, string?, Encoding)"/>
    public static void AppendAllFileText(this IFileSystemView view, string path, ReadOnlySpan<char> contents, Encoding encoding)
    {
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

    /// <inheritdoc cref="AppendTextFileAsync(IFileSystemView, string, Encoding, CancellationToken)"/>
    public static async Task AppendAllFileTextAsync(
        this IFileSystemView view,
        string path,
        ReadOnlyMemory<char> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NET9_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllTextAsync(path, contents, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            using var writer = await view.AppendTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await writer.WriteAsync(contents, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #endregion

    #region Read/write/append lines

    #region Read lines

    /// <summary>
    /// Reads the lines of a file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to read.</param>
    /// <returns>A sequence of lines of the file.</returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.ReadFile(string)" path="/exception" />
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadLines(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadFileLinesCore(view, path, Encoding.UTF8);
        }
    }

    /// <summary>
    /// Asynchronously reads the lines of a file.
    /// </summary>
    /// <inheritdoc cref="ReadFileLines(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static IAsyncEnumerable<string> ReadFileLinesAsync(
        this IReadOnlyFileSystemView view,
        string path,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadLinesAsync(path, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadFileLinesCoreAsync(view, path, Encoding.UTF8, cancellationToken);
        }
    }

    /// <summary>
    /// Reads the lines of a file that has a specified encoding.
    /// </summary>
    /// <inheritdoc cref="ReadFileLines(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    public static IEnumerable<string> ReadFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllLines(path, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadFileLinesCore(view, path, encoding);
        }
    }

    /// <summary>
    /// Asynchronously reads the lines of a file that has a specified encoding.
    /// </summary>
    /// <inheritdoc cref="ReadFileLines(IReadOnlyFileSystemView, string, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static IAsyncEnumerable<string> ReadFileLinesAsync(
        this IReadOnlyFileSystemView view,
        string path,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadLinesAsync(path, encoding, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadFileLinesCoreAsync(view, path, encoding, cancellationToken);
        }
    }

    static IEnumerable<string> ReadFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);

        while (reader.ReadLine() is not null and var line)
            yield return line;
    }

    static IAsyncEnumerable<string> ReadFileLinesCoreAsync(IReadOnlyFileSystemView view, string path, Encoding encoding, CancellationToken cancellationToken)
    {
        return CoreImpl(view, path, encoding, cancellationToken);

        static async IAsyncEnumerable<string> CoreImpl(
            IReadOnlyFileSystemView view,
            string path,
            Encoding encoding,
            CancellationToken cancellationToken,
            [EnumeratorCancellation] CancellationToken enumeratorCancellationToken = default)
        {
            using var reader = new StreamReader(await view.ReadFileAsync(path, cancellationToken).ConfigureAwait(false), encoding);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, enumeratorCancellationToken);

            while (await reader.ReadLineAsync(
#if NET7_0_OR_GREATER
                cts.Token
#endif
                ).ConfigureAwait(false) is { } line)
            {
                yield return line;

#if !NET7_0_OR_GREATER
                cts.Token.ThrowIfCancellationRequested();
#endif
            }
        }
    }

    /// <summary>
    /// Opens a text file, reads all lines of the file, and then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to read.</param>
    /// <returns>A string array containing all lines of the file.</returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.ReadFile(string)" path="/exception" />
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllLines(path);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadAllFileLinesCore(view, path, Encoding.UTF8);
        }
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all lines of the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileLines(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<string[]> ReadAllFileLinesAsync(this IReadOnlyFileSystemView view, string path, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadAllLinesAsync(path, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);

            return ReadAllFileLinesCoreAsync(view, path, Encoding.UTF8, cancellationToken);
        }
    }

    /// <inheritdoc cref="ReadAllFileLines(IReadOnlyFileSystemView, string)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    public static string[] ReadAllFileLines(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllLines(path, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadAllFileLinesCore(view, path, encoding);
        }
    }

    /// <summary>
    /// Asynchronously opens a text file, reads all lines of the file with the specified encoding, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="ReadAllFileLinesAsync(IReadOnlyFileSystemView, string, CancellationToken)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task<string[]> ReadAllFileLinesAsync(this IReadOnlyFileSystemView view, string path, Encoding encoding, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            return File.ReadAllLinesAsync(path, encoding, cancellationToken);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(encoding);

            return ReadAllFileLinesCoreAsync(view, path, encoding, cancellationToken);
        }
    }

    static string[] ReadAllFileLinesCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        using var reader = new StreamReader(view.ReadFile(path), encoding);

        var lines = new List<string>();
        while (reader.ReadLine() is not null and var line)
            lines.Add(line);

        return [.. lines];
    }

    static async Task<string[]> ReadAllFileLinesCoreAsync(IReadOnlyFileSystemView view, string path, Encoding encoding, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(
            await view.ReadFileAsync(path, cancellationToken).ConfigureAwait(false),
            encoding);

        var lines = new List<string>();
        while (await reader.ReadLineAsync(
#if NET7_0_OR_GREATER
            cancellationToken
#endif
            ).ConfigureAwait(false) is { } line)
        {
#if !NET7_0_OR_GREATER
            cancellationToken.ThrowIfCancellationRequested();
#endif
            lines.Add(line);
        }

        return [.. lines];
    }

    #endregion

    #region Write lines

    /// <summary>
    /// Creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to write to.</param>
    /// <param name="contents">The lines to write to the file.</param>
    /// <inheritdoc cref="CreateTextFile(IFileSystemView, string)" path="/exception" />
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = view.CreateTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <summary>
    /// Asynchronously creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileLines(IFileSystemView, string, IEnumerable{string?})"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllFileLinesAsync(
        this IFileSystemView view,
        string path,
        IEnumerable<string?> contents,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllLinesAsync(path, contents!, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = await view.CreateTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await WriteAllLinesCoreAsync(writer, contents, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileLines(IFileSystemView, string, IEnumerable{string?})" />
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    public static void WriteAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            File.WriteAllLines(path, contents!, encoding);
        }
        else
        {
            using var writer = view.CreateTextFile(path, encoding);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <summary>
    /// Asynchronously creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="WriteAllFileLines(IFileSystemView, string, IEnumerable{string?}, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task WriteAllFileLinesAsync(
        this IFileSystemView view,
        string path,
        IEnumerable<string?> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.WriteAllLinesAsync(path, contents!, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = await view.CreateTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await WriteAllLinesCoreAsync(writer, contents, cancellationToken).ConfigureAwait(false);
        }
    }

    static void WriteAllLinesCore(StreamWriter writer, IEnumerable<string?> contents)
    {
        foreach (string? line in contents)
            writer.WriteLine(line);
    }

    static async Task WriteAllLinesCoreAsync(StreamWriter writer, IEnumerable<string?> contents, CancellationToken cancellationToken)
    {
        foreach (string? line in contents)
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Append lines

    /// <summary>
    /// Appends lines to a file, and then closes the file.
    /// If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
    /// </summary>
    /// <param name="view">The file system view.</param>
    /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
    /// <param name="contents">The lines to append to the file.</param>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="contents"/> is <see langword="null"/>.</exception>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents)
    {
        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = view.AppendTextFile(path);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="AppendAllFileLines(IFileSystemView, string, IEnumerable{string?})"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous append operation.</returns>
    public static async Task AppendAllFileLinesAsync(
        this IFileSystemView view,
        string path,
        IEnumerable<string?> contents,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllLinesAsync(path, contents!, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = await view.AppendTextFileAsync(path, cancellationToken).ConfigureAwait(false);
            await WriteAllLinesCoreAsync(writer, contents, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Appends lines to a file by using a specified encoding, and then closes the file.
    /// If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.
    /// </summary>
    /// <inheritdoc cref="AppendAllFileLines(IFileSystemView, string, IEnumerable{string?})"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
    /// <param name="contents">The lines to append to the file.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <inheritdoc cref="AppendTextFile(IFileSystemView, string, Encoding)" path="/exception"/>
    /// <exception cref="ArgumentNullException"><paramref name="contents"/> is <see langword="null"/>.</exception>
    public static void AppendAllFileLines(this IFileSystemView view, string path, IEnumerable<string?> contents, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            File.AppendAllLines(path, contents!, encoding);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = view.AppendTextFile(path, encoding);
            WriteAllLinesCore(writer, contents);
        }
    }

    /// <inheritdoc cref="AppendAllFileLines(IFileSystemView, string, IEnumerable{string?}, Encoding)"/>
    /// <param name="view"><inheritdoc/></param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="contents"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous append operation.</returns>
    public static async Task AppendAllFileLinesAsync(
        this IFileSystemView view,
        string path,
        IEnumerable<string?> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (view is LocalFileSystemView)
        {
            await File.AppendAllLinesAsync(path, contents!, encoding, cancellationToken).ConfigureAwait(false);
        }
        else
#endif
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(contents);

            using var writer = await view.AppendTextFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);
            await WriteAllLinesCoreAsync(writer, contents, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

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

    /// <summary>
    /// Asynchronously copies an existing file to a new file.
    /// Overwriting a file of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.CopyFileAsync(string, string, bool, VfsCopyOptions, CancellationToken)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task CopyFileAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default) =>
        CopyFileAsync(view, sourcePath, destinationPath, false, cancellationToken: cancellationToken);

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
        VfsCopyOptions options = VfsCopyOptions.None)
    {
        ArgumentNullException.ThrowIfNull(view);

        view.CopyFile(sourcePath, destinationPath, overwrite, options);
    }

    /// <inheritdoc cref="IFileSystemView.CopyFileAsync(string, string, bool, VfsCopyOptions, CancellationToken)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task CopyFileAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.CopyFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken);
    }

    // ------------------------------------------------------------------------

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
    /// Asynchronously copies an existing file to a new file in the specified destination location.
    /// Overwriting a file of the same name is not allowed.
    /// </summary>
    /// <inheritdoc cref="CopyFileAsync(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions, CancellationToken)"/>
    public static Task CopyFileAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        CancellationToken cancellationToken = default) =>
        CopyFileAsync(sourceView, sourcePath, destination, false, VfsCopyOptions.None, cancellationToken);

    /// <inheritdoc cref="CopyFileAsync(IReadOnlyFileSystemView, string, VfsLocation, CancellationToken)"/>
    [Obsolete("Use CopyFileAsync(string, VfsLocation, CancellationToken) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyFileAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default) =>
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

    /// <summary>
    /// <inheritdoc cref="CopyFile(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions)"/>
    /// </summary>
    /// <param name="sourceView"><inheritdoc/></param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destination"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous file copy operation.</returns>
    public static Task CopyFileAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        return IOHelper.CopyFileOptimizedAsync(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options,
            cancellationToken);
    }

    /// <inheritdoc cref="CopyFileAsync(IReadOnlyFileSystemView, string, VfsLocation, bool, VfsCopyOptions, CancellationToken)"/>
    [Obsolete("Use CopyFileAsync(string, VfsLocation, bool, VfsCopyOptions, CancellationToken) method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyFileAsync(
        this IReadOnlyFileSystemView sourceView, string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsCopyOptions options = VfsCopyOptions.None,
        CancellationToken cancellationToken = default) =>
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

    /// <summary>
    /// Asynchronously moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="IFileSystemView.MoveFileAsync(string, string, bool, VfsMoveOptions, CancellationToken)"/>
    /// <param name="view">The file system view to move the file at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task MoveFileAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default) =>
        MoveFileAsync(view, sourcePath, destinationPath, false, VfsMoveOptions.None, cancellationToken);

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
        VfsMoveOptions options = VfsMoveOptions.None)
    {
        ArgumentNullException.ThrowIfNull(view);

        view.MoveFile(sourcePath, destinationPath, overwrite, options);
    }

    /// <inheritdoc cref="IFileSystemView.MoveFileAsync(string, string, bool, VfsMoveOptions, CancellationToken)"/>
    /// <param name="view">The file system view to move the file at.</param>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task MoveFileAsync(
        this IFileSystemView view,
        string sourcePath,
        string destinationPath,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(view);

        return view.MoveFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken);
    }

    // ------------------------------------------------------------------------

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

    /// <summary>
    /// Asynchronously moves a specified file to a new location,
    /// providing the option to specify a new file name.
    /// </summary>
    /// <inheritdoc cref="MoveFileAsync(IFileSystemView, string, VfsLocation, bool, VfsMoveOptions, CancellationToken)"/>
    public static Task MoveFileAsync(
        this IFileSystemView sourceView,
        string sourcePath,
        VfsLocation destination,
        CancellationToken cancellationToken = default) =>
        MoveFileAsync(sourceView, sourcePath, destination, false, VfsMoveOptions.None, cancellationToken);

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

    /// <inheritdoc cref="IFileSystemView.MoveFileAsync(string, string, bool, VfsMoveOptions, CancellationToken)"/>
    /// <param name="sourceView">The source <see cref="IFileSystemView"/> of the file to move.</param>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destination">The destination location to move the file to.</param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task MoveFileAsync(
        this IFileSystemView sourceView, string sourcePath,
        VfsLocation destination,
        bool overwrite = false,
        VfsMoveOptions options = VfsMoveOptions.None,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        return IOHelper.MoveFileOptimizedAsync(
            new(sourceView, sourcePath),
            destination,
            overwrite,
            options,
            cancellationToken);
    }

    #endregion
}
