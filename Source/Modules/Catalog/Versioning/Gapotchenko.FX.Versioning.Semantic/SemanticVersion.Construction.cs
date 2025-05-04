// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major value.
    /// </summary>
    /// <param name="major">The major version number.</param>
    public SemanticVersion(int major) :
        this(major, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major and minor values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    public SemanticVersion(int major, int minor) :
        this(major, minor, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major, minor, patch, and label values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    /// <param name="label">The labels.</param>
    /// <exception cref="ArgumentException"><paramref name="label"/> has an invalid format.</exception>
    public SemanticVersion(int major, int minor, int patch, string? label) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(label))
        {
            if (!Parser.TryParseLabels(label, out m_PreReleaseLabel, out m_BuildLabel))
                throw new ArgumentException("The value has an invalid format.", nameof(label));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major, minor, and patch values.
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
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major, minor, patch, pre-release label, and build label values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    /// <param name="preReleaseLabel">The pre-release label.</param>
    /// <param name="buildLabel">The build label.</param>
    /// <exception cref="ArgumentException"><paramref name="preReleaseLabel"/> has an invalid format.</exception>
    /// <exception cref="ArgumentException"><paramref name="buildLabel"/> has an invalid format.</exception>
    public SemanticVersion(int major, int minor, int patch, string? preReleaseLabel, string? buildLabel) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(preReleaseLabel))
            m_PreReleaseLabel = VerifyLabel(preReleaseLabel);

        if (!string.IsNullOrEmpty(buildLabel))
            m_BuildLabel = VerifyLabel(buildLabel);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified version string.
    /// </summary>
    /// <param name="version">The version string.</param>
    /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="version"/> is empty.</exception>
    /// <exception cref="ArgumentException"><paramref name="version"/> has an invalid format.</exception>
    public SemanticVersion(string version) :
        this(Model.Create(version))
    {
    }

    [StackTraceHidden]
    [return: NotNullIfNotNull(nameof(value))]
    static string? VerifyLabel(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (!Parser.IsValidLabel(value))
            throw new ArgumentException("The value has an invalid format.", paramName);
        return value;
    }
}
