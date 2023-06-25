namespace Gapotchenko.FX.Threading;

#if NETFRAMEWORK || NETSTANDARD || !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8603
#endif

/// <summary>
/// Provides the recursion tracking among thread and asynchronous tasks.
/// </summary>
struct AsyncRecursionTracker
{
    public AsyncRecursionTracker()
    {
    }

    readonly AsyncLocal<int> m_AsyncRecursionCounter = new();

    /// <summary>
    /// Enters the scope.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the first scope was entered; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Enter()
    {
        int recursionCounter = m_AsyncRecursionCounter.Value++;
        return recursionCounter == 0;
    }

    /// <summary>
    /// Leaves the scope.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the final scope was left; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="LockRecursionException">Unbalanced lock/unlock acquisitions of a thread synchronization primitive.</exception>
    public bool Leave()
    {
        int recursionCounter = m_AsyncRecursionCounter.Value - 1;
        if (recursionCounter < 0)
            throw new LockRecursionException("Unbalanced lock/unlock acquisitions of a thread synchronization primitive.");
        m_AsyncRecursionCounter.Value = recursionCounter;
        return recursionCounter == 0;
    }
}
