// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial class SemanticVersion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified string.
    /// </summary>
    /// <param name="version">The version string.</param>
    /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="version"/> is empty.</exception>
    /// <exception cref="ArgumentException"><paramref name="version"/> has an invalid format.</exception>
    public SemanticVersion(string version) :
        this(Model.Create(version))
    {
    }
}
