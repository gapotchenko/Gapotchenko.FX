// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives;

/// <summary>
/// Provides storage operations for working with <typeparamref name="TArchive"/> data archives.
/// </summary>
/// <typeparam name="TArchive">The type of the data archive.</typeparam>
/// <typeparam name="TOptions">The type of the data archive options.</typeparam>
public interface IDataArchiveStorage<TArchive, TOptions> : IVfsStorage<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
    /// <summary>
    /// Gets the storage format for the <typeparamref name="TArchive"/>.
    /// </summary>
    new IDataArchiveFormat<TArchive, TOptions> Format { get; }
}
