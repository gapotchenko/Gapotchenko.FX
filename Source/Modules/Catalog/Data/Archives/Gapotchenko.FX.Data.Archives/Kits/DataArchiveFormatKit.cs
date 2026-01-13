// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.Data.Archives.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IDataArchiveFormat{TArchive, TOptions}"/> interface.
/// </summary>
/// <remarks/>
/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class DataArchiveFormatKit<TArchive, TOptions> :
    VfsFileStorageFormatKit<TArchive, TOptions>,
    IDataArchiveFormat<TArchive, TOptions>
    where TArchive : IDataArchive
    where TOptions : DataArchiveOptions
{
}
