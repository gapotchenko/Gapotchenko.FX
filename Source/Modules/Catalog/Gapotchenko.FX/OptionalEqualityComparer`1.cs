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

    internal static bool EqualsCore(Optional<T> x, object? y, IEqualityComparer<T> valueComparer)
    {
        if (!x.HasValue)
        {
            if (y is Optional<T> otherOptional)
                return !otherOptional.HasValue;
            else
                return false;
        }
        else if (y is Optional<T> otherOptional)
        {
            return EqualsCore(otherOptional, x.Value, valueComparer);
        }
        else if (y is T otherValue)
        {
            return valueComparer.Equals(x.Value, otherValue);
        }
        else
        {
            var value = x.Value;
            if (value is null)
                return y is null;
            else
                return value.Equals(y);
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

    internal static int GetHashCodeCore(Optional<T> obj, IEqualityComparer<T> valueComparer)
    {
        if (!obj.HasValue)
        {
            return 0;
        }
        else if (obj.Value is not null and var value)
            return valueComparer.GetHashCode(value);
        else
            return 0;
    }
}
