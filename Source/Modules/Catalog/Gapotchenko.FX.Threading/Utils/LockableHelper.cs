using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

static class LockableHelper
{
    /// <summary>
    /// Validates that a lock on the lockable synchronization primitive is held by the current thread.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    [StackTraceHidden]
    public static void ValidateLockOwnership(ILockable lockable)
    {
        if (!IsLockHeld(lockable))
            throw new SynchronizationLockException();
    }

    public static bool IsLockHeld(ILockable lockable)
    {
        if (lockable is IRecursiveLockable recursiveLockable)
            return recursiveLockable.IsLockedByCurrentThread;
        else
            return lockable.IsEntered;
    }

    public readonly struct ExitScope(ILockable lockable) : IDisposable
    {
        public void Dispose()
        {
            // Undo the exit.
            Enter(lockable, m_RecursionLevel);
        }

        readonly int m_RecursionLevel = ExitAll(lockable);
    }

    public readonly struct AsyncExitScope(IAsyncLockable lockable)
    {
        public Task DisposeAsync()
        {
            // Undo the exit.
            return EnterAsync(lockable, m_RecursionLevel);
        }

        readonly int m_RecursionLevel = ExitAll(lockable);
    }

    static void Enter(ILockable lockable, int level, CancellationToken cancellationToken = default)
    {
        Debug.Assert(level >= 0);

        switch (lockable)
        {
            case IReentrableLockable reentrableLockable:
                reentrableLockable.Enter(level, cancellationToken);
                break;

            default:
                for (int i = 0; i < level; ++i)
                    lockable.Enter(cancellationToken);
                break;
        }
    }

    static Task EnterAsync(IAsyncLockable lockable, int level, CancellationToken cancellationToken = default)
    {
        Debug.Assert(level >= 0);

        return
            lockable switch
            {
                IAsyncReentrableLockable reentrableLockable => reentrableLockable.EnterAsync(level, cancellationToken),
                _ => BasicImpl()
            };

        async Task BasicImpl()
        {
            for (int i = 0; i < level; ++i)
                await lockable.EnterAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    static int ExitAll(ILockable lockable)
    {
        switch (lockable)
        {
            case IReentrableLockable reentrableLockable:
                return reentrableLockable.ExitAll();

            case IRecursiveLockable recursiveLockable:
                {
                    int recursionLevel = 0;
                    while (recursiveLockable.IsLockedByCurrentThread)
                    {
                        recursiveLockable.Exit();
                        ++recursionLevel;
                    }
                    return recursionLevel;
                }

            default:
                lockable.Exit();
                return 1;
        }
    }
}
