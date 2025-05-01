// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial class SemanticVersion : IEquatable<SemanticVersion>
{
    /// <summary>
    /// Returns a value indicating whether the current <see cref="SemanticVersion"/> object is equal to a specified object.
    /// </summary>
    /// <param name="obj">An object to compare with the current <see cref="SemanticVersion"/> object, or null.</param>
    /// <returns>
    /// <c>true</c> if the current <see cref="SemanticVersion"/> object and <paramref name="obj"/> are both <see cref="SemanticVersion"/> objects,
    /// and every significant component of the current <see cref="SemanticVersion"/> object matches the corresponding
    /// component of <paramref name="obj"/>; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as SemanticVersion);

    /// <summary>
    /// Returns a value indicating whether the current <see cref="SemanticVersion"/> object and a specified
    /// <see cref="SemanticVersion"/> object represent the same value.
    /// </summary>
    /// <param name="obj">A <see cref="SemanticVersion"/> object to compare to the current System.Version object, or null.</param>
    /// <returns>
    /// <c>true</c> if every significant component of the current <see cref="SemanticVersion"/> object matches the corresponding
    /// component of the <paramref name="obj"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(SemanticVersion? obj) =>
        ReferenceEquals(obj, this) ||
        obj is not null &&
        m_Major == obj.Major &&
        m_Minor == obj.m_Minor &&
        m_Patch == obj.m_Patch &&
        // SemVer 2.0 standard requires to ignore 'BuildLabel' (Build metadata).
        string.Equals(m_PreReleaseLabel, obj.m_PreReleaseLabel, StringComparison.Ordinal);

    /// <summary>
    /// Returns a hash code for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(
            m_Major.GetHashCode(),
            m_Minor.GetHashCode(),
            m_Patch.GetHashCode(),
            m_PreReleaseLabel != null ? StringComparer.Ordinal.GetHashCode(m_PreReleaseLabel) : 0);

    /// <summary>
    /// Determines whether two specified <see cref="SemanticVersion"/> objects are equal.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> equals <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(SemanticVersion? v1, SemanticVersion? v2) =>
        ReferenceEquals(v1, v2) ||
        v1 is not null && v1.Equals(v2);

    /// <summary>
    /// Determines whether two specified <see cref="SemanticVersion"/> objects are not equal.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> does not equal <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(SemanticVersion? v1, SemanticVersion? v2) => !(v1 == v2);
}
