// Gapotchenko.FX
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
        // SemVer 2.0 standard requires to ignore the build metadata.
        ;

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(
            m_Major.GetHashCode(),
            m_Minor.GetHashCode(),
            m_Patch.GetHashCode(),
            m_Prerelease != null ? StringComparer.Ordinal.GetHashCode(m_Prerelease) : 0);
}
