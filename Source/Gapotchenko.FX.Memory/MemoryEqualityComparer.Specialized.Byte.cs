using System;

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class ByteComparer : EquatableComparer<byte>
    {
        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            // FNV-1a
            uint hash = 2166136261;
            foreach (var i in obj.Span)
                hash = (hash ^ i) * 16777619;
            return (int)hash;
        }
    }
}
