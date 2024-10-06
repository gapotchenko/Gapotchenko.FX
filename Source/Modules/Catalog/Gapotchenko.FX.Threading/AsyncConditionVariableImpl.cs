// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © Stephen Cleary
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Utils;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading;

readonly struct AsyncConditionVariableImpl
{
    // ----------------------------------------------------------------------
    // Public Facade
    // ----------------------------------------------------------------------

    public AsyncConditionVariableImpl()
    {
    }

    public void Notify(IAsyncLockable lockable)
    {
        ValidateLockable(lockable);

        lock (m_Queue.SyncRoot)
            m_Queue.TryDeque(true);
    }

    /// <inheritdoc/>
    public void NotifyAll(IAsyncLockable lockable)
    {
        ValidateLockable(lockable);

        lock (m_Queue.SyncRoot)
            m_Queue.DequeAll(true);
    }

    public void Wait(IAsyncLockable lockable, CancellationToken cancellationToken)
    {
        ValidateLockable(lockable);

        DoWait(lockable, cancellationToken);
    }

    public bool Wait(IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        ValidateLockable(lockable);

        return DoWait(lockable, TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    public bool Wait(IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        ValidateLockable(lockable);

        return DoWait(lockable, timeout, cancellationToken);
    }

    public Task WaitAsync(IAsyncLockable lockable, CancellationToken cancellationToken)
    {
        ValidateLockable(lockable);

        return DoWaitAsync(lockable, cancellationToken);
    }

    public Task<bool> WaitAsync(IAsyncLockable lockable, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        ExceptionHelper.ValidateTimeoutArgument(millisecondsTimeout);

        ValidateLockable(lockable);

        return DoWaitAsync(lockable, TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
    }

    public Task<bool> WaitAsync(IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ExceptionHelper.ValidateTimeoutArgument(timeout);

        ValidateLockable(lockable);

        return DoWaitAsync(lockable, timeout, cancellationToken);
    }

    // ----------------------------------------------------------------------
    // Core Implementation
    // ----------------------------------------------------------------------

    bool DoWait(IAsyncLockable lockable, CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSourceHelper.CreateLinked(cancellationToken);
        try
        {
            var waitHandle = AllocateWaitHandle(cts.Token);

            lockable.Exit();
            try
            {
                return TaskBridge.Execute(waitHandle);
            }
            finally
            {
                lockable.Enter(CancellationToken.None);
            }
        }
        finally
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    bool DoWait(IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        if (timeout == Timeout.InfiniteTimeSpan)
            return DoWait(lockable, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        if (timeout == TimeSpan.Zero)
            return false;

        var cts = new CancellationTokenSource();
        try
        {
            var waitHandle = AllocateWaitHandle(cts.Token);

            lockable.Exit();
            try
            {
                return TaskBridge.Execute(
                    ct =>
                    {
                        return TaskHelper.ExecuteWithTimeout(
                            async ct =>
                            {
                                using (ct.Register(cts.Cancel))
                                    return await waitHandle.ConfigureAwait(false);
                            },
                            timeout,
                            false,
                            ct);
                    },
                    cancellationToken);
            }
            finally
            {
                lockable.Enter(CancellationToken.None);
            }
        }
        finally
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    Task<bool> DoWaitAsync(IAsyncLockable lockable, CancellationToken cancellationToken)
    {
        var waitHandle = AllocateWaitHandleAsync(cancellationToken);

        async Task<bool> ExecuteAsync()
        {
            lockable.Exit();
            try
            {
                return await waitHandle.ConfigureAwait(false);
            }
            finally
            {
                await lockable.EnterAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        return ExecuteAsync();
    }

    Task<bool> DoWaitAsync(IAsyncLockable lockable, TimeSpan timeout, CancellationToken cancellationToken)
    {
        Debug.Assert(ExceptionHelper.IsValidTimeout(timeout));

        if (timeout == TimeSpan.Zero)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            return Task.FromResult(false);
        }
        else
        {
            var @this = this;
            return TaskHelper.ExecuteWithTimeout(
                ct => @this.DoWaitAsync(lockable, ct),
                timeout,
                false,
                cancellationToken);
        }
    }

    Task<bool> AllocateWaitHandle(CancellationToken cancellationToken)
    {
        Task<bool> result;

#if TFF_CER
        // Execute the chunk of work related to an asynchronous task in a constrained region
        // because the task cannot interact with a task scheduler after the thread is aborted.
        RuntimeHelpers.PrepareConstrainedRegionsNoOP();
        try
        {
        }
        finally
#endif
        {
            result = AllocateWaitHandleAsync(cancellationToken);
        }

        return result;
    }

    Task<bool> AllocateWaitHandleAsync(CancellationToken cancellationToken)
    {
        lock (m_Queue.SyncRoot)
            return m_Queue.Enqueue(cancellationToken);
    }

    readonly AsyncWaitQueue<bool> m_Queue = new();

    [StackTraceHidden]
    void ValidateLockable(IAsyncLockable lockable)
    {
        LockableHelper.ValidateLockOwnership(lockable);
    }
}
