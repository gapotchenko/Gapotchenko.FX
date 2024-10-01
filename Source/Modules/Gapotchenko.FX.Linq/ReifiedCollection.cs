using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Linq;

sealed class ReifiedCollection<T> : IReadOnlyCollection<T>
{
    public ReifiedCollection(IEnumerable<T> source, int count)
    {
        m_Source = source;
        m_Count = count;
    }

    readonly IEnumerable<T> m_Source;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly int m_Count;

    public int Count => m_Count;

    public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Source).GetEnumerator();
}
