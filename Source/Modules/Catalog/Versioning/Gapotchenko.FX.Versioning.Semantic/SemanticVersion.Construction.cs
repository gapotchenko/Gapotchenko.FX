// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major, minor, patch, prerelease, and build values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch number.</param>
    /// <param name="prerelease">The prerelease string.</param>
    /// <param name="build">The build metadata.</param>
    /// <exception cref="ArgumentException"><paramref name="prerelease"/> has an invalid format.</exception>
    /// <exception cref="ArgumentException"><paramref name="build"/> has an invalid format.</exception>
    public SemanticVersion(int major, int minor, int patch, string? prerelease, string? build) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(prerelease))
        {
            ValidateLabelComponent(prerelease);
            m_Prerelease = prerelease;
        }

        if (!string.IsNullOrEmpty(build))
        {
            ValidateLabelComponent(build);
            m_Build = build;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified major, minor, patch, and label values.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch version number.</param>
    /// <param name="label">The version label.</param>
    /// <exception cref="ArgumentException"><paramref name="label"/> has an invalid format.</exception>
    public SemanticVersion(int major, int minor, int patch, string? label) :
        this(major, minor, patch)
    {
        if (!string.IsNullOrEmpty(label))
        {
            if (!Parser.TryParseLabel(label, out m_Prerelease, out m_Build))
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
        ArgumentOutOfRangeException.ThrowIfNegative(major);
        ArgumentOutOfRangeException.ThrowIfNegative(minor);
        ArgumentOutOfRangeException.ThrowIfNegative(patch);

        m_Major = major;
        m_Minor = minor;
        m_Patch = patch;
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

    /// <summary>
    /// Copy constructor. Initializes a new record clone.
    /// </summary>
    /// <param name="semanticVersion">The record to clone.</param>
    SemanticVersion(SemanticVersion semanticVersion)
    {
        m_Major = semanticVersion.m_Major;
        m_Minor = semanticVersion.m_Minor;
        m_Patch = semanticVersion.m_Patch;
        m_Prerelease = semanticVersion.m_Prerelease;
        m_Build = semanticVersion.m_Build;
    }

    static void ValidateLabelComponent(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (!Parser.IsValidLabelComponent(value))
            throw new ArgumentException("The value has an invalid format.", paramName);
    }
}
