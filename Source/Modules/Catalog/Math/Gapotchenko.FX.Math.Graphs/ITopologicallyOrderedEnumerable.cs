// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Represents a topologically ordered sequence.
/// </summary>
/// <typeparam name="TElement">The type of the elements of the sequence.</typeparam>
public interface ITopologicallyOrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
{
    /// <summary>
    /// Inverts the topological order of the elements in a sequence.
    /// </summary>
    /// <returns>
    /// A sequence whose elements correspond to those of the input sequence in reverse topological order.
    /// </returns>
    public ITopologicallyOrderedEnumerable<TElement> Reverse();
}
