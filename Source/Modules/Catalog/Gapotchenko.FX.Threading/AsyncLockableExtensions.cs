// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides extension methods for <see cref="IAsyncLockable"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AsyncLockableExtensions
{
    /// <returns>A <see cref="LockableScope"/> that can be disposed to exit the lock.</returns>
    /// <inheritdoc cref="IAsyncLockable.EnterAsync(CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task<LockableScope> EnterScopeAsync(this IAsyncLockable lockable, CancellationToken cancellationToken = default) =>
        (lockable ?? throw new ArgumentNullException(nameof(lockable)))
        .EnterAsync(cancellationToken)
        .Then(() => new LockableScope(lockable));

    /// <remarks>
    /// When the method returns a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="true"/>,
    /// the current task is the only task that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="timeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns
    /// a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="false"/>.
    /// </remarks>
    /// <returns>
    /// A <see cref="LockableScope"/> that can be disposed to exit the lock.
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current task successfully entered the lock.
    /// </returns>
    /// <inheritdoc cref="IAsyncLockable.TryEnterAsync(TimeSpan, CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="timeout"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task<LockableScope> TryEnterScopeAsync(this IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        TryEnterScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryEnterAsync(timeout, cancellationToken));

    /// <remarks>
    /// When the method returns a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="true"/>,
    /// the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="millisecondsTimeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns
    /// a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="false"/>.
    /// </remarks>
    /// <returns>
    /// A <see cref="LockableScope"/> that can be disposed to exit the lock.
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully entered the lock.
    /// </returns>
    /// <inheritdoc cref="IAsyncLockable.TryEnterAsync(int, CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="millisecondsTimeout"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static Task<LockableScope> TryEnterScopeAsync(this IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        TryEnterScopeAsyncCore(
            lockable ?? throw new ArgumentNullException(nameof(lockable)),
            lockable.TryEnterAsync(millisecondsTimeout, cancellationToken));

    static Task<LockableScope> TryEnterScopeAsyncCore(IAsyncLockable lockable, Task<bool> tryEnterFunc) =>
        tryEnterFunc.Then(result => new LockableScope(result ? lockable : null));
}
