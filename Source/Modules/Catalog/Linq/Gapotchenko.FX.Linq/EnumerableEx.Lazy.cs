// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq.Properties;
using Gapotchenko.FX.Threading;
using System.Collections;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> that is lazily initialized using the specified factory upon actual enumeration.
    /// </summary>
    /// <typeparam name="T">The type of enumerable objects.</typeparam>
    /// <param name="factory">The delegate that is invoked to produce the lazily initialized <see cref="IEnumerable{T}"/> when it is needed.</param>
    /// <returns>A lazily initialized <see cref="IEnumerable{T}"/> instance.</returns>
    public static IEnumerable<T> Lazy<T>(Func<IEnumerable<T>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        return new LazyEnumerable<T>(factory);
    }

    sealed class LazyEnumerable<T>(Func<IEnumerable<T>> factory) : IEnumerable<T>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator() => new Enumerator(() => m_Factory.Value.GetEnumerator());

        EvaluateOnce<IEnumerable<T>> m_Factory = new(factory);

        sealed class Enumerator(Func<IEnumerator<T>> factory) : IEnumerator<T>
        {
            object? IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    var factory = GetFactory();
                    if (factory.IsValueCreated)
                        return factory.Value.Current;
                    else
                        throw new InvalidOperationException(Resources.EnumerationHasNotStarted);
                }
            }

            public bool MoveNext() => GetFactory().Value.MoveNext();

            public void Reset()
            {
                var factory = GetFactory();
                if (factory.IsValueCreated)
                    factory.Value.Reset();
            }

            Lazy<IEnumerator<T>> GetFactory()
            {
                var factory = m_Factory;
                ObjectDisposedException.ThrowIf(factory is null, this);
                return factory;
            }

            public void Dispose()
            {
                if (m_Factory is { } factory)
                {
                    if (factory.IsValueCreated)
                        factory.Value.Dispose();
                    else
                        m_Factory = null;
                }
            }

            Lazy<IEnumerator<T>>? m_Factory = new(factory, false);
        }
    }
}
