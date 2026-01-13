// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides extension methods for <see cref="ILockable"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LockableExtensions
{
    /// <returns>A <see cref="LockableScope"/> that can be disposed to exit the lock.</returns>
    /// <inheritdoc cref="ILockable.Enter(CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static LockableScope EnterScope(this ILockable lockable, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lockable);

        lockable.Enter(cancellationToken);
        return new LockableScope(lockable);
    }

    /// <remarks>
    /// <para>
    /// When the method returns a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="true"/>,
    /// the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method returns without waiting for the lock and
    /// <see cref="LockableScope.HasLock"/> property of the returned <see cref="LockableScope"/> is set to <see langword="false"/>.
    /// </para>
    /// <para>
    /// This method is intended to be used with a language construct that automatically disposes the <see cref="LockableScope"/>,
    /// such as the C# <see langword="using"/> keyword.
    /// </para>
    /// </remarks>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <returns>
    /// A <see cref="LockableScope"/> that can be disposed to exit the lock.
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully entered the lock.
    /// </returns>
    /// <inheritdoc cref="ILockable.TryEnter()"/>
    public static LockableScope TryEnterScope(this ILockable lockable) =>
        new(
            (lockable ?? throw new ArgumentNullException(nameof(lockable)))
            .TryEnter()
            ? lockable : null);

    /// <remarks>
    /// When the method returns a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="true"/>,
    /// the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="timeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns
    /// a <see cref="LockableScope"/> with <see cref="LockableScope.HasLock"/> property set to <see langword="false"/>.
    /// </remarks>
    /// <returns>
    /// A <see cref="LockableScope"/> that can be disposed to exit the lock.
    /// <see cref="LockableScope.HasLock"/> property indicates whether the current thread successfully entered the lock.
    /// </returns>
    /// <inheritdoc cref="ILockable.TryEnter(TimeSpan, CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="timeout"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static LockableScope TryEnterScope(this ILockable lockable, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(
            (lockable ?? throw new ArgumentNullException(nameof(lockable)))
            .TryEnter(timeout, cancellationToken)
            ? lockable : null);

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
    /// <inheritdoc cref="ILockable.TryEnter(int, CancellationToken)"/>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <param name="millisecondsTimeout"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    public static LockableScope TryEnterScope(this ILockable lockable, int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(
            (lockable ?? throw new ArgumentNullException(nameof(lockable)))
            .TryEnter(millisecondsTimeout, cancellationToken)
            ? lockable : null);
}
