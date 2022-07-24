﻿using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Generic;

sealed class CollectionDebugView<T>
{
    public CollectionDebugView(IReadOnlyCollection<T> collection)
    {
        m_Collection = collection;
    }

    readonly IReadOnlyCollection<T> m_Collection;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => m_Collection.ToArray();
}
