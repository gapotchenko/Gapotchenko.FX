// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    class EquatableComparer<T> : MemoryEqualityComparer<T>
    {
        public sealed override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => x.Span.SequenceEqual(y.Span);

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            // FNV-1a
            uint hash = 2166136261;
            foreach (var i in obj.Span)
                hash = (hash ^ (uint)(i?.GetHashCode() ?? 0)) * 16777619;
            return (int)hash;
        }
    }
}
