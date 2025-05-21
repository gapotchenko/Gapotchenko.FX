// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Globalization;

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion : IComparable, IComparable<SemanticVersion>
{
    /// <summary>
    /// Compares the current <see cref="SemanticVersion"/> object to a specified object and returns an indication of their relative values.
    /// </summary>
    /// <inheritdoc/>
    public int CompareTo(object? obj) =>
        obj switch
        {
            null => 1,
            SemanticVersion semanticVersion => CompareTo(semanticVersion),
            _ => throw new ArgumentException("Argument must be a semantic version.", nameof(obj))
        };

    /// <summary>
    /// Compares the current <see cref="SemanticVersion"/> object to a specified <see cref="SemanticVersion"/> object and returns an indication of their relative values.
    /// </summary>
    /// <param name="obj">A <see cref="SemanticVersion"/> object to compare to the current <see cref="SemanticVersion"/> object, or null.</param>
    /// <returns>A signed integer that indicates the relative values of the two objects.</returns>
    public int CompareTo(SemanticVersion? obj) =>
        ReferenceEquals(obj, this) ? 0 :
        obj is null ? 1 :
        m_Major != obj.m_Major ? (m_Major > obj.m_Major ? 1 : -1) :
        m_Minor != obj.m_Minor ? (m_Minor > obj.m_Minor ? 1 : -1) :
        m_Patch != obj.m_Patch ? (m_Patch > obj.m_Patch ? 1 : -1) :
        ComparePreReleaseLabels(m_Prerelease, obj.m_Prerelease);

    static int ComparePreReleaseLabels(string? x, string? y)
    {
        // SemVer 2.0 standard p.9
        // Pre-release versions have a lower precedence than the associated normal version.
        // Comparing each dot separated identifier from left to right
        // until a difference is found as follows:
        //     - identifiers consisting of only digits are compared numerically
        //     - identifiers with letters or hyphens are compared lexically in ASCII sort order.
        // Numeric identifiers always have lower precedence than non-numeric identifiers.
        // A larger set of pre-release fields has a higher precedence than a smaller set,
        // if all of the preceding identifiers are equal.

        if (x is null)
            return y is null ? 0 : 1;
        if (y is null)
            return -1;

        string[] xUnits = x.Split('.');
        string[] yUnits = y.Split('.');

        int minLength = Math.Min(xUnits.Length, yUnits.Length);
        var formatProvider = NumberFormatInfo.InvariantInfo;

        for (int i = 0; i < minLength; i++)
        {
            string unitX = xUnits[i];
            string unitY = yUnits[i];

            bool isNumberX = int.TryParse(unitX, NumberStyles.None, formatProvider, out int numberX);
            bool isNumberY = int.TryParse(unitY, NumberStyles.None, formatProvider, out int numberY);

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
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is less than
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is less than <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <(SemanticVersion? v1, SemanticVersion? v2)
    {
        if (v1 is null)
            return v2 is not null;
        else
            return v1.CompareTo(v2) < 0;
    }

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is less than or equal to
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is less than or equal to <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <=(SemanticVersion? v1, SemanticVersion? v2)
    {
        if (v1 is null)
            return true;
        else
            return v1.CompareTo(v2) <= 0;
    }

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is greater than
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is greater than <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >(SemanticVersion? v1, SemanticVersion? v2) => v2 < v1;

    /// <summary>
    /// Determines whether the first specified <see cref="SemanticVersion"/> object is greater than or equal to
    /// the second specified <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="v1">The first <see cref="SemanticVersion"/> object.</param>
    /// <param name="v2">The second <see cref="SemanticVersion"/> object.</param>
    /// <returns><c>true</c> if <paramref name="v1"/> is greater than or equal to <paramref name="v2"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >=(SemanticVersion? v1, SemanticVersion? v2) => v2 <= v1;
}
