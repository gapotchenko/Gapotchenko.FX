using System.Collections.Specialized;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    const int F_IsUndirected = 1 << 0;
    const int F_IsDirected_Initialized = 1 << 1;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    BitVector32 m_Flags;
}
