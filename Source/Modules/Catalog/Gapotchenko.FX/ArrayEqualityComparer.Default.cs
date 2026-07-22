using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class DefaultArrayComparer<T>(IEqualityComparer<T>? elementComparer) : ArrayEqualityComparer<T>
    {
        public override bool Equals(T[]? x, T[]? y)
        {
            if (x == y)
                return true;
            if (x is null || y is null)
                return false;

            return x.SequenceEqual(y, elementComparer);
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj, elementComparer);
        }

        public override bool Equals(object? obj) => obj is ArrayEqualityComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
