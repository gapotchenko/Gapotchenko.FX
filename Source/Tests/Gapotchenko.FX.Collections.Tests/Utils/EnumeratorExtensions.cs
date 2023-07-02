using Gapotchenko.FX.Linq;
using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Tests.Utils;

static class EnumeratorExtensions
{
    sealed class NotRepetableEnumerable<T> : IEnumerable<T>
    {
        public NotRepetableEnumerable(IEnumerable<T> source)
        {
            m_Source =
                Debugger.IsAttached ?
                    source.Memoize() :
                    source;
        }

        readonly IEnumerable<T> m_Source;
        bool m_Taken;

        public IEnumerator<T> GetEnumerator()
        {
            if (m_Taken)
                throw new InvalidOperationException("Enumeration sourced from an enumerator may only be taken once.");

            m_Taken = true;
            return m_Source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static IEnumerable<T> Take<T>(this IEnumerator<T> source, int count)
    {
        return new NotRepetableEnumerable<T>(TakeCore(source, count));

        static IEnumerable<T> TakeCore(IEnumerator<T> source, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                if (source.MoveNext())
                    yield return source.Current;
                else
                    throw new EndOfStreamException();
            }
        }
    }

    public static T Take<T>(this IEnumerator<T> source)
    {
        if (source.MoveNext())
            return source.Current;
        else
            throw new EndOfStreamException();
    }
}
