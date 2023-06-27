namespace Gapotchenko.FX.Threading;

#if NETFRAMEWORK || NETSTANDARD || !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8603
#endif

/// <summary>
/// Provides the recursion tracking among thread and asynchronous tasks.
/// </summary>
readonly struct AsyncRecursionTracker
{
    public AsyncRecursionTracker()
    {
    }

    readonly AsyncLocal<int> m_RecursionCounter = new();

    public bool IsFirstLevel => m_RecursionCounter.Value == 0;

    /// <summary>
    /// Enters the recursion level.
    /// </summary>
    public void Enter()
    {
        ++m_RecursionCounter.Value;
    }

    /// <summary>
    /// Leaves the recursion level.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the final recursion level was left; otherwise, <see langword="false"/>.
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
