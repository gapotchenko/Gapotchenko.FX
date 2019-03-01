using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Represents a pair of values.
    /// </summary>
    /// <typeparam name="TFirst">The type of the first value.</typeparam>
    /// <typeparam name="TSecond">The type of the second value.</typeparam>
    public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>, IComparable<Pair<TFirst, TSecond>>
    {
        /// <summary>
        /// Initializes a new pair of values.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// The first value.
        /// </summary>
        public TFirst First
        {
            get;
        }

        /// <summary>
        /// The second value.
        /// </summary>
        public TSecond Second
        {
            get;
        }

        /// <summary>
        /// Returns a hash code for <see cref="Pair{TFirst, TSecond}"/>.
        /// </summary>
        /// <returns>A hash code for <see cref="Pair{TFirst, TSecond}"/>.</returns>
        public override int GetHashCode() => HashCode.Combine(First, Second);

        /// <summary>
        /// Indicates whether the current <see cref="Pair{TFirst, TSecond}"/> object is equal to a specified value.
        /// </summary>
        /// <param name="other">A value.</param>
        /// <returns><c>true</c> if the other parameter is equal to the current <see cref="Pair{TFirst, TSecond}"/> object; otherwise, <c>false</c>.</returns>
        public bool Equals(Pair<TFirst, TSecond> other) =>
            EqualityComparer<TFirst>.Default.Equals(First, other.First) &&
            EqualityComparer<TSecond>.Default.Equals(Second, other.Second);

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Pair{TFirst, TSecond}"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="Pair{TFirst, TSecond}"/>.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to the current <see cref="Pair{TFirst, TSecond}"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair<TFirst, TSecond> other)
                return Equals(other);
            else
                return false;
        }

        int IComparable<Pair<TFirst, TSecond>>.CompareTo(Pair<TFirst, TSecond> other) =>
            Empty.Nullify(Comparer<TFirst>.Default.Compare(First, other.First)) ??
            Comparer<TSecond>.Default.Compare(Second, other.Second);

        /// <summary>
        /// Determines whether two specified <see cref="Pair{TFirst, TSecond}"/> objects are equal.
        /// </summary>
        /// <param name="pair1">The first <see cref="Pair{TFirst, TSecond}"/> object.</param>
        /// <param name="pair2">The second <see cref="Pair{TFirst, TSecond}"/> object.</param>
        /// <returns><c>true</c> if <paramref name="pair1"/> equals <paramref name="pair2"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Pair<TFirst, TSecond> pair1, Pair<TFirst, TSecond> pair2) => pair1.Equals(pair2);

        /// <summary>
        /// Determines whether two specified <see cref="Pair{TFirst, TSecond}"/> objects are not equal.
        /// </summary>
        /// <param name="pair1">The first <see cref="Pair{TFirst, TSecond}"/> object.</param>
        /// <param name="pair2">The second <see cref="Pair{TFirst, TSecond}"/> object.</param>
        /// <returns><c>true</c> if <paramref name="pair1"/> does not equal <paramref name="pair2"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Pair<TFirst, TSecond> pair1, Pair<TFirst, TSecond> pair2) => !pair1.Equals(pair2);

        /// <summary>
        /// Deconstructs the pair of values.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TFirst first, out TSecond second)
        {
            first = First;
            second = Second;
        }
    }
}
