// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_QUEUE_TRYDEQUEUE
#endif

using Gapotchenko.FX.Collections.Utils;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Provides polyfill extension methods for <see cref="Queue{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class QueuePolyfills
{
    /// <summary>
    /// Removes the object at the beginning of the <see cref="Queue{T}"/>,
    /// and copies it to the <paramref name="result"/> parameter.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    /// <param name="queue">The <see cref="Deque{T}"/>.</param>
    /// <param name="result">The removed object.</param>
    /// <returns>
    /// <see langword="true"/> if the object is successfully removed;
    /// <see langword="false"/> if the <see cref="Queue{T}"/> is empty.
    /// </returns>
#if !TFF_QUEUE_TRYDEQUEUE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool TryDequeue<T>(
#if !TFF_QUEUE_TRYDEQUEUE
        this
#endif
        Queue<T> queue, [MaybeNullWhen(false)] out T result)
#if TFF_QUEUE_TRYDEQUEUE
        => queue.TryDequeue(out result);
#else
    {
        ExceptionHelper.ThrowIfThisIsNull(queue);

        if (queue.Count == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = queue.Dequeue();
            return true;
        }
    }
#endif
}
