namespace Gapotchenko.FX.Threading.Tests.Utils;

static class AsyncLockableHelper
{
    public static bool IsLockHeld(IAsyncLockable lockable)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
            return recursiveLockable.IsHeldByCurrentTask;
        else
            return lockable.IsLocked;
    }
}
