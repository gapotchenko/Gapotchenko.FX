// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Represents a ZIP data archive view on the base storage.
/// </summary>
/// <typeparam name="T">The type of the base storage.</typeparam>
public interface IZipArchiveView<out T> : IZipArchive
{
    /// <summary>
    /// Gets the base storage.
    /// </summary>
    T BaseStorage { get; }
}
