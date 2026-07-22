// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class ByteComparer : EquatableComparer<byte>
    {
        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            return (int)XxHash3.HashToUInt64(obj.Span);
        }
    }

    sealed class Int32Comparer : EquatableComparer<int>
    {
        public override int GetHashCode(ReadOnlyMemory<int> obj)
        {
            return (int)XxHash3.HashToUInt64(MemoryMarshal.AsBytes(obj.Span));
        }
    }
}
