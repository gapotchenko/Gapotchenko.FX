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
    public void Enter(CancellationToken cancellationToken)
    {
        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Enter(cancellationToken);
        m_RecursionTracker.EnterNoBarrier();
    }

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken) => EnterAsyncCore(1, cancellationToken);

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

    public void Reenter(int level)
    {
        if (level < 0)
            throw new ArgumentOutOfRangeException(nameof(level), "The value cannot be negative.");

        if (level == 0)
            return;

        if (!m_RecursionTracker.IsEntered)
            m_Lockable.Enter();
        m_RecursionTracker.EnterNoBarrier(level);
    }

    public Task ReenterAsync(int level)
    {
        if (level < 0)
            throw new ArgumentOutOfRangeException(nameof(level), "The value cannot be negative.");

        if (level == 0)
            return Task.CompletedTask;
        else
            return EnterAsyncCore(level);
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

    Task EnterAsyncCore(int level, CancellationToken cancellationToken = default)
    {
        Debug.Assert(level >= 0);

        if (!m_RecursionTracker.IsEntered)
        {
            var @this = this;
            var asyncLocalChanges = ExecutionContextHelper.ModifyAsyncLocal(() => @this.m_RecursionTracker.EnterNoBarrier(level));

            async Task ExecuteAsync()
            {
                using (asyncLocalChanges)
                {
                    await @this.m_Lockable.EnterAsync(cancellationToken).ConfigureAwait(false);
                    asyncLocalChanges.Commit();
                }
            }

            return ExecuteAsync();
        }
        else
        {
            // Already locked by the current task.
            m_RecursionTracker.EnterNoBarrier(level);
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
