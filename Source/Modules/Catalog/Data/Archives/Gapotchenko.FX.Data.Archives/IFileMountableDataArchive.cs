// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives;

/// <summary>
/// Defines the interface of a data archive mountable on a file.
/// </summary>
/// <typeparam name="TArchive">The type of the data archive.</typeparam>
/// <typeparam name="TOptions">The type of the data archive options.</typeparam>
public interface IFileMountableDataArchive<out TArchive, TOptions> :
    IDataArchive,
    IFileMountableVfs<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the object for <typeparamref name="TArchive"/> files manipulation.
    /// </summary>
    static new abstract IDataArchiveFile<TArchive, TOptions> File { get; }
#endif
}
