// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Represents a semantic version.
/// </summary>
[Serializable]
[ImmutableObject(true)]
[TypeConverter(typeof(SemanticVersionConverter))]
public sealed partial record SemanticVersion
{
    /// <summary>
    /// Gets or initializes the value of the major component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The major version number.</value>
    public int Major
    {
        get => m_Major;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            m_Major = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Major;

    /// <summary>
    /// Gets or initializes the value of the minor component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The minor version number.</value>
    public int Minor
    {
        get => m_Minor;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            m_Minor = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Minor;

    /// <summary>
    /// Gets or initializes the value of the patch component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The patch number.</value>
    public int Patch
    {
        get => m_Patch;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            m_Patch = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Patch;

    /// <summary>
    /// Gets or initializes the value of the prerelease metadata of the version label for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The prerelease metadata, or <see langword="null"/> if there is no prerelease metadata.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? Prerelease
    {
        get => m_Prerelease;
        init
        {
            ValidateLabelMetadata(value);
            m_Prerelease = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_Prerelease;

    /// <summary>
    /// Gets or initializes the value of the build metadata of the version label for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The build metadata, or <see langword="null"/> if there is no build metadata.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? Build
    {
        get => m_Build;
        init
        {
            ValidateLabelMetadata(value);
            m_Build = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_Build;

    #region Formatting

    /// <summary>
    /// Converts the value of the current <see cref="SemanticVersion"/> object to its equivalent <see cref="String"/> representation.
    /// </summary>
    /// <returns>The string representation of the current <see cref="SemanticVersion"/> object.</returns>
    public override string ToString() => m_CachedString ??= ToStringCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_CachedString;

    string ToStringCore()
    {
        var builder = new StringBuilder()
            .Append(m_Major)
            .Append('.').Append(m_Minor)
            .Append('.').Append(m_Patch);

        if (m_Prerelease is not null and var prerelease)
            builder.Append('-').Append(prerelease);

        if (m_Build is not null and var build)
            builder.Append('+').Append(build);

        return builder.ToString();
    }

    #endregion
}
