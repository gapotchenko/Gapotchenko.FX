// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// Deconstructs the version into major and minor components.
    /// </summary>
    /// <param name="major">The major component.</param>
    /// <param name="minor">The minor component.</param>
    public void Deconstruct(out int major, out int minor)
    {
        major = Major;
        minor = Minor;
    }

    /// <summary>
    /// Deconstructs the version into major, minor, and patch components.
    /// </summary>
    /// <param name="major">The major component.</param>
    /// <param name="minor">The minor component.</param>
    /// <param name="patch">The patch component.</param>
    public void Deconstruct(out int major, out int minor, out int patch)
    {
        major = Major;
        minor = Minor;
        patch = Patch;
    }
}
