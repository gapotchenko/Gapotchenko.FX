namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableExtensions
{
    public static LockableHelper.RecursiveScope EnterScopeRecursively(this ILockable lockable, int recursionLevel) =>
        LockableHelper.RecursiveScope.Create(() => lockable.EnterScope(), recursionLevel);

}
