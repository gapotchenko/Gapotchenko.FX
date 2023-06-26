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

    readonly AsyncLocal<int> m_RecursionCounter = new();

    /// <summary>
    /// Enters the scope.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the first scope was entered; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Enter()
    {
        int recursionCounter = m_RecursionCounter.Value++;
        return recursionCounter == 0;
    }

    /// <summary>
    /// Leaves the scope.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the final scope was left; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">Unbalanced lock/unlock acquisitions of a thread synchronization primitive.</exception>
    public bool Leave()
    {
        int recursionCounter = m_RecursionCounter.Value - 1;
        if (recursionCounter < 0)
            throw new InvalidOperationException("Unbalanced lock/unlock acquisitions of a thread synchronization primitive.");
        m_RecursionCounter.Value = recursionCounter;
        return recursionCounter == 0;
    }
}
