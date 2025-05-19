// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Represents a ZIP data archive view that uses <typeparamref name="T"/> as a backing store.
/// </summary>
/// <typeparam name="T">The type of the backing store.</typeparam>
public interface IZipArchiveView<out T> : IZipArchive
{
    /// <summary>
    /// Gets the <typeparamref name="T"/> backing store.
    /// </summary>
    T BackingStore { get; }
}
