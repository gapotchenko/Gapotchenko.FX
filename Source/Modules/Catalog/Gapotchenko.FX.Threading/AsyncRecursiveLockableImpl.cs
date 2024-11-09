// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

readonly struct AsyncRecursiveLockableImpl<TLockable>(TLockable lockable) :
    IAsyncRecursiveLockable,
    IAsyncReentrableLockable
    where TLockable : IAsyncLockable
{
    public void Enter(CancellationToken cancellationToken) => EnterCore(1, cancellationToken);

    public void Enter(int recursionLevel, CancellationToken cancellationToken)
    {
        if (recursionLevel < 0)
            throw new ArgumentOutOfRangeException(nameof(recursionLevel), "The value cannot be negative.");

        if (recursionLevel != 0)
            EnterCore(recursionLevel, cancellationToken);
    }

    public Task EnterAsync(CancellationToken cancellationToken) => EnterAsyncCore(1, cancellationToken);

    public Task EnterAsync(int recursionLevel, CancellationToken cancellationToken)
    {
        if (recursionLevel < 0)
            throw new ArgumentOutOfRangeException(nameof(recursionLevel), "The value cannot be negative.");

        if (recursionLevel == 0)
            return Task.CompletedTask;
        else
            return EnterAsyncCore(recursionLevel, cancellationToken);
    }

    public bool TryEnter() => TryEnter(0, CancellationToken.None);

    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryEnterCore(() => lockable.TryEnter(timeout, cancellationToken));
    }

    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryEnterCore(() => lockable.TryEnter(millisecondsTimeout, cancellationToken));
    }

    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryEnterAsyncCore(() => lockable.TryEnterAsync(timeout, cancellationToken));
    }

    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        var lockable = m_Lockable;
        return TryEnterAsyncCore(() => lockable.TryEnterAsync(millisecondsTimeout, cancellationToken));
    }

    public void Exit()
    {
        if (m_RecursionTracker.Exit())
            m_Lockable.Exit();
    }

    public int ExitAll()
    {
        int level = m_RecursionTracker.ExitAll();
        if (level > 0)
            m_Lockable.Exit();
        return level;
    }

    public bool IsEntered => m_Lockable.IsEntered;

    public bool IsLockedByCurrentThread => m_RecursionTracker.IsEntered;

    bool ILockable.IsRecursive => true;

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    void EnterCore(int recursionLevel, CancellationToken cancellationToken)
    {
        Debug.Assert(recursionLevel >= 0);

        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Enter(cancellationToken);
        m_RecursionTracker.EnterNoBarrier(recursionLevel);
    }

    Task EnterAsyncCore(int recursionLevel, CancellationToken cancellationToken)
    {
        Debug.Assert(recursionLevel >= 0);

        if (!m_RecursionTracker.IsEntered)
        {
            var @this = this;
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocalBy(() => @this.m_RecursionTracker.EnterNoBarrier(recursionLevel));

            async Task ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    await @this.m_Lockable.EnterAsync(cancellationToken).ConfigureAwait(false);
                    asyncLocalChanges.Apply();
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current task.
            m_RecursionTracker.EnterNoBarrier(recursionLevel);
            return Task.CompletedTask;
        }
    }

    bool TryEnterCore(Func<bool> tryEnterFunc)
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

    Task<bool> TryEnterAsyncCore(Func<Task<bool>> tryEnterFunc)
    {
        if (!m_RecursionTracker.IsEntered)
        {
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocalBy(m_RecursionTracker.EnterNoBarrier);

            async Task<bool> ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    bool locked = await tryEnterFunc().ConfigureAwait(false);
                    if (locked)
                        asyncLocalChanges.Apply();
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
