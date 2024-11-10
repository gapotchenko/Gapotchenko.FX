using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class CollectionDebugView<T>(IReadOnlyCollection<T> collection)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => collection.ToArray();
}
