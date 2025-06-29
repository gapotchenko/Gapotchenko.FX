// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX;

/// <summary>
/// <para>
/// Represents an optional value.
/// </para>
/// <para>
/// <see cref="Optional{T}"/> is similar to <see cref="Nullable{T}"/> but can also operate on reference types.
/// </para>
/// </summary>
/// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> generic type.</typeparam>
[Serializable]
public struct Optional<T> : IOptional, IEquatable<Optional<T>>, IComparable<Optional<T>>
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly object? IOptional.Value => Value;

    /// <summary>
    /// Gets the value of the current <see cref="Optional{T}"/> if it has been assigned a valid underlying value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="HasValue"/> property is <see langword="false"/>.</exception>
    public readonly T Value
    {
        get
        {
            if (!m_HasValue)
                ThrowMustHaveValueException();
            return m_Value;
        }
    }

    // A separate method is due to performance/inlining reasons.
    [DoesNotReturn, StackTraceHidden]
    static void ThrowMustHaveValueException() => throw new InvalidOperationException(Resources.OptionalMustHaveValue);

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Optional{T}"/> has a valid value of its underlying type.
    /// </summary>
    public readonly bool HasValue => m_HasValue;

    /// <summary>
    /// Retrieves the value of the current <see cref="Optional{T}"/> object, or the object's <see langword="default"/> value.
    /// </summary>
    /// <returns>
    /// The value of the <see cref="Value"/> property if the <see cref="HasValue"/> property is <see langword="true"/>;
    /// otherwise, the <see langword="default"/> value of <typeparamref name="T"/> type.
    /// </returns>
    public readonly T GetValueOrDefault() => m_Value;

    /// <summary>
    /// Retrieves the value of the current <see cref="Optional{T}"/> object, or the specified default value.
    /// </summary>
    /// <param name="defaultValue">A value to return if the <see cref="HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>
    /// The value of the <see cref="Value"/> property if the <see cref="HasValue"/> property is <see langword="true"/>;
    /// otherwise, the <paramref name="defaultValue"/> parameter.
    /// </returns>
    public readonly T GetValueOrDefault(T defaultValue) => m_HasValue ? m_Value : defaultValue;

    /// <summary>
    /// Returns a new <see cref="Optional{T}"/> instance whose underlying <typeparamref name="T"/> value is casted to the specified <typeparamref name="TResult"/> type.
    /// </summary>
    /// <typeparam name="TResult">The type to cast the underlying value to.</typeparam>
    /// <returns>
    /// The new <see cref="Optional{T}"/> instance whose underlying <typeparamref name="T"/> value is casted to <typeparamref name="TResult"/> type.
    /// </returns>
    /// <exception cref="InvalidCastException"><typeparamref name="T"/> value cannot be casted to <typeparamref name="TResult"/>.</exception>
    public readonly Optional<TResult> Cast<TResult>() =>
        m_HasValue ? new((TResult)(object)m_Value!) : default;

    /// <summary>
    /// Returns the text representation of the value of the current <see cref="Optional{T}"/> object.
    /// </summary>
    /// <returns>
    /// The text representation of the value of the current <see cref="Optional{T}"/> object if the <see cref="HasValue"/> property is <see langword="true"/>,
    /// or a <see langword="null"/> if the <see cref="HasValue"/> property is <see langword="false"/>.
    /// </returns>
    public override readonly string? ToString() => m_HasValue ? m_Value?.ToString() : null;

    /// <summary>
    /// Creates a new <see cref="Optional{T}"/> object initialized to a specified value.
    /// </summary>
    /// <param name="value">A value.</param>
    public static implicit operator Optional<T>(T value) =>
        value switch
        {
            IOptional optional when typeof(T) == typeof(object) => optional.HasValue ? new((T)optional.Value!) : default,
            _ => new(value)
        };

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
    /// A <see cref="Optional{T}"/> object whose <see cref="HasValue"/> property is <see langword="false"/>.
    /// </returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static Optional<T> None => default;
