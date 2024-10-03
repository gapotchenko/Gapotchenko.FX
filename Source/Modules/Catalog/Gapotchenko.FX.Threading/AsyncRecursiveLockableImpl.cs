// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

readonly struct AsyncRecursiveLockableImpl<TLockable> : IAsyncRecursiveLockable
    where TLockable : IAsyncLockable
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    public AsyncRecursiveLockableImpl(TLockable lockable)
    {
        m_Lockable = lockable;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly TLockable m_Lockable;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursionTracker m_RecursionTracker = new();

    public void Lock(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Lock(cancellationToken);
        m_RecursionTracker.Enter();
    }

    /// <inheritdoc/>
    public Task LockAsync(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.Enter);
            var lockable = m_Lockable;

            async Task ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    await lockable.LockAsync(cancellationToken).ConfigureAwait(false);
                    asyncLocalChanges.Commit();
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return Task.CompletedTask;
        }
    }

    public bool TryLock() => TryLock(0, CancellationToken.None);

    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockCore(() => lockable.TryLock(timeout, cancellationToken));
    }

    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockCore(() => lockable.TryLock(millisecondsTimeout, cancellationToken));
    }

    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockAsyncCore(() => lockable.TryLockAsync(timeout, cancellationToken));
    }

    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockAsyncCore(() => lockable.TryLockAsync(millisecondsTimeout, cancellationToken));
    }

    public void Unlock()
    {
        if (m_RecursionTracker.Leave())
            m_Lockable.Unlock();
    }

    public bool IsLocked => m_Lockable.IsLocked;

    public bool IsHeldByCurrentTask => m_RecursionTracker.IsEntered;

    bool IAsyncLockable.IsRecursive => true;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    bool TryLockCore(Func<bool> func)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            bool locked = func();
            if (locked)
                m_RecursionTracker.Enter();
            return locked;
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return true;
        }
    }

    Task<bool> TryLockAsyncCore(Func<Task<bool>> func)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.Enter);

            async Task<bool> ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    bool locked = await func().ConfigureAwait(false);
                    if (locked)
                        asyncLocalChanges.Commit();
                    return locked;
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
            return Task.FromResult(true);
        }
    }
}
