// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Represents semantic version.
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
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Major component of semantic version cannot be negative.");
            m_Major = value;
        }
    }

    /// <summary>
    /// Gets or initializes the value of the minor component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The minor version number.</value>
    public int Minor
    {
        get => m_Minor;
        init
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Minor component of semantic version cannot be negative.");
            m_Minor = value;
        }
    }

    /// <summary>
    /// Gets or initializes the value of the patch component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The patch number.</value>
    public int Patch
    {
        get => m_Patch;
        init
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Patch component of semantic version cannot be negative.");
            m_Patch = value;
        }
    }

    /// <summary>
    /// Gets or initializes the value of the pre-release label of the version string for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The pre-release label.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? PreReleaseLabel
    {
        get => m_PreReleaseLabel;
        init => m_PreReleaseLabel = VerifyLabel(value);
    }

    /// <summary>
    /// Gets or initializes the value of the build label of the version string for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The build label.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? BuildLabel
    {
        get => m_BuildLabel;
        init => m_BuildLabel = VerifyLabel(value);
    }

    int m_Major;
    int m_Minor;
    int m_Patch;
    string? m_PreReleaseLabel;
    string? m_BuildLabel;

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

        if (m_PreReleaseLabel is not null and var preReleaseLabel)
            builder.Append('-').Append(preReleaseLabel);

        if (m_BuildLabel is not null and var buildLabel)
            builder.Append('+').Append(buildLabel);

        return builder.ToString();
    }

    #endregion
}
