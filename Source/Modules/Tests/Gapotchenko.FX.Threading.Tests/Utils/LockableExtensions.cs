namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableExtensions
{
    public static ILockableScope EnterScopeRecursively(
        this ILockable lockable,
        int recursionLevel,
        CancellationToken cancellationToken = default) =>
        recursionLevel switch
        {
            1 => lockable.EnterScope(cancellationToken),
            _ => LockableHelper.RecursiveScope.Enter(lockable, recursionLevel, cancellationToken)
        };

    public static Task<ILockableScope> EnterScopeRecursivelyAsync(
        this IAsyncLockable lockable,
        int recursionLevel,
        CancellationToken cancellationToken = default) =>
        recursionLevel switch
        {
            1 => lockable.EnterScopeAsync(cancellationToken).Then(x => (ILockableScope)x),
            _ => LockableHelper.RecursiveScope.EnterAsync(lockable, recursionLevel, cancellationToken).Then(x => (ILockableScope)x)
        };
}
