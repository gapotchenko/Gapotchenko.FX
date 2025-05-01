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
public sealed partial class SemanticVersion : ICloneable<SemanticVersion>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified major, minor, patch, pre-release label, and build label values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    /// <param name="preReleaseLabel">The pre-release label.</param>
    /// <param name="buildLabel">The build label.</param>
    public SemanticVersion(int major, int minor, int patch, string? preReleaseLabel, string? buildLabel) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(preReleaseLabel))
        {
            if (!Parser.IsValidLabel(preReleaseLabel))
                throw new ArgumentException("Pre-release label for semantic version has an invalid format.", nameof(preReleaseLabel));
            m_PreReleaseLabel = preReleaseLabel;
        }

        if (!string.IsNullOrEmpty(buildLabel))
        {
            if (!Parser.IsValidLabel(buildLabel))
                throw new ArgumentException("Build label for semantic version has an invalid format.", nameof(buildLabel));
            m_BuildLabel = buildLabel;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified major, minor, patch, and label values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    /// <param name="label">The labels.</param>
    public SemanticVersion(int major, int minor, int patch, string? label) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(label))
        {
            if (!Parser.TryParseLabels(label, out m_PreReleaseLabel, out m_BuildLabel))
                throw new ArgumentException("Label for semantic version has an invalid format.", nameof(label));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified major, minor, and patch values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    public SemanticVersion(int major, int minor, int patch)
    {
        if (major < 0)
            throw new ArgumentOutOfRangeException(nameof(major), "Major component of semantic version cannot be negative.");
        if (minor < 0)
            throw new ArgumentOutOfRangeException(nameof(minor), "Minor component of semantic version cannot be negative.");
        if (patch < 0)
            throw new ArgumentOutOfRangeException(nameof(patch), "Patch component of semantic version cannot be negative.");

        m_Major = major;
        m_Minor = minor;
        m_Patch = patch;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified major and minor values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    public SemanticVersion(int major, int minor) :
        this(major, minor, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified major value.
    /// </summary>
    /// <param name="major">The major version number.</param>
    public SemanticVersion(int major) :
        this(major, 0)
    {
    }

    SemanticVersion()
    {
    }

    /// <summary>
    /// Gets the value of the major component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>The major version number.</returns>
    public int Major => m_Major;

    /// <summary>
    /// Gets the value of the minor component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>The minor version number.</returns>
    public int Minor => m_Minor;

    /// <summary>
    /// Gets the value of the patch component of the version number for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>The patch number.</returns>
    public int Patch => m_Patch;

    /// <summary>
    /// Gets the value of the pre-release label of the version string for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    public string? PreReleaseLabel => m_PreReleaseLabel;

    /// <summary>
    /// Gets the value of the build label of the version string for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    public string? BuildLabel => m_BuildLabel;

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

    /// <summary>
    /// Returns a new <see cref="SemanticVersion"/> object whose value is the same as the current object.
    /// </summary>
    /// <returns>A new <see cref="SemanticVersion"/> whose values are a copy of the current object.</returns>
    public SemanticVersion Clone() =>
        new()
        {
            m_Major = m_Major,
            m_Minor = m_Minor,
            m_Patch = m_Patch,
            m_PreReleaseLabel = m_PreReleaseLabel,
            m_BuildLabel = m_BuildLabel,
            m_CachedString = m_CachedString
        };

    object ICloneable.Clone() => Clone();
}
