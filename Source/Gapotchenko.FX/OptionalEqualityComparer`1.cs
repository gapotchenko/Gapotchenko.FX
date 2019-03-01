using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    [Serializable]
    sealed class OptionalEqualityComparer<T> : IEqualityComparer<Optional<T>>
    {
        public OptionalEqualityComparer(IEqualityComparer<T> valueEqualityComparer)
        {
            _ValueEqualityComparer = valueEqualityComparer ?? EqualityComparer<T>.Default;
        }

        readonly IEqualityComparer<T> _ValueEqualityComparer;

        public bool Equals(Optional<T> x, Optional<T> y) => EqualsCore(x, y, _ValueEqualityComparer);

        public int GetHashCode(Optional<T> obj) => GetHashCodeCore(obj, _ValueEqualityComparer);

        internal static bool EqualsCore(Optional<T> x, object y, IEqualityComparer<T> valueEqualityComparer)
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
                    return valueEqualityComparer.Equals(x.Value, otherOption.Value);
                else
                    return false;
            }
            else if (y is T anotherValue)
            {
                return valueEqualityComparer.Equals(x.Value, anotherValue);
            }
            else
            {
                var value = x.Value;
                if (value == null)
                    return y == null;
                return value.Equals(y);
            }
        }

        internal static bool EqualsCore(Optional<T> x, T y, IEqualityComparer<T> valueEqualityComparer)
        {
            if (!x.HasValue)
                return false;
            return valueEqualityComparer.Equals(x.Value, y);
        }

        internal static bool EqualsCore(Optional<T> x, Optional<T> y, IEqualityComparer<T> valueEqualityComparer)
        {
            if (x.HasValue)
            {
                if (y.HasValue)
                    return valueEqualityComparer.Equals(x.Value, y.Value);
                return false;
            }
            else
            {
                return !y.HasValue;
            }
        }

        internal static int GetHashCodeCore(Optional<T> obj, IEqualityComparer<T> valueEqualityComparer)
        {
            if (!obj.HasValue)
            {
                return 0;
            }
            else
            {
                var value = obj.Value;
                if (value == null)
                    return 0;
                else
                    return valueEqualityComparer.GetHashCode(value);
            }
        }
    }
}
