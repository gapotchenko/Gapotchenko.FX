// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// Converts a <see cref="SemanticVersion"/> object to a <see cref="Version"/> object.
    /// </summary>
    /// <remarks>
    /// The conversion is lossy because <see cref="Version"/> class has no properties equivalent to <see cref="PreReleaseLabel"/> and <see cref="BuildLabel"/>.
    /// </remarks>
    /// <param name="version">The semantic version.</param>
    /// <returns>A <see cref="Version"/> object.</returns>
    [return: NotNullIfNotNull(nameof(version))]
    public static implicit operator Version?(SemanticVersion? version) =>
        version is null
            ? null
            : new(version.Major, version.Minor, version.Patch);
}
