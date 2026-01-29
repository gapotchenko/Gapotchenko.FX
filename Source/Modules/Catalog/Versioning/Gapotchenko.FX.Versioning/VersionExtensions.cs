// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Provides extensions methods for <see cref="Version"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class VersionExtensions
{
    /// <summary>
    /// Gets a version based on the given version with the specified number of components set.
    /// </summary>
    /// <remarks>
    /// For example, version <c>1.2.3.4</c> with three components is <c>1.2.3</c>.
    /// </remarks>
    /// <param name="version">The version.</param>
    /// <param name="count">The number of components.</param>
    /// <returns>The version based on the given version with the specified number of components set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 1 or greater than 4.</exception>
    public static Version WithComponents(this Version version, int count)
    {
        ArgumentNullException.ThrowIfNull(version);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, 4);

        switch (count)
        {
            case 1:
                if (version.Build == -1 && version.Minor == 0)
                    return version;
                else
                    return new Version(version.Major, 0);

            case 2:
                if (version.Build == -1)
                    return version;
                else
                    return new Version(version.Major, version.Minor);

            case 3:
                if (version.Build != -1 && version.Revision == -1)
                    return version;
                else
                    return new Version(version.Major, version.Minor, Empty.Nullify(version.Build, -1) ?? 0);

            case 4:
                if (version.Build != -1 && version.Revision != -1)
                    return version;
                else
                    return new Version(version.Major, version.Minor, Empty.Nullify(version.Build, -1) ?? 0, Empty.Nullify(version.Revision, -1) ?? 0);

            default:
                throw new SwitchExpressionException(count);
        }
    }
}
