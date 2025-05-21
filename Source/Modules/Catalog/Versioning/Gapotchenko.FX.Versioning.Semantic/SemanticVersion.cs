// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Represents a semantic version.
/// </summary>
[Serializable]
[ImmutableObject(true)]
public sealed partial record SemanticVersion
{
    // This type is partial.
    // Please take a look at the neighboring source files for the rest of the implementation.

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
    /// Gets or initializes the value of the prerelease component of the version label for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The non-empty prerelease string, or <see langword="null"/> if there is no prerelease string.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? Prerelease
    {
        get => m_Prerelease;
        init
        {
            ValidateLabelComponent(value);
            m_Prerelease = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_Prerelease;

    /// <summary>
    /// Gets or initializes the value of the build metadata component of the version label for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <value>The non-empty build metadata string, or <see langword="null"/> if there is no build metadata.</value>
    /// <exception cref="ArgumentException"><paramref name="value"/> has an invalid format.</exception>
    public string? Build
    {
        get => m_Build;
        init
        {
            ValidateLabelComponent(value);
            m_Build = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_Build;
}
