// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <inheritdoc/>
    public bool Equals(SemanticVersion? obj) =>
        ReferenceEquals(obj, this) ||
        obj is not null &&
        m_Major == obj.Major &&
        m_Minor == obj.m_Minor &&
        m_Patch == obj.m_Patch &&
        string.Equals(m_Prerelease, obj.m_Prerelease, StringComparison.Ordinal)
        // Semantic Versioning 2.0.0 specification requires to ignore the build metadata.
        ;

    /// <summary>
    /// Returns the hash code for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(
            m_Major.GetHashCode(),
            m_Minor.GetHashCode(),
            m_Patch.GetHashCode(),
            m_Prerelease is { } prerelease ? StringComparer.Ordinal.GetHashCode(prerelease) : 0);
}
