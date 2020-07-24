using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Gapotchenko.FX
{
    /// <summary>
    /// Represents an optional value.
    /// It is similar to <see cref="Nullable{T}"/>, but can also operate with a value of a reference type.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
    [Serializable]
    public struct Optional<T> : IEquatable<Optional<T>>, IComparable<Optional<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Optional{T}"/> structure to the specified value.
        /// </summary>
        /// <param name="value">A value.</param>
        public Optional(T value)
        {
            m_Value = value;
            m_HasValue = true;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal T m_Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool m_HasValue;

        /// <summary>
        /// Gets the value of the current <see cref="Optional{T}"/> if it has been assigned a valid underlying value.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="Optional{T}.HasValue"/> property is <c>false</c>.</exception>
        public T Value
        {
            get
            {
                if (!m_HasValue)
                    _ThrowMustHaveValueException();
                return m_Value;
            }
        }

        // A separate method is due to performance/inlining reasons.
        [DoesNotReturn]
        static void _ThrowMustHaveValueException() => throw new InvalidOperationException("Optional object must have a value.");

        /// <summary>
        /// Gets a value indicating whether the current <see cref="Optional{T}"/> has a valid value of its underlying type.
        /// </summary>
        public bool HasValue => m_HasValue;

        /// <summary>
        /// Retrieves the value of the current <see cref="Optional{T}"/> object, or the object's default value.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="Value"/> property if the <see cref="HasValue"/> property is <c>true</c>;
        /// otherwise, the default value of <typeparamref name="T"/> type.
        /// </returns>
        public T GetValueOrDefault() => m_Value;

        /// <summary>
        ///  Retrieves the value of the current <see cref="Optional{T}"/> object, or the specified default value.
        /// </summary>
        /// <param name="defaultValue">A value to return if the <see cref="HasValue"/> property is <c>false</c>.</param>
        /// <returns>
        /// The value of the <see cref="Value"/> property if the <see cref="HasValue"/> property is <c>true</c>;
        /// otherwise, the <paramref name="defaultValue"/> parameter.
        /// </returns>
        public T GetValueOrDefault(T defaultValue) => m_HasValue ? m_Value : defaultValue;

        /// <summary>
        /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns><c>true</c> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj) => OptionalEqualityComparer<T>.EqualsCore(this, obj, EqualityComparer<T>.Default);

        /// <summary>
        /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified optional value.
        /// </summary>
        /// <param name="other">An optional value.</param>
        /// <returns><c>true</c> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <c>false</c>.</returns>
        public bool Equals(Optional<T> other) => Optional.Equals(this, other, null);

        /// <summary>
        /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified value.
        /// </summary>
        /// <param name="other">A value.</param>
        /// <returns><c>true</c> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <c>false</c>.</returns>
        public bool Equals([AllowNull] T other) => OptionalEqualityComparer<T>.EqualsCore(this, other, EqualityComparer<T>.Default);

        /// <summary>
        /// Retrieves the hash code of the object returned by the <see cref="Value"/> property.
        /// </summary>
        /// <returns>
        /// The hash code of the object returned by the <see cref="Value"/> property if the <see cref="HasValue"/> property is <c>true</c>,
        /// or zero if the <see cref="HasValue"/> property is <c>false</c>.
        /// </returns>
        public override int GetHashCode() => Optional.GetHashCode(this, null);

        /// <summary>
        /// Returns the text representation of the value of the current <see cref="Optional{T}"/> object.
        /// </summary>
        /// <returns>
        /// The text representation of the value of the current <see cref="Optional{T}"/> object if the <see cref="HasValue"/> property is <c>true</c>,
        /// or an empty string ("") if the <see cref="HasValue"/> property is <c>false</c>.
        /// </returns>
        public override string? ToString() => m_HasValue ? m_Value?.ToString() : null;

        /// <summary>
        /// Creates a new <see cref="Optional{T}"/> object initialized to a specified value.
        /// </summary>
        /// <param name="value">A value.</param>
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);

        /// <summary>
        /// Returns the value of a specified <see cref="Optional{T}"/> value.
        /// </summary>
        /// <param name="value">A <see cref="Optional{T}"/> value.</param>
        public static explicit operator T(Optional<T> value) => value.Value;

        /// <summary>
        /// <para>
        /// Returns <see cref="Optional{T}"/> object initialized without a value.
        /// </para>
        /// <para>
        /// Use <seealso cref="Optional.Some{T}(T)"/> method to create a new <see cref="Optional{T}"/> object initialized to a specified value.
        /// </para>
        /// </summary>
        /// <returns>
        /// A <see cref="Optional{T}"/> object whose <see cref="HasValue"/> property is <c>false</c>.
        /// </returns>
        public static Optional<T> None => default;

        /// <summary>
        /// Compares the current <see cref="Optional{T}"/> object to a specified object
        /// </summary>
        /// <param name="other">An object.</param>
        /// <returns>The comparison result.</returns>
        int IComparable<Optional<T>>.CompareTo(Optional<T> other) => Optional.Compare(this, other, null);

        static bool _EqualsOp(Optional<T> value1, Optional<T> value2)
        {
            var a = _EmptifyNull(value1);
            var b = _EmptifyNull(value2);
            return a.Equals(b);
        }

        static Optional<T> _EmptifyNull(Optional<T> optional)
        {
            if (optional.HasValue && optional.Value is null)
                return None;
            else
                return optional;
        }

        /// <summary>
        /// Determines whether two specified <see cref="Optional{T}"/> objects are equal.
        /// </summary>
        /// <param name="optional1">The first <see cref="Optional{T}"/> object.</param>
        /// <param name="optional2">The second <see cref="Optional{T}"/> object.</param>
        /// <returns><c>true</c> if <paramref name="optional1"/> equals <paramref name="optional2"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Optional<T> optional1, Optional<T> optional2) => _EqualsOp(optional1, optional2);

        /// <summary>
        /// Determines whether two specified <see cref="Optional{T}"/> objects are not equal.
        /// </summary>
        /// <param name="optional1">The first <see cref="Optional{T}"/> object.</param>
        /// <param name="optional2">The second <see cref="Optional{T}"/> object.</param>
        /// <returns><c>true</c> if <paramref name="optional1"/> does not equal <paramref name="optional2"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Optional<T> optional1, Optional<T> optional2) => !_EqualsOp(optional1, optional2);

    }
}
