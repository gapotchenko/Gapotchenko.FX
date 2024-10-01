// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <summary>
    /// Clones the current graph.
    /// </summary>
    /// <returns>A new graph instance which is a clone of the current graph.</returns>
    public Graph<TVertex> Clone()
    {
        var clone = NewGraph();
        clone.UnionWith(this);
        clone.CopyCacheFrom(this);
        return clone;
    }

    IGraph<TVertex> ICloneable<IGraph<TVertex>>.Clone() => Clone();

    object ICloneable.Clone() => Clone();
}
