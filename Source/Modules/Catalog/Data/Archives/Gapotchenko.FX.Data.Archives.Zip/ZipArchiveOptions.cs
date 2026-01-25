// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

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
    public ZipArchiveOptions()
    {
    }

    /// <summary>
    /// Gets or initializes the encoding to use when reading or writing entry names and comments in ZIP archive.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Specify a value for this option only when an encoding is required for interoperability with ZIP archive tools and libraries
    /// that do not support UTF-8 encoding for entry names or comments.
    /// </para>
    /// <para>
    /// The default value is <see langword="null"/> which is equivalent to UTF-8 encoding.
    /// </para>
    /// </remarks>
    public Encoding? EntryNameEncoding { get; init; }

#if FUTURE_DEVELOPMENT

    /// <summary>
    /// Gets or initializes the conformance level.
    /// </summary>
    public ZipArchiveConformanceLevel ConformanceLevel { get; init; }

#endif
}
