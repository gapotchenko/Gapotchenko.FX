using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

static class AsyncLockableHelper
{
    /// <summary>
    /// Validates that a lock on the lockable synchronization primitive is held by the current thread.
    /// </summary>
    /// <param name="lockable">The lockable synchronization primitive.</param>
    /// <exception cref="SynchronizationLockException">Object synchronization method was called from an unsynchronized block of code.</exception>
    [StackTraceHidden]
    public static void ValidateLockOwnership(IAsyncLockable lockable)
    {
        if (!IsLockHeld(lockable))
            throw new SynchronizationLockException();
    }

    public static bool IsLockHeld(IAsyncLockable lockable)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
            return recursiveLockable.LockIsHeldByCurrentTask;
        else
            return lockable.IsEntered;
    }
}
