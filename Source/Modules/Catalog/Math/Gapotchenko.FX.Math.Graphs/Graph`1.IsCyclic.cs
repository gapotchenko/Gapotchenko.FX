// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Math.Graphs.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool IsCyclic => IsCyclicHint ??= IsCyclicCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool? IsCyclicHint
    {
        get => m_CachedFlags.GetNullableBooleanValue(CF_IsCyclic_HasValue, CF_IsCyclic_Value);
        set => m_CachedFlags.SetNullableBooleanValue(CF_IsCyclic_HasValue, CF_IsCyclic_Value, value);
    }

    bool IsCyclicCore()
    {
        var comparer = VertexComparer;
        var visited = new HashSet<TVertex>(comparer);
        var recStack = new HashSet<TVertex>(comparer);

        bool IsCyclicHelper(TVertex v)
        {
            if (recStack.Contains(v))
                return true;

            if (!visited.Add(v))
                return false;

            recStack.Add(v);

            foreach (var i in OutgoingVerticesAdjacentTo(v))
                if (IsCyclicHelper(i))
                    return true;

            recStack.Remove(v);

            return false;
        }

        foreach (var v in Vertices)
        {
            if (IsCyclicHelper(v))
                return true;
        }

        return false;
    }
}
