// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Defines the options of a ZIP archive.
/// </summary>
[ImmutableObject(true)]
public sealed record ZipArchiveOptions : DataArchiveOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchiveOptions"/> record.
    /// </summary>
    [Obsolete("ZIP archive options are not configurable", true)]
    public ZipArchiveOptions()
    {
    }

#if FUTURE_DEVELOPMENT

    /// <summary>
    /// Gets or initializes the conformance level.
    /// </summary>
    public ZipArchiveConformanceLevel ConformanceLevel { get; init; }

#endif
}
