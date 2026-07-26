// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class DefaultArrayComparer<T> : ArrayEqualityComparer<T>
    {
        public static DefaultArrayComparer<T> Instance { get; } = new();

        DefaultArrayComparer()
        {
        }

        public override bool Equals(T[]? x, T[]? y)
        {
            return EqualsCore(x, y);
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj);
        }

        public override bool Equals(object? obj) => obj is DefaultArrayComparer<T>;

        public override int GetHashCode() => HashCode.Combine(0xbba6c00f, typeof(T).GetHashCode());
    }
}
