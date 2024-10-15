﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

readonly struct AsyncRecursiveLockableImpl<TLockable>(TLockable lockable) :
    IAsyncRecursiveLockable
    where TLockable : IAsyncLockable
{
    public void Enter(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Enter(cancellationToken);
        m_RecursionTracker.EnterNoBarrier();
    }

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.EnterNoBarrier);
            var lockable = m_Lockable;

            async Task ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    await lockable.EnterAsync(cancellationToken).ConfigureAwait(false);
                    asyncLocalChanges.Commit();
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current task.
            m_RecursionTracker.EnterNoBarrier();
            return Task.CompletedTask;
        }
    }

    public bool TryEnter() => TryEnter(0, CancellationToken.None);

    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockCore(() => lockable.TryEnter(timeout, cancellationToken));
    }

    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockCore(() => lockable.TryEnter(millisecondsTimeout, cancellationToken));
    }

    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockAsyncCore(() => lockable.TryEnterAsync(timeout, cancellationToken));
    }

    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryLockAsyncCore(() => lockable.TryEnterAsync(millisecondsTimeout, cancellationToken));
    }

    public void Exit()
    {
        if (m_RecursionTracker.Exit())
            m_Lockable.Exit();
    }

    public bool IsEntered => m_Lockable.IsEntered;

    public bool IsLockedByCurrentThread => m_RecursionTracker.IsEntered;

    bool ILockable.IsRecursive => true;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    bool TryLockCore(Func<bool> tryEnterFunc)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            bool locked = tryEnterFunc();
            if (locked)
                m_RecursionTracker.EnterNoBarrier();
            return locked;
        }
        else
        {
            // Already locked by the current thread.
            m_RecursionTracker.EnterNoBarrier();
            return true;
        }
    }

    Task<bool> TryLockAsyncCore(Func<Task<bool>> tryEnterFunc)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.EnterNoBarrier);

            async Task<bool> ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    bool locked = await tryEnterFunc().ConfigureAwait(false);
                    if (locked)
                        asyncLocalChanges.Commit();
                    return locked;
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current task.
            m_RecursionTracker.EnterNoBarrier();
            return Task.FromResult(true);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly TLockable m_Lockable = lockable;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursionTracker m_RecursionTracker = new();
}
