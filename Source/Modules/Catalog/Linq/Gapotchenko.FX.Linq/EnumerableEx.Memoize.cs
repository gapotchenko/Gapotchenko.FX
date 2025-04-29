using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Memoizes (caches) all elements of a sequence by ensuring that every element is retrieved only once.
    /// </summary>
    /// <remarks>
    /// The resulting sequence is not thread safe.
    /// </remarks>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>The sequence that fully replicates the source with all elements being memoized.</returns>
    public static IEnumerable<T> Memoize<T>(this IEnumerable<T> source) => Memoize(source, false);

    /// <summary>
    /// Memoizes (caches) all elements of a sequence by ensuring that every element is retrieved only once.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="isThreadSafe">Indicates whether resulting sequence is thread safe.</param>
    /// <returns>The sequence that fully replicates the source with all elements being memoized.</returns>
    public static IEnumerable<T> Memoize<T>(this IEnumerable<T> source, bool isThreadSafe)
    {
        switch (source)
        {
            case null:
#if BINARY_COMPATIBILITY
                return null!;
#else
                throw new ArgumentNullException(nameof(source));
#endif

            case CachedEnumerable<T> cachedEnumerable:
                if (!isThreadSafe ||
                    cachedEnumerable is ThreadSafeCachedEnumerable<T>)
                {
                    // The source is already memoized with compatible parameters.
                    return cachedEnumerable;
                }
                break;

            case IList<T>:
            case IReadOnlyList<T>:
            case string:
                // Given source types are intrinsically memoized by their nature.
                return source;
        }

#pragma warning disable IDE0306 // Simplify collection initialization
        if (isThreadSafe)
            return new ThreadSafeCachedEnumerable<T>(source);
        else
            return new CachedEnumerable<T>(source);
#pragma warning restore IDE0306 // Simplify collection initialization
    }

    class CachedEnumerable<T>(IEnumerable<T> source) : IReadOnlyList<T>
    {
        public virtual T this[int index]
        {
            get
            {
                EnsureItemIsCachedNoLock(index);
                return Cache[index];
            }
        }

        internal virtual bool EnsureItemIsCached(int index) => EnsureItemIsCachedNoLock(index);

        bool EnsureItemIsCachedNoLock(int index)
        {
            while (Cache.Count <= index)
            {
                if (!TryCacheElementNoLock())
                    return false;
            }
            return true;
        }

        public virtual int Count
        {
            get
            {
                while (TryCacheElementNoLock()) ;
                return Cache.Count;
            }
        }

        protected bool CacheIsBuilt => m_Source == null && m_SourceEnumerator == null;

        bool TryCacheElementNoLock()
        {
            if (m_SourceEnumerator == null)
            {
                if (m_Source != null)
                {
                    // The first access.
                    m_SourceEnumerator = m_Source.GetEnumerator();
                    m_Source = null;
                }
                else
                {
                    // Source enumerator already reached the end.
                    return false;
                }
            }

            if (m_SourceEnumerator.MoveNext())
            {
                Cache.Add(m_SourceEnumerator.Current);
                return true;
            }
            else
            {
                // Source enumerator has reached the end, so it is no longer needed.
                m_SourceEnumerator.Dispose();
                ClearSourceEnumerator();
                return false;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<T>? m_Source = source;

        protected virtual void ClearSourceEnumerator()
        {
            m_SourceEnumerator = null;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerator<T>? m_SourceEnumerator;

        public IEnumerator<T> GetEnumerator() => new CachedEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal virtual T GetCacheItem(int index) => Cache[index];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly IList<T> Cache = [];
    }

    sealed class ThreadSafeCachedEnumerable<T>(IEnumerable<T> source) : CachedEnumerable<T>(source)
    {
        public override int Count
        {
            get
            {
                if (CacheIsBuilt)
                {
                    return Cache.Count;
                }
                else
                {
                    lock (Cache)
                        return base.Count;
                }
            }
        }

        public override T this[int index]
        {
            get
            {
                if (CacheIsBuilt)
                    return Cache[index];
                else
                    return GetItemWithLock(index);
            }
        }

        // Should be a separate method to allow code inlining opportunities for this[index] getter.
        T GetItemWithLock(int index)
        {
            lock (Cache)
                return base[index];
        }

        internal override bool EnsureItemIsCached(int index)
        {
            if (CacheIsBuilt)
            {
                return base.EnsureItemIsCached(index);
            }
            else
            {
                lock (Cache)
                    return base.EnsureItemIsCached(index);
            }
        }

        internal override T GetCacheItem(int index)
        {
            if (CacheIsBuilt)
            {
                return base.GetCacheItem(index);
            }
            else
            {
                lock (Cache)
                    return base.GetCacheItem(index);
            }
        }

        protected override void ClearSourceEnumerator()
        {
            Thread.MemoryBarrier();
            base.ClearSourceEnumerator();
        }
    }

    sealed class CachedEnumerator<T>(CachedEnumerable<T> cachedEnumerable) : IEnumerator<T>
    {
        public T Current
        {
            get
            {
                var cachedEnumerable = m_CachedEnumerable ?? throw new InvalidOperationException();

                int index = m_Index;
                if (index < 0)
                    throw new InvalidOperationException();

                return cachedEnumerable.GetCacheItem(index);
            }
        }

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
            m_CachedEnumerable = null;
        }

        public bool MoveNext()
        {
            var cachedEnumerable = m_CachedEnumerable;
            if (cachedEnumerable == null)
            {
                // Disposed.
                return false;
            }

            if (m_Index == EofIndex)
                return false;

            m_Index++;
            if (!cachedEnumerable.EnsureItemIsCached(m_Index))
            {
                m_Index = EofIndex;
                return false;
            }
            else
            {
                return true;
            }
        }

        CachedEnumerable<T>? m_CachedEnumerable = cachedEnumerable;

        public void Reset()
        {
            m_Index = InitialIndex;
        }

        int m_Index = InitialIndex;

        const int InitialIndex = -1;
        const int EofIndex = -2;
    }
}
