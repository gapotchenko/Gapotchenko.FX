// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides extensions methods for <see cref="IAsyncLockable"/>.
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
    public static AsyncLockableScope LockScope(this IAsyncLockable lockable, CancellationToken cancellationToken = default)
    {
        if (lockable is null)
            throw new ArgumentNullException(nameof(lockable));

        lockable.Lock(cancellationToken);
        return new AsyncLockableScope(lockable);
    }

    /// <summary>
    /// Tries to lock the synchronization primitive and
    /// returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="AsyncLockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    public static AsyncLockableScope TryLockScope(this IAsyncLockable lockable) => TryLockScope(lockable, 0);

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
    public static AsyncLockableScope TryLockScope(this IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(lockable.TryLock(timeout, cancellationToken) ? lockable : null);

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
    public static AsyncLockableScope TryLockScope(this IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(lockable.TryLock(millisecondsTimeout, cancellationToken) ? lockable : null);

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
    public static Task<AsyncLockableScope> LockScopeAsync(this IAsyncLockable lockable, CancellationToken cancellationToken = default) =>
        (lockable ?? throw new ArgumentNullException(nameof(lockable)))
        .LockAsync(cancellationToken)
        .ContinueWith(
            _ => new AsyncLockableScope(lockable),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);

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
    public static Task<AsyncLockableScope> TryLockScopeAsync(this IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryLockScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryLockAsync(timeout, cancellationToken),
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
    public static Task<AsyncLockableScope> TryLockScopeAsync(this IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryLockScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryLockAsync(millisecondsTimeout, cancellationToken),
            cancellationToken);

    static Task<AsyncLockableScope> TryLockScopeAsyncCore(IAsyncLockable lockable, Task<bool> task, CancellationToken cancellationToken) =>
        task.ContinueWith(
            task => new AsyncLockableScope(task.Result ? lockable : null),
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
}
