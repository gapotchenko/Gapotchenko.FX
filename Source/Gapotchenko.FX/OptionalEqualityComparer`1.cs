using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Gapotchenko.FX
{
    [Serializable]
    sealed class OptionalEqualityComparer<T> : IEqualityComparer<Optional<T>>
    {
        public OptionalEqualityComparer(IEqualityComparer<T>? valueComparer)
        {
            m_ValueComparer = valueComparer ?? EqualityComparer<T>.Default;
        }

        readonly IEqualityComparer<T> m_ValueComparer;

        public bool Equals(Optional<T> x, Optional<T> y) => EqualsCore(x, y, m_ValueComparer);

        public int GetHashCode(Optional<T> obj) => GetHashCodeCore(obj, m_ValueComparer);

        internal static bool EqualsCore(Optional<T> x, object? y, IEqualityComparer<T> valueComparer)
        {
            if (!x.HasValue)
            {
                if (y is Optional<T> otherOption)
                    return !otherOption.HasValue;
                else
                    return false;
            }
            else if (y is Optional<T> otherOption)
            {
                if (otherOption.HasValue)
                    return valueComparer.Equals(x.Value, otherOption.Value);
                else
                    return false;
            }
            else if (y is T otherValue)
            {
                return valueComparer.Equals(x.Value, otherValue);
            }
            else
            {
                var value = x.Value;
                if (value == null)
                    return y == null;
                return value.Equals(y);
            }
        }

        internal static bool EqualsCore(Optional<T> x, [AllowNull] T y, IEqualityComparer<T> valueComparer)
        {
            if (!x.HasValue)
                return false;

#if NETSTANDARD2_1
#pragma warning disable CS8604 // Possible null reference argument.
#endif
            return valueComparer.Equals(x.Value, y);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        internal static bool EqualsCore(Optional<T> x, Optional<T> y, IEqualityComparer<T> valueComparer)
        {
            if (x.HasValue)
            {
                if (y.HasValue)
                    return valueComparer.Equals(x.Value, y.Value);
                return false;
            }
            else
            {
                return !y.HasValue;
            }
        }

        internal static int GetHashCodeCore(Optional<T> obj, IEqualityComparer<T> valueComparer)
        {
            if (!obj.HasValue)
            {
                return 0;
            }
            else
            {
                var value = obj.Value;
                if (value is null)
                    return 0;
                else
                    return valueComparer.GetHashCode(value);
            }
        }
    }
}
