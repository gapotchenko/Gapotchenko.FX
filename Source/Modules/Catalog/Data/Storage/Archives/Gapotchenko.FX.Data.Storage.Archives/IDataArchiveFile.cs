// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Storage.Archives;

/// <summary>
/// Provides operations for working with <typeparamref name="TArchive"/> files.
/// </summary>
/// <typeparam name="TArchive">The type of the data archive.</typeparam>
/// <typeparam name="TOptions">The type of the data archive options.</typeparam>
public interface IDataArchiveFile<out TArchive, TOptions> : IVfsFile<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
    /// <summary>
    /// Gets the file format of the <typeparamref name="TArchive"/> data archive.
    /// </summary>
    new IDataArchiveFileFormat<TArchive, TOptions> Format { get; }
}
