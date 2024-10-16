using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableExtensions
{
    public static LockableHelper.RecursiveScope EnterScopeRecursively(this ILockable lockable, int recursionLevel)
    {
        ValidateRecursion(lockable, recursionLevel);

        return LockableHelper.RecursiveScope.Create(() => lockable.EnterScope(), recursionLevel);
    }

    public static Task<LockableHelper.RecursiveScope> EnterScopeRecursivelyAsync(this IAsyncLockable lockable, int recursionLevel)
    {
        ValidateRecursion(lockable, recursionLevel);

        return LockableHelper.RecursiveScope.CreateAsync(() => lockable.EnterScopeAsync(), recursionLevel);
    }

    [StackTraceHidden]
    static void ValidateRecursion(ILockable lockable, int recursionLevel)
    {
        if (recursionLevel > 1 && !lockable.IsRecursive)
            throw new ArgumentException("The synchronization primitive does not support recursion.", nameof(lockable));
    }
}
