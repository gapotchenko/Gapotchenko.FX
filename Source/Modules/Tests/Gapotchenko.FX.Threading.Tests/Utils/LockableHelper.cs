namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableHelper
{
    public static bool IsLockHeld(ILockable lockable)
    {
        if (lockable is IRecursiveLockable recursiveLockable)
            return recursiveLockable.IsLockedByCurrentThread;
        else
            return lockable.IsEntered;
    }
}
