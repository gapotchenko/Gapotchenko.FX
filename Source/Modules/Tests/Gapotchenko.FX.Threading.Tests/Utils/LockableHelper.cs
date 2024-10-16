namespace Gapotchenko.FX.Threading.Tests.Utils;

static class LockableHelper
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

    public readonly struct RecursiveScope : IDisposable
    {
        public static RecursiveScope Create(Func<LockableScope> enterFunc, int recursionLevel)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(enterFunc);
            ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

            var scopes = new LockableScope[recursionLevel];
            for (int i = recursionLevel - 1; i >= 0; --i)
                scopes[i] = enterFunc();
            return new(scopes);
        }

        public static Task<RecursiveScope> CreateAsync(Func<Task<LockableScope>> enterFunc, int recursionLevel)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(enterFunc);
            ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

            var tasks = new Task<LockableScope>[recursionLevel];
            for (int i = recursionLevel - 1; i >= 0; --i)
                tasks[i] = enterFunc();

            return
                Task.WhenAll(tasks)
                .ContinueWith(
                    x => new RecursiveScope(x.Result),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);
        }

        RecursiveScope(LockableScope[]? scopes)
        {
            m_Scopes = Empty.Nullify(scopes);
        }

        public bool HasLock => m_Scopes?[^1].HasLock ?? false;

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
