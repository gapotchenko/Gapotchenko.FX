namespace Gapotchenko.FX.Threading.Tests.Utils;

static partial class LockableHelper
{
    public static int GetLockDepth(ILockable lockable)
    {
        int depth = UnwindLock(lockable);

        // Reenter the lock.
        for (int i = 0; i < depth; ++i)
            lockable.Enter();

        return depth;
    }

    public static async Task<int> GetLockDepthAsync(IAsyncLockable lockable)
    {
        int depth = UnwindLock(lockable);

        // Reenter the lock.
        for (int i = 0; i < depth; ++i)
            await lockable.EnterAsync().ConfigureAwait(false);

        return depth;
    }

    static int UnwindLock(ILockable lockable)
    {
        int depth = 0;

        // Exit the lock until it unlocks.
        while (IsLockHeld(lockable))
        {
            lockable.Exit();
            ++depth;
        }

        return depth;
    }

    public static bool IsLockHeld(ILockable lockable)
    {
        if (lockable is IRecursiveLockable recursiveLockable)
            return recursiveLockable.IsLockedByCurrentThread;
        else
            return lockable.IsEntered;
    }

    public static void Exit(ILockable lockable, int recursionLevel)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(lockable);
        ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

        for (int i = 0; i < recursionLevel; ++i)
            lockable.Exit();
    }
}
