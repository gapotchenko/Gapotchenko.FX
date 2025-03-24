// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Storage.Archives;

/// <summary>
/// Defines the options for a data archive.
/// </summary>
[ImmutableObject(true)]
public abstract record DataArchiveOptions : VfsOptions
{
}
