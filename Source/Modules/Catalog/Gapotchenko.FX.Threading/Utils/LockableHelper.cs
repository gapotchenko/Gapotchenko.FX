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
            for (int i = 0; i < m_RecursionLevel; ++i)
                lockable.Enter();
        }

        readonly int m_RecursionLevel = ExitAll(lockable);
    }

    public readonly struct AsyncExitScope(IAsyncLockable lockable)
    {
        public async Task DisposeAsync()
        {
            // Undo the exit.
            for (int i = 0; i < m_RecursionLevel; ++i)
                await lockable.EnterAsync().ConfigureAwait(false);
        }

        readonly int m_RecursionLevel = ExitAll(lockable);
    }

    static int ExitAll(ILockable lockable)
    {
        if (lockable is IRecursiveLockable recursiveLockable)
        {
            int recursionLevel = 0;
            do
            {
                recursiveLockable.Exit();
                ++recursionLevel;
            }
            while (recursiveLockable.IsLockedByCurrentThread);
            return recursionLevel;
        }
        else
        {
            lockable.Exit();
            return 1;
        }
    }
}
