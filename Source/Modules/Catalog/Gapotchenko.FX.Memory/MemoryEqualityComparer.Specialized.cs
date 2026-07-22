// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Runtime.InteropServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class ByteMemoryComparer : DefaultMemoryComparerBase<byte>
    {
        public static ByteMemoryComparer Instance = new();

        ByteMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            return HashOperations.GetHashCode(obj.Span);
        }

        public override bool Equals(object? obj) => obj is ByteMemoryComparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    sealed class StructMemoryComparer<T> : DefaultMemoryComparerBase<T>
        where T : struct
    {
        public static StructMemoryComparer<T> Instance = new();

        StructMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return HashOperations.GetHashCode(MemoryMarshal.AsBytes(obj.Span));
        }

        public override bool Equals(object? obj) => obj is StructMemoryComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
