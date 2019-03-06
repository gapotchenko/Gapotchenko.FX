using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Text
{
    /// <summary>
    /// Represents a string span.
    /// </summary>
    public struct StringSpan : IEquatable<StringSpan>
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

        public bool Equals(StringSpan other) =>
            StartIndex == other.StartIndex &&
            Length == other.Length;

        public override bool Equals(object obj)
        {
            if (obj is StringSpan ss)
                return Equals(ss);
            else
                return false;
        }

        public override int GetHashCode() => HashCode.Combine(StartIndex, Length);

        public static bool operator ==(StringSpan a, StringSpan b) => a.Equals(b);

        public static bool operator !=(StringSpan a, StringSpan b) => !a.Equals(b);

        /// <summary>
        /// Implicitly converts a regular expression capture to a string span.
        /// </summary>
        /// <param name="capture">The regular expression capture.</param>
        public static implicit operator StringSpan(Capture capture)
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));
            return new StringSpan(capture.Index, capture.Length);
        }
    }
}
