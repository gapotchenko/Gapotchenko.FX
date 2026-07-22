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
    class EquatableComparer<T> : MemoryEqualityComparer<T>
    {
        public sealed override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => x.Span.SequenceEqual(y.Span);

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            var hash = new XxHash3();
            Span<int> buffer = stackalloc int[1];
            foreach (var i in obj.Span)
            {
                buffer[0] = i?.GetHashCode() ?? 0;
                hash.Append(MemoryMarshal.AsBytes(buffer));
            }
            return (int)hash.GetCurrentHashAsUInt64();
        }
    }
}
