// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives;

/// <summary>
/// Provides description and operations for the data archive format.
/// </summary>
public interface IDataArchiveFormat : IVfsFileFormat
{
}

/// <summary>
/// Provides strongly typed description and operations for the data archive format.
/// </summary>
/// <typeparam name="TArchive">The type of the data archive.</typeparam>
/// <typeparam name="TOptions">The type of the data archive options.</typeparam>
public interface IDataArchiveFormat<out TArchive, TOptions> :
    IDataArchiveFormat,
    IVfsFormat<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
}
