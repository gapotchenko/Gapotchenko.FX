using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
        /// <summary>
        /// Memoize all elements of a sequence by ensuring that every element is retrieved only once.
        /// </summary>
        /// <remarks>
        /// The resulting sequence is not thread safe.
        /// </remarks>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>The sequence that fully replicates the source with all elements being memoized.</returns>
        [return: NotNullIfNotNull("source")]
        public static IEnumerable<T>? Memoize<T>(this IEnumerable<T>? source) => Memoize(source, false);

        /// <summary>
        /// Memoize all elements of a sequence by ensuring that every element is retrieved only once.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="isThreadSafe">Indicates whether resulting sequence is thread safe.</param>
        /// <returns>The sequence that fully replicates the source with all elements being memoized.</returns>
        [return: NotNullIfNotNull("source")]
        public static IEnumerable<T>? Memoize<T>(this IEnumerable<T>? source, bool isThreadSafe)
        {
            switch (source)
            {
                case null:
                    return null;

                case CachedEnumerable<T> existingCachedEnumerable:
                    if (!isThreadSafe || existingCachedEnumerable is ThreadSafeCachedEnumerable<T>)
                    {
                        // The source is already memoized with compatible parameters.
                        return existingCachedEnumerable;
                    }
                    break;

                case IList<T> _:
                case IReadOnlyList<T> _:
                case string _:
                    // Given source types are intrinsically memoized by their nature.
                    return source;
            }

            if (isThreadSafe)
                return new ThreadSafeCachedEnumerable<T>(source);
            else
                return new CachedEnumerable<T>(source);
        }

        class CachedEnumerable<T> : IEnumerable<T>, IReadOnlyList<T>
        {
            public CachedEnumerable(IEnumerable<T> source)
            {
                _Source = source;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IEnumerable<T>? _Source;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IEnumerator<T>? _SourceEnumerator;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            protected readonly IList<T> Cache = new List<T>();

            public virtual int Count
            {
                get
                {
                    while (_TryCacheElementNoLock()) ;
                    return Cache.Count;
                }
            }

            bool _TryCacheElementNoLock()
            {
                if (_SourceEnumerator == null && _Source != null)
                {
                    _SourceEnumerator = _Source.GetEnumerator();
                    _Source = null;
                }

                if (_SourceEnumerator == null)
                {
                    // Source enumerator already reached the end.
                    return false;
                }
                else if (_SourceEnumerator.MoveNext())
                {
                    Cache.Add(_SourceEnumerator.Current);
                    return true;
                }
                else
                {
                    // Source enumerator has reached the end, so it is no longer needed.
                    _SourceEnumerator.Dispose();
                    _SourceEnumerator = null;
                    return false;
                }
            }

            public virtual T this[int index]
            {
                get
                {
                    _EnsureItemIsCachedNoLock(index);
                    return Cache[index];
                }
            }

            public IEnumerator<T> GetEnumerator() => new CachedEnumerator<T>(this);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            internal virtual bool EnsureItemIsCached(int index) => _EnsureItemIsCachedNoLock(index);

            bool _EnsureItemIsCachedNoLock(int index)
            {
                while (Cache.Count <= index)
                {
                    if (!_TryCacheElementNoLock())
                        return false;
                }
                return true;
            }

            internal virtual T GetCacheItem(int index) => Cache[index];
        }

        sealed class ThreadSafeCachedEnumerable<T> : CachedEnumerable<T>
        {
            public ThreadSafeCachedEnumerable(IEnumerable<T> source) :
                base(source)
            {
            }

            public override int Count
            {
                get
                {
                    lock (Cache)
                        return base.Count;
                }
            }

            public override T this[int index]
            {
                get
                {
                    lock (Cache)
                        return base[index];
                }
            }

            internal override bool EnsureItemIsCached(int index)
            {
                lock (Cache)
                    return base.EnsureItemIsCached(index);
            }

            internal override T GetCacheItem(int index)
            {
                lock (Cache)
                    return base.GetCacheItem(index);
            }
        }

        sealed class CachedEnumerator<T> : IEnumerator<T>
        {
            CachedEnumerable<T>? _CachedEnumerable;

            const int InitialIndex = -1;
            const int EofIndex = -2;

            int _Index = InitialIndex;

            public CachedEnumerator(CachedEnumerable<T> cachedEnumerable)
            {
                _CachedEnumerable = cachedEnumerable;
            }

            public T Current
            {
                get
                {
                    var cachedEnumerable = _CachedEnumerable;
                    if (cachedEnumerable == null)
                        throw new InvalidOperationException();

                    var index = _Index;
                    if (index < 0)
                        throw new InvalidOperationException();

                    return cachedEnumerable.GetCacheItem(index);
                }
            }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                _CachedEnumerable = null;
            }

            public bool MoveNext()
            {
                var cachedEnumerable = _CachedEnumerable;
                if (cachedEnumerable == null)
                {
                    // Disposed.
                    return false;
                }

                if (_Index == EofIndex)
                    return false;

                _Index++;
                if (!cachedEnumerable.EnsureItemIsCached(_Index))
                {
                    _Index = EofIndex;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void Reset()
            {
                _Index = InitialIndex;
            }
        }
    }
}
