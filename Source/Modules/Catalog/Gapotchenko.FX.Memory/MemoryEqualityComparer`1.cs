// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Equality comparer for contiguous regions of memory represented by <see cref="ReadOnlyMemory{T}"/> type.
/// </summary>
public abstract class MemoryEqualityComparer<T> : EqualityComparer<ReadOnlyMemory<T>>
{
    /// <summary>
    /// Returns a default equality comparer for contiguous regions of memory with an element type specified by the generic argument <typeparamref name="T"/>.
    /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static new MemoryEqualityComparer<T> Default => DefaultFactory.Instance;
#pragma warning restore CA1000 // Do not declare static members on generic types

    static class DefaultFactory
    {
        public static readonly MemoryEqualityComparer<T> Instance = MemoryEqualityComparer.Create<T>(null);
    }
}
