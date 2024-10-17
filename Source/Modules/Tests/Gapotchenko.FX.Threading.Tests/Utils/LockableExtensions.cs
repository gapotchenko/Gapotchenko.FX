using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableExtensions
{
    public static IDisposable EnterScopeRecursively(
        this ILockable lockable,
        int recursionLevel,
        CancellationToken cancellationToken = default)
    {
        ValidateRecursion(lockable, recursionLevel);

        if (lockable is IReentrableLockable reentrableLockable)
            return LockableHelper.RecursiveReentrableScope.Create(reentrableLockable, recursionLevel, cancellationToken);
        else
            return LockableHelper.RecursiveScope.Create(() => lockable.EnterScope(cancellationToken), recursionLevel);
    }

    public static Task<IDisposable> EnterScopeRecursivelyAsync(
        this IAsyncLockable lockable,
        int recursionLevel,
        CancellationToken cancellationToken = default)
    {
        ValidateRecursion(lockable, recursionLevel);

        if (lockable is IAsyncReentrableLockable reentrableLockable)
        {
            return
                LockableHelper.RecursiveReentrableScope.CreateAsync(reentrableLockable, recursionLevel, cancellationToken)
                .Then(x => (IDisposable)x);
        }
        else
        {
            return
                LockableHelper.RecursiveScope.CreateAsync(() => lockable.EnterScopeAsync(cancellationToken), recursionLevel)
                .Then(x => (IDisposable)x);
        }
    }

    [StackTraceHidden]
    static void ValidateRecursion(ILockable lockable, int recursionLevel)
    {
        if (recursionLevel > 1 && !lockable.IsRecursive)
            throw new ArgumentException("The synchronization primitive does not support recursion.", nameof(lockable));
    }
}
