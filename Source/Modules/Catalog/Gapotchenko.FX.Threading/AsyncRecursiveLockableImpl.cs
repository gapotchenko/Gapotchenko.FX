// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

readonly struct AsyncRecursiveLockableImpl<TLockable>(TLockable lockable) : IAsyncRecursiveLockable
    where TLockable : IAsyncLockable
{
    public void Enter(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Enter(cancellationToken);
        m_RecursionTracker.Enter();
    }

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(m_RecursionTracker.Enter);
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
            // Already locked by the current thread.
            m_RecursionTracker.Enter();
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

    public bool IsLockedByCurrentTask => m_RecursionTracker.IsEntered;

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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly TLockable m_Lockable = lockable;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncRecursionTracker m_RecursionTracker = new();
}
