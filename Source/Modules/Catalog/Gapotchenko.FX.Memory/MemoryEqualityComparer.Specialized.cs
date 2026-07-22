// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class ByteComparer : EquatableComparer<byte>
    {
        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            // FNV-1a
            uint hash = 2166136261;
            foreach (byte i in obj.Span)
                hash = (hash ^ i) * 16777619;
            return (int)hash;
        }
    }

    sealed class Int32Comparer : EquatableComparer<int>
    {
        public override int GetHashCode(ReadOnlyMemory<int> obj)
        {
            // FNV-1a
            uint hash = 2166136261;
            foreach (int i in obj.Span)
                hash = (hash ^ (uint)i) * 16777619;
            return (int)hash;
        }
    }
}
