// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Represents semantic version.
/// </summary>
[ImmutableObject(true)]
[TypeConverter(typeof(SemanticVersionConverter))]
[Serializable]
public sealed partial class SemanticVersion : IEquatable<SemanticVersion>, IComparable, IComparable<SemanticVersion>, ICloneable<SemanticVersion>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class using the specified string.
    /// </summary>
    /// <param name="version">The version string.</param>
    public SemanticVersion(string version)
    {
        if (version == null)
            throw new ArgumentNullException(nameof(version));
        if (version.Length == 0)
            throw new ArgumentException("Semantic version string cannot be empty.", nameof(version));

        if (!TryParseCore(version))
            throw new ArgumentException("Semantic version string has an invalid format.", nameof(version));
    }

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

    int m_Major;
    int m_Minor;
    int m_Patch;
    string? m_PreReleaseLabel;
    string? m_BuildLabel;

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

    /// <summary>
    /// Converts the value of the current <see cref="SemanticVersion"/> object to its equivalent <see cref="String"/> representation.
    /// </summary>
    /// <returns>The string representation of the current <see cref="SemanticVersion"/> object.</returns>
    public override string ToString() => m_CachedString ??= ToStringCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string? m_CachedString;

    string ToStringCore()
    {
        var builder = new StringBuilder().Append(m_Major).Append('.').Append(m_Minor).Append('.').Append(m_Patch);
        if (m_PreReleaseLabel is not null and var preReleaseLabel)
            builder.Append('-').Append(preReleaseLabel);
        if (m_BuildLabel is not null and var buildLabel)
            builder.Append('+').Append(buildLabel);
        return builder.ToString();
    }

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

    /// <summary>
    /// Compares the current <see cref="SemanticVersion"/> object to a specified object and returns an indication of their relative values.
    /// </summary>
    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;
        else if (obj is SemanticVersion semanticVersion)
            return CompareTo(semanticVersion);
        else
            throw new ArgumentException("Argument must be a semantic version.", nameof(obj));
    }

    /// <summary>
    /// Compares the current <see cref="SemanticVersion"/> object to a specified <see cref="SemanticVersion"/> object and returns an indication of their relative values.
    /// </summary>
    /// <param name="value">A <see cref="SemanticVersion"/> object to compare to the current <see cref="SemanticVersion"/> object, or null.</param>
    /// <returns>A signed integer that indicates the relative values of the two objects.</returns>
    public int CompareTo(SemanticVersion? value) =>
        ReferenceEquals(value, this) ? 0 :
        value is null ? 1 :
        m_Major != value.m_Major ? (m_Major > value.m_Major ? 1 : -1) :
        m_Minor != value.m_Minor ? (m_Minor > value.m_Minor ? 1 : -1) :
        m_Patch != value.m_Patch ? (m_Patch > value.m_Patch ? 1 : -1) :
        _ComparePreReleaseLabels(m_PreReleaseLabel, value.m_PreReleaseLabel);

    static int _ComparePreReleaseLabels(string? x, string? y)
    {
        // Symver 2.0 standard p.9
        // Pre-release versions have a lower precedence than the associated normal version.
        // Comparing each dot separated identifier from left to right
        // until a difference is found as follows:
        //     - identifiers consisting of only digits are compared numerically
        //     - identifiers with letters or hyphens are compared lexically in ASCII sort order.
        // Numeric identifiers always have lower precedence than non-numeric identifiers.
        // A larger set of pre-release fields has a higher precedence than a smaller set,
        // if all of the preceding identifiers are equal.

        if (x == null)
            return y == null ? 0 : 1;
        if (y == null)
            return -1;

        string[] xUnits = x.Split('.');
        string[] yUnits = y.Split('.');

        int minLength = Math.Min(xUnits.Length, yUnits.Length);

        for (int i = 0; i < minLength; i++)
        {
            string unitX = xUnits[i];
            string unitY = yUnits[i];

            bool isNumberX = int.TryParse(unitX, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int numberX);
            bool isNumberY = int.TryParse(unitY, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int numberY);

            if (isNumberX && isNumberY)
            {
                if (numberX != numberY)
                    return numberX < numberY ? -1 : 1;
            }
            else
            {
                if (isNumberX)
                    return -1;
                if (isNumberY)
                    return 1;

                int result = string.CompareOrdinal(unitX, unitY);
                if (result != 0)
                    return result;
            }
        }

        return xUnits.Length.CompareTo(yUnits.Length);
    }

    /// <summary>
    /// Returns a value indicating whether the current <see cref="SemanticVersion"/> object is equal to a specified object.
    /// </summary>
    /// <param name="obj">An object to compare with the current <see cref="SemanticVersion"/> object, or null.</param>
    /// <returns>
    /// <c>true</c> if the current <see cref="SemanticVersion"/> object and <paramref name="obj"/> are both <see cref="SemanticVersion"/> objects,
    /// and every significant component of the current <see cref="SemanticVersion"/> object matches the corresponding
    /// component of <paramref name="obj"/>; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as SemanticVersion);

    /// <summary>
    /// Returns a value indicating whether the current <see cref="SemanticVersion"/> object and a specified
    /// <see cref="SemanticVersion"/> object represent the same value.
    /// </summary>
    /// <param name="obj">A <see cref="SemanticVersion"/> object to compare to the current System.Version object, or null.</param>
    /// <returns>
    /// <c>true</c> if every significant component of the current <see cref="SemanticVersion"/> object matches the corresponding
    /// component of the <paramref name="obj"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(SemanticVersion? obj)
    {
        return
            ReferenceEquals(obj, this) ||
            obj is not null &&
            m_Major == obj.Major &&
            m_Minor == obj.m_Minor &&
            m_Patch == obj.m_Patch &&
            string.Equals(m_PreReleaseLabel, obj.m_PreReleaseLabel, StringComparison.Ordinal);

        // SymVer 2.0 standard requires to ignore 'BuildLabel' (Build metadata).
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(
            m_Major.GetHashCode(),
            m_Minor.GetHashCode(),
            m_Patch.GetHashCode(),
            m_PreReleaseLabel != null ? StringComparer.Ordinal.GetHashCode(m_PreReleaseLabel) : 0);

    /// <summary>
    /// Determines whether two specified <see cref="SemanticVersion"/> objects are equal.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> equals <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(SemanticVersion? v1, SemanticVersion? v2)
    {
        return
            ReferenceEquals(v1, v2) ||
            v1 is not null &&
            v2 is not null &&
            v1.Equals(v2);
    }

    /// <summary>
    /// Determines whether two specified <see cref="SemanticVersion"/> objects are not equal.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> does not equal <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(SemanticVersion? v1, SemanticVersion? v2) => !(v1 == v2);

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is less than
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is less than <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <(SemanticVersion v1, SemanticVersion v2)
    {
        if (ReferenceEquals(v1, null))
            throw new ArgumentNullException(nameof(v1));
        return v1.CompareTo(v2) < 0;
    }

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is less than or equal to
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is less than or equal to <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
    {
        if (ReferenceEquals(v1, null))
            throw new ArgumentNullException(nameof(v1));
        return v1.CompareTo(v2) <= 0;
    }

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is greater than
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is greater than <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >(SemanticVersion v1, SemanticVersion v2) => v2 < v1;

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is greater than or equal to
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is greater than or equal to <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >=(SemanticVersion v1, SemanticVersion v2) => v2 <= v1;
}
