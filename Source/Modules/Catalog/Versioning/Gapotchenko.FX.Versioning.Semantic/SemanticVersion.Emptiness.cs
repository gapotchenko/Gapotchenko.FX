// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion : IEmptiable<SemanticVersion>
{
    /// <summary>
    /// Gets the empty <see cref="SemanticVersion"/>.
    /// </summary>
    /// <remarks>
    /// An empty semantic version is represented by the <c>0.0.0</c> value.
    /// </remarks>
    public static SemanticVersion Empty => EmptyFactory.Instance;

    static class EmptyFactory
    {
        public static readonly SemanticVersion Instance = new(0, 0, 0);
    }

    /// <summary>
    /// Gets a value indicating whether the current semantic version is empty.
    /// </summary>
    /// <remarks>
    /// An empty semantic version is represented by the <c>0.0.0</c> value.
    /// </remarks>
    public bool IsEmpty =>
        m_Major is 0 &&
        m_Minor is 0 &&
        m_Patch is 0 &&
        m_Prerelease is null &&
        m_Build is null;
}
