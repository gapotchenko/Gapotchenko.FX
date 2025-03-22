using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives;

/// <summary>
/// Defines the interface of a compressible data archive.
/// </summary>
public interface IDataArchive : IFileSystemView, IDisposable
{
}