#pragma warning restore CA1000

    #region Equality

    /// <summary>
    /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified object.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns><see langword="true"/> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <see langword="false"/>.</returns>
    public override readonly bool Equals(object? obj) => OptionalEqualityComparer<T>.EqualsCore(this, obj, EqualityComparer<T>.Default);

    /// <summary>
    /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified optional value.
    /// </summary>
    /// <param name="other">An optional value.</param>
    /// <returns><see langword="true"/> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(Optional<T> other) => Optional.Equals(this, other, null);

    /// <inheritdoc cref="Equals(object?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool Equals<TOther>(TOther? obj) where TOther : struct, IOptional =>
        OptionalEqualityComparer<T>.EqualsCore(this, obj, EqualityComparer<T>.Default);

    /// <summary>
    /// Indicates whether the current <see cref="Optional{T}"/> object is equal to a specified value.
    /// </summary>
    /// <param name="other">A value.</param>
    /// <returns><see langword="true"/> if the other parameter is equal to the current <see cref="Optional{T}"/> object; otherwise, <see langword="false"/>.</returns>
    public readonly bool Equals(T? other) => OptionalEqualityComparer<T>.EqualsCore(this, other, EqualityComparer<T>.Default);

    /// <summary>
    /// Retrieves the hash code of the object returned by the <see cref="Value"/> property.
    /// </summary>
    /// <returns>
    /// The hash code of the object returned by the <see cref="Value"/> property if the <see cref="HasValue"/> property is <see langword="true"/>,
    /// or zero if the <see cref="HasValue"/> property is <see langword="false"/>.
    /// </returns>
    public override readonly int GetHashCode() => Optional.GetHashCode(this, null);

    /// <summary>
    /// Determines whether two specified <see cref="Optional{T}"/> objects are equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        var a = EmptifyNull(left);
        var b = EmptifyNull(right);
        return a.Equals(b);

        static Optional<T> EmptifyNull(in Optional<T> optional)
        {
            if (optional.HasValue && optional.Value is null)
                return None;
            else
                return optional;
        }
    }

    /// <summary>
    /// Determines whether two specified <see cref="Optional{T}"/> objects are not equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Optional<T> left, Optional<T> right) => !(left == right);

    /// <summary>
    /// Determines whether two specified objects are equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Optional<T> left, T? right) =>
        !left.HasValue && right is null ||
        left.Equals(right);

    /// <summary>
    /// Determines whether two specified objects are not equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <typeparamref name="T"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Optional<T> left, T? right) => !(left == right);

    /// <summary>
    /// Determines whether two specified optional objects are equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <see cref="IOptional"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Optional<T> left, IOptional? right) =>
        OptionalEqualityComparer<T>.EqualsCore(left, right, EqualityComparer<T>.Default);

    /// <summary>
    /// Determines whether two specified optional objects are not equal.
    /// </summary>
    /// <param name="left">The left <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The right <see cref="IOptional"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Optional<T> left, IOptional? right) => !(left == right);

    #endregion

    #region Comparability

    /// <summary>
    /// Compares the current <see cref="Optional{T}"/> object to a specified object.
    /// </summary>
    /// <param name="other">An object to compare to.</param>
    /// <returns>The comparison result.</returns>
    readonly int IComparable<Optional<T>>.CompareTo(Optional<T> other) => CompareTo(other);

    /// <summary>
    /// Determines whether the first specified <see cref="Optional{T}"/> object
    /// is greater than
    /// the second specified <see cref="Optional{T}"/> object.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/>
    /// is greater than
    /// <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(Optional<T> left, Optional<T> right) => left.HasValue && right.HasValue && left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether the first specified <see cref="Optional{T}"/> object
    /// is less than
    /// the second specified <see cref="Optional{T}"/> object.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/>
    /// is less than
    /// <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(Optional<T> left, Optional<T> right) => left.HasValue && right.HasValue && left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether the first specified <see cref="Optional{T}"/> object
    /// is greater than or equal to
    /// the second specified <see cref="Optional{T}"/> object.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/>
    /// is greater than or equal to
    /// <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(Optional<T> left, Optional<T> right) => left.HasValue && right.HasValue && left.CompareTo(right) >= 0;

    /// <summary>
    /// Determines whether the first specified <see cref="Optional{T}"/> object
    /// is less than or equal to
    /// the second specified <see cref="Optional{T}"/> object.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> object.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/>
    /// is less than or equal to
    /// <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(Optional<T> left, Optional<T> right) => left.HasValue && right.HasValue && left.CompareTo(right) <= 0;

    readonly int CompareTo(Optional<T> other) => Optional.Compare(this, other, null);

    #endregion
}
