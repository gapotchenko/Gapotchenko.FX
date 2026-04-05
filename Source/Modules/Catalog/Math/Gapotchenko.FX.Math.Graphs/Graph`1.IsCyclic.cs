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

        if (IsDirected)
        {
            var recStack = new HashSet<TVertex>(comparer);

            foreach (var v in Vertices)
            {
                if (IsCyclicHelper(v))
                    return true;
            }

            bool IsCyclicHelper(TVertex v)
            {
                if (recStack.Contains(v))
                    return true;

                if (!visited.Add(v))
                    return false;

                recStack.Add(v);

                foreach (var i in OutgoingVerticesAdjacentTo(v))
                {
                    if (IsCyclicHelper(i))
                        return true;
                }

                recStack.Remove(v);

                return false;
            }
        }
        else
        {
            foreach (var v in Vertices)
            {
                if (!visited.Contains(v) && IsCyclicHelper(v, default))
                    return true;
            }

            bool IsCyclicHelper(TVertex v, Optional<TVertex> parent)
            {
                visited.Add(v);

                foreach (var i in OutgoingVerticesAdjacentTo(v))
                {
                    if (parent.HasValue && comparer.Equals(i, parent.Value))
                        continue;

                    if (visited.Contains(i))
                        return true;

                    if (IsCyclicHelper(i, v))
                        return true;
                }

                return false;
            }
        }

        return false;
    }
}
