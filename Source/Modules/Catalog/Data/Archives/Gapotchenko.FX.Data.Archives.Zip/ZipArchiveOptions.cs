// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Defines the options for a ZIP archive.
/// </summary>
[ImmutableObject(true)]
public sealed record ZipArchiveOptions : DataArchiveOptions
{
    // Prohibit instantiation of the type.
    ZipArchiveOptions()
    {
    }
}
