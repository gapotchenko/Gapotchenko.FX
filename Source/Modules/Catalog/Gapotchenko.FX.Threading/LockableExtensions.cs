// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Threading.Utils;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides extension methods for <see cref="ILockable"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LockableExtensions
{
    /// <summary>
    /// Blocks the current thread until it can lock the synchronization primitive,
    /// and returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A scope that can be disposed to unlock the synchronization primitive.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static LockableScope EnterScope(this ILockable lockable, CancellationToken cancellationToken = default)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(lockable);

        lockable.Enter(cancellationToken);
        return new LockableScope(lockable);
    }

    /// <summary>
    /// Tries to lock the synchronization primitive and
    /// returns a disposable scope that can be disposed to unlock it.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <returns>
    /// A scope that can be disposed to unlock the synchronization primitive.
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    public static LockableScope TryEnterScope(this ILockable lockable) => TryEnterScope(lockable, 0);

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
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static LockableScope TryEnterScope(this ILockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(
            (lockable ?? throw new ArgumentNullException(nameof(lockable)))
            .TryEnter(timeout, cancellationToken) ? lockable : null);

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
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully locked the synchronization primitive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public static LockableScope TryEnterScope(this ILockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(
            (lockable ?? throw new ArgumentNullException(nameof(lockable)))
            .TryEnter(millisecondsTimeout, cancellationToken) ? lockable : null);
}
