using System.Diagnostics;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Compression.Zip;

/// <summary>
/// Represents a package of compressed files in the ZIP archive format.
/// </summary>
public class ZipArchive : IDataArchive, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class
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
        ZipArchiveMode mode;
        if (writable)
        {
            if (stream.Length == 0)
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

    /// <inheritdoc/>
    public bool CanWrite { get; }

    #region File

    /// <inheritdoc/>
    public bool FileExists([NotNullWhen(true)] string? path)
    {
        var pathModel = new PathModel(path);
        if (pathModel.IsNil)
            return false;
        return m_UnderlyingArchive.Entries.Any(x => pathModel.Equals(new PathModel(x.FullName)));
    }

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateFiles(string path)
    {
        var pathModel = new PathModel(path);
        if (pathModel.IsNil)
            throw new DirectoryNotFoundException();

        return m_UnderlyingArchive.Entries.Select(x => x.FullName).Where(IsMatch);

        bool IsMatch(string entryPath)
        {
            var entryPathModel = new PathModel(entryPath);
            if (entryPathModel.TryPop() is null)
                return false;
            return entryPathModel.StartsWith(pathModel);
        }
    }

    /// <inheritdoc/>
    public Stream OpenFileForReading(string path) => throw new NotImplementedException();

    #endregion

    #region Path

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path) => path;

    #endregion

    #region Directory

    /// <inheritdoc/>
    public bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        var pathModel = new PathModel(path);
        if (pathModel.IsNil)
            return false;
        if (pathModel.IsRoot)
            return true;

        return m_UnderlyingArchive.Entries.Any(x => GetDirectory(x.FullName).HasValue);

        PathModel? GetDirectory(string entryPath)
        {
            var entryPathModel = new PathModel(entryPath);
            if (entryPathModel.TryPop() is null)
                return null;
            if (entryPathModel.StartsWith(pathModel))
                return entryPathModel;
            else
                return null;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateDirectories(string path)
    {
        var pathModel = new PathModel(path);
        if (pathModel.IsNil)
            throw new DirectoryNotFoundException();

        return m_UnderlyingArchive.Entries
            .Select(x => GetDirectory(x.FullName))
            .Where(x => x.HasValue && !x.Value.IsNil)
            .Select(x => x!.Value)
            .Distinct()
            .Select(x => x.ToString()!);

        PathModel? GetDirectory(string entryPath)
        {
            var entryPathModel = new PathModel(entryPath);
            if (entryPathModel.TryPop() is null)
                return null;
            if (entryPathModel.StartsWith(pathModel))
                return entryPathModel;
            else
                return null;
        }
    }

    #endregion

    #region Entry

    /// <inheritdoc/>
    public bool EntryExists([NotNullWhen(true)] string? path) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEnumerable<string> EnumerateEntries(string path) =>
        EnumerateDirectories(path)
        .Concat(EnumerateFiles(path));

    #endregion

    /// <inheritdoc/>
    public void Dispose()
    {
        m_UnderlyingArchive.Dispose();
        GC.SuppressFinalize(this);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly System.IO.Compression.ZipArchive m_UnderlyingArchive;
}
