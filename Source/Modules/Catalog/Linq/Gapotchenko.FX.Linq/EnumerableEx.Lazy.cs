// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Threading;
using System.Collections;

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> that is lazily initialized by the specified factory.
    /// </summary>
    /// <typeparam name="T">The type of enumerable objects.</typeparam>
    /// <param name="factory">The delegate that is invoked to produce the lazily initialized <see cref="IEnumerable{T}"/> when it is needed.</param>
    /// <returns>A lazily initialized <see cref="IEnumerable{T}"/> instance.</returns>
    public static IEnumerable<T> Lazy<T>(Func<IEnumerable<T>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        return new LazyEnumerable<T>(factory);
    }

    sealed class LazyEnumerable<T> : IEnumerable<T>
    {
        public LazyEnumerable(Func<IEnumerable<T>> factory)
        {
            m_Factory = new(factory, this);
        }

        public IEnumerator<T> GetEnumerator() => m_Factory.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        EvaluateOnce<IEnumerable<T>> m_Factory;
    }
}
