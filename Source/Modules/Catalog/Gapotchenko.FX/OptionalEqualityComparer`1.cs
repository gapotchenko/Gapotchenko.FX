// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX;

sealed class OptionalEqualityComparer<T>(IEqualityComparer<T>? valueComparer) : IEqualityComparer<Optional<T>>
{
    public bool Equals(Optional<T> x, Optional<T> y) => EqualsCore(x, y, m_ValueComparer);

    public int GetHashCode(Optional<T> obj) => GetHashCodeCore(obj, m_ValueComparer);

    readonly IEqualityComparer<T> m_ValueComparer = valueComparer ?? EqualityComparer<T>.Default;

    internal static bool EqualsCore(Optional<T> x, object? y, IEqualityComparer<T> valueComparer) =>
        x.HasValue
            ? y switch
            {
                Optional<T> other => EqualsCore(other, x.Value, valueComparer),
                IOptional other => EqualsCore(other, x.Value, valueComparer),
                _ => ValueEqualsCore(x.Value, y, valueComparer)
            }
            : y switch
            {
                Optional<T> other => !other.HasValue,
                IOptional other => !other.HasValue && HasCompatibleType(other),
                _ => false
            };

    static bool HasCompatibleType(IOptional optional)
    {
        var thisType = typeof(T);
        var objectType = typeof(object);
        if (thisType == objectType)
        {
            return true;
        }
        else
        {
            var otherType = Optional.GetUnderlyingType(optional.GetType());
            return otherType == thisType || otherType == objectType;
        }
    }

    internal static bool EqualsCore(Optional<T> x, Optional<T> y, IEqualityComparer<T> valueComparer)
    {
        if (x.HasValue)
            return EqualsCore(y, x.Value, valueComparer);
        else
            return !y.HasValue;
    }

    internal static bool EqualsCore(Optional<T> x, T? y, IEqualityComparer<T> valueComparer)
    {
        if (x.HasValue)
            return valueComparer.Equals(x.Value, y);
        else
            return false;
    }

    static bool EqualsCore(IOptional x, T y, IEqualityComparer<T> valueComparer)
    {
        if (x.HasValue)
            return ValueEqualsCore(y, x.Value, valueComparer);
        else
            return false;
    }

    static bool ValueEqualsCore(T x, object? y, IEqualityComparer<T> valueComparer)
    {
        if (y is T otherValue)
            return valueComparer.Equals(x, otherValue);
        else if (x is null)
            return y is null;
        else
            return x.Equals(y);
    }

    internal static int GetHashCodeCore(Optional<T> obj, IEqualityComparer<T> valueComparer)
    {
        if (!obj.HasValue)
            return 0;
        else if (obj.Value is not null and var value)
            return valueComparer.GetHashCode(value);
        else
            return 0;
    }
}
