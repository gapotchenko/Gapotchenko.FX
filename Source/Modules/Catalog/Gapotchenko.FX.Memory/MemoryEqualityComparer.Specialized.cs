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
    sealed class ByteComparer : EquatableComparer<byte>
    {
        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            return HashOperations.GetHashCode(obj.Span);
        }
    }

    sealed class Int32Comparer : EquatableComparer<int>
    {
        public override int GetHashCode(ReadOnlyMemory<int> obj)
        {
            return HashOperations.GetHashCode(MemoryMarshal.AsBytes(obj.Span));
        }
    }
}
