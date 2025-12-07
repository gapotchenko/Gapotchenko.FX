// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives;

/// <summary>
/// Defines the interface of a data archive mountable on a storage.
/// </summary>
/// <typeparam name="TArchive">The type of the data archive.</typeparam>
/// <typeparam name="TOptions">The type of the data archive options.</typeparam>
public interface IStorageMountableDataArchive<out TArchive, TOptions> :
    IDataArchive,
    IStorageMountableVfs<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the object for <typeparamref name="TArchive"/> storage manipulation.
    /// </summary>
    static new abstract IDataArchiveStorage<TArchive, TOptions> Storage { get; }
#endif
}
