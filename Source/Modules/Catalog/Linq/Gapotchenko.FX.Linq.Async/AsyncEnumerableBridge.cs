// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Linq;

/// <summary>
/// Bridges synchronous and asynchronous enumeration models together.
/// </summary>
public static class AsyncEnumerableBridge
{
    // ------------------------------------------------------------------------
    // Public Facade
    // ------------------------------------------------------------------------

    /// <summary>
    /// Synchronously enumerates the values from an asynchronous <see cref="IAsyncEnumerable{T}"/> source.
    /// </summary>
    /// <param name="source">The asynchronous <see cref="IAsyncEnumerable{T}"/> source to enumerate the values from.</param>
    /// <typeparam name="T">The type of values to enumerate.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that enumerates the values from the specified <see cref="IAsyncEnumerable{T}"/> source.</returns>
    [return: NotNullIfNotNull(nameof(source))]
    public static IEnumerable<T>? Enumerate<T>(IAsyncEnumerable<T>? source) => Enumerate(source, default);

    /// <inheritdoc cref="Enumerate{T}(IAsyncEnumerable{T}?)"/>
    /// <param name="source"><inheritdoc/></param>
    /// <param name="cancellationToken">The additional cancellation token to propagate to the enumerated source.</param>
    [return: NotNullIfNotNull(nameof(source))]
    public static IEnumerable<T>? Enumerate<T>(IAsyncEnumerable<T>? source, CancellationToken cancellationToken)
    {
        if (source is null)
            return null;
        else
            return EnumerateCore(source, cancellationToken);
    }

    // ------------------------------------------------------------------------

    /// <summary>
    /// Asynchronously enumerates the values from a synchronous <see cref="IEnumerable{T}"/> source.
    /// </summary>
    /// <param name="source">The synchronous <see cref="IEnumerable{T}"/> source to enumerate the values from.</param>
    /// <typeparam name="T">The type of values to enumerate.</typeparam>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that enumerates the values from the specified <see cref="IEnumerable{T}"/> source.</returns>
    [return: NotNullIfNotNull(nameof(source))]
    public static IAsyncEnumerable<T>? EnumerateAsync<T>(IEnumerable<T>? source) => EnumerateAsync(source, default);

    /// <summary>
    /// Asynchronously enumerates the values from a synchronous <see cref="IEnumerable{T}"/> source.
    /// If a cancellation is requested then a thread executing the synchronous enumeration is terminated.
    /// </summary>
    /// <inheritdoc cref="EnumerateAsync{T}(IEnumerable{T})"/>
    /// <param name="source"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [return: NotNullIfNotNull(nameof(source))]
    public static IAsyncEnumerable<T>? EnumerateAsync<T>(IEnumerable<T>? source, CancellationToken cancellationToken)
    {
        if (source is null)
            return null;
        else
            return EnumerateAsyncCore(source.GetEnumerator, cancellationToken);
    }

    /// <summary>
    /// Asynchronously enumerates the values from a synchronous <see cref="IEnumerable{T}"/> source.
    /// If a cancellation is requested then a thread executing the synchronous enumeration is terminated.
    /// </summary>
    /// <inheritdoc cref="EnumerateAsync{T}(IEnumerable{T})"/>
    /// <param name="sourceFunc">The function returning a synchronous <see cref="IEnumerable{T}"/> source to enumerate the values from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="sourceFunc"/> is <see langword="null"/>.</exception>
    public static IAsyncEnumerable<T> EnumerateAsync<T>(Func<IEnumerable<T>> sourceFunc, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sourceFunc);

        return EnumerateAsyncCore(
            () => sourceFunc().GetEnumerator(),
            cancellationToken);
    }

    // ------------------------------------------------------------------------
    // Core Implementation
    // ------------------------------------------------------------------------

    static IEnumerable<T> EnumerateCore<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var enumerator = TaskBridge.Execute(
            ct =>
            {
                using var ctr = ct.Register(() => cts.Cancel());
                return Task.FromResult(source.GetAsyncEnumerator(cts.Token));
            },
            cancellationToken);
        try
        {
            while (TaskBridge.Execute(
                async Task<bool> (ct) =>
                {
                    using var ctr = ct.Register(() => cts.Cancel());
                    return await enumerator.MoveNextAsync().ConfigureAwait(false);
                },
                cancellationToken))
            {
                yield return enumerator.Current;
            }
        }
        finally
        {
            TaskBridge.Execute(
                enumerator.DisposeAsync()
#if !TFF_VALUETASK
                    .AsTask()
#endif
                );
        }
    }

    // ------------------------------------------------------------------------

    static IAsyncEnumerable<T> EnumerateAsyncCore<T>(Func<IEnumerator<T>> getEnumerator, CancellationToken cancellationToken)
    {
        return ImplCore(getEnumerator, cancellationToken);

        static async IAsyncEnumerable<T> ImplCore(
            Func<IEnumerator<T>> getEnumerator,
            CancellationToken cancellationToken,
            [EnumeratorCancellation] CancellationToken enumeratorCancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, enumeratorCancellationToken);

            var enumerator = await TaskBridge.ExecuteAsync(getEnumerator, cts.Token).ConfigureAwait(false);
            try
            {
                while (await TaskBridge.ExecuteAsync(enumerator.MoveNext, cts.Token).ConfigureAwait(false))
                    yield return enumerator.Current;
            }
            finally
            {
                await TaskBridge.ExecuteAsync(enumerator.Dispose, cts.Token).ConfigureAwait(false);
            }
        }
    }
}
