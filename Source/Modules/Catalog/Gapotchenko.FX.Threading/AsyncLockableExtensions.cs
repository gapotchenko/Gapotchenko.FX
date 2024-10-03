// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

#pragma warning disable CA1062

/// <summary>
/// Provides extension methods for <see cref="IAsyncLockable"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AsyncLockableExtensions
{
    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// and returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A scope that can be disposed to unlock the synchronization primitive.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static AsyncLockableScope EnterScope(this IAsyncLockable lockable, CancellationToken cancellationToken = default)
    {
        if (lockable is null)
            throw new ArgumentNullException(nameof(lockable));

        lockable.Enter(cancellationToken);
        return new AsyncLockableScope(lockable);
    }

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive,
    /// and returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <returns>
    /// A task that will complete when the synchronization primitive has been locked with a result of the scope that can be disposed to unlock the synchronization primitive.
    /// </returns>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static Task<AsyncLockableScope> EnterScopeAsync(this IAsyncLockable lockable, CancellationToken cancellationToken = default) =>
        (lockable ?? throw new ArgumentNullException(nameof(lockable)))
        .EnterAsync(cancellationToken)
        .ContinueWith(
            _ => new AsyncLockableScope(lockable),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);

    /// <summary>
    /// Tries to lock the synchronization primitive and
    /// returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    public static AsyncLockableScope TryEnterScope(this IAsyncLockable lockable) => TryEnterScope(lockable, 0);

    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the synchronization primitive.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static AsyncLockableScope TryEnterScope(this IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(lockable.TryEnter(timeout, cancellationToken) ? lockable : null);

    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to unlock the synchronization primitive.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static AsyncLockableScope TryEnterScope(this IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(lockable.TryEnter(millisecondsTimeout, cancellationToken) ? lockable : null);

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive,
    /// using a <see cref="TimeSpan"/> to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the synchronization primitive.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static Task<AsyncLockableScope> TryEnterScopeAsync(this IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryEnterScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryEnterAsync(timeout, cancellationToken),
            cancellationToken);

    /// <summary>
    /// Asynchronously waits to lock the synchronization primitive,
    /// using a 32-bit signed integer to measure the time interval,
    /// and returns a disposable scope that can be disposed to unlock the synchronization primitive.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to lock the synchronization primitive and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static Task<AsyncLockableScope> TryEnterScopeAsync(this IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryEnterScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryEnterAsync(millisecondsTimeout, cancellationToken),
            cancellationToken);

    static Task<AsyncLockableScope> TryEnterScopeAsyncCore(IAsyncLockable lockable, Task<bool> task, CancellationToken cancellationToken) =>
        task.ContinueWith(
            task => new AsyncLockableScope(task.Result ? lockable : null),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
}
