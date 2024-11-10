using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Text;

/// <summary>
/// Represents a string span.
/// </summary>
public readonly struct StringSpan : IEquatable<StringSpan>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringSpan"/> struct.
    /// </summary>
    /// <param name="startIndex">The zero-based start index.</param>
    /// <param name="length">The length.</param>
    public StringSpan(int startIndex, int length)
    {
        StartIndex = startIndex;
        Length = length;
    }

    /// <summary>
    /// Gets the zero-based start index.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Indicates whether the current <see cref="StringSpan"/> struct is equal to another <see cref="StringSpan"/>.
    /// </summary>
    /// <param name="other">An <see cref="StringSpan"/> value to compare with this struct.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current <see cref="StringSpan"/>; otherwise, <see langword="false"/>.</returns>
    public bool Equals(StringSpan other) =>
        StartIndex == other.StartIndex &&
        Length == other.Length;

    /// <summary>
    /// Indicates whether the current <see cref="StringSpan"/> struct is equal to another object.
    /// </summary>
    /// <param name="obj">An object to compare with this <see cref="StringSpan"/>.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current <see cref="StringSpan"/>; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj) => obj is StringSpan other && Equals(other);

    /// <summary>
    /// Gets a hash code for the current <see cref="StringSpan"/> struct.
    /// </summary>
    /// <returns>A hash code for the current <see cref="StringSpan"/> struct.</returns>
    public override int GetHashCode() => HashCode.Combine(StartIndex, Length);

    /// <summary>
    /// Indicates whether two <see cref="StringSpan"/> structs are equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns><see langword="true"/> if the specified <see cref="StringSpan"/> structs are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(StringSpan a, StringSpan b) => a.Equals(b);

    /// <summary>
    /// Indicates whether two <see cref="StringSpan"/> structs are not equal.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns><see langword="true"/> if the specified <see cref="StringSpan"/> structs are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(StringSpan a, StringSpan b) => !a.Equals(b);

    /// <summary>
    /// Implicitly converts a regular expression capture to a string span.
    /// </summary>
    /// <param name="capture">The regular expression capture.</param>
    public static implicit operator StringSpan(Capture capture)
    {
        if (capture == null)
            return new(0, 0);
        else
            return new(capture.Index, capture.Length);
    }
}
