// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Provides polyfill static methods for <see cref="Version"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class VersionPolyfills
{
    /// <summary>
    /// Deconstructs a <see cref="Version"/> object into major and minor components.
    /// </summary>
    /// <param name="version">The version to deconstruct.</param>
    /// <param name="major">The major component.</param>
    /// <param name="minor">The minor component.</param>
    public static void Deconstruct(this Version version, out int major, out int minor)
    {
        ArgumentNullException.ThrowIfNull(version);

        major = version.Major;
        minor = version.Minor;
    }

    /// <summary>
    /// Deconstructs a <see cref="Version"/> object into major, minor, and build components.
    /// </summary>
    /// <param name="version">The version to deconstruct.</param>
    /// <param name="major">The major component.</param>
    /// <param name="minor">The minor component.</param>
    /// <param name="build">The build component.</param>
    public static void Deconstruct(this Version version, out int major, out int minor, out int build)
    {
        ArgumentNullException.ThrowIfNull(version);

        major = version.Major;
        minor = version.Minor;
        build = version.Build;
    }

    /// <summary>
    /// Deconstructs a <see cref="Version"/> object into major, minor, build, and revision components.
    /// </summary>
    /// <param name="version">The version to deconstruct.</param>
    /// <param name="major">The major component.</param>
    /// <param name="minor">The minor component.</param>
    /// <param name="build">The build component.</param>
    /// <param name="revision">The revision component.</param>
    public static void Deconstruct(this Version version, out int major, out int minor, out int build, out int revision)
    {
        ArgumentNullException.ThrowIfNull(version);

        major = version.Major;
        minor = version.Minor;
        build = version.Build;
        revision = version.Revision;
    }
}
