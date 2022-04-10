using System;
using System.Linq;

namespace Gapotchenko.FX.Memory
{
    partial class MemoryEqualityComparer
    {
        class EquatableComparer<T> : MemoryEqualityComparer<T>
            where T : IEquatable<T>
        {
            public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => x.Span.SequenceEqual(y.Span);

            public override int GetHashCode(ReadOnlyMemory<T> obj)
            {
                // FNV-1a
                uint hash = 2166136261;
                foreach (var i in obj.Span)
                    hash = (hash ^ (uint)i.GetHashCode()) * 16777619;
                return (int)hash;
            }
        }
    }
}
