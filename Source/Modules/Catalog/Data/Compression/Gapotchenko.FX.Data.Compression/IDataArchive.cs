using Gapotchenko.FX.IO;

namespace Gapotchenko.FX.Data.Compression;

/// <summary>
/// Defines the interface of a compressible data archive.
/// </summary>
public interface IDataArchive : IFileSystemView
{
    /// <summary>
    /// Gets a value indicating whether the current archive supports writing.
    /// </summary>
    bool CanWrite { get; }
}
