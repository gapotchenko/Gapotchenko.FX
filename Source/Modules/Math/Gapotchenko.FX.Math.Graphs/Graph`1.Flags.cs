// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Collections.Specialized;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    const int F_IsUndirected = 1 << 0;
    const int F_IsUndirected_Initialized = 1 << 1;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    BitVector32 m_Flags;
}
