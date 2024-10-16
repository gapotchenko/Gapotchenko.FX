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

    public readonly struct RecursiveScope : IDisposable
    {
        public static RecursiveScope Create(Func<LockableScope> enterFunc, int recursionLevel)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(enterFunc);
            ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

            return new(
                Enumerable.Range(1, recursionLevel)
                .Select(_ => enterFunc())
                .ToArray());
        }

        public static async Task<RecursiveScope> CreateAsync(Func<Task<LockableScope>> enterFunc, int recursionLevel)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(enterFunc);
            ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

            var scopes = new LockableScope[recursionLevel];
            for (int i = 0; i < recursionLevel; ++i)
                scopes[i] = await enterFunc().ConfigureAwait(false);
            return new(scopes);
        }

        RecursiveScope(LockableScope[] scopes)
        {
            m_Scopes = scopes;
        }

        public bool HasLock => m_Scopes?[0].HasLock ?? false;

        public void Dispose()
        {
            var scopes = m_Scopes;
            if (scopes != null)
            {
                foreach (var scope in scopes)
                    scope.Dispose();
            }
        }

        readonly LockableScope[]? m_Scopes;
    }
}
