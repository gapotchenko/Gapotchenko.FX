namespace Gapotchenko.FX.Threading;

#if NETFRAMEWORK || NETSTANDARD || !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8603
#endif

/// <summary>
/// Provides recursion tracking for threads and asynchronous tasks.
/// </summary>
readonly struct AsyncRecursionTracker
{
    public AsyncRecursionTracker()
    {
    }

    readonly AsyncLocal<int> m_RecursionLevel = new();

    /// <summary>
    /// Indicates whether the current recursion level is root.
    /// </summary>
    public bool IsRoot => m_RecursionLevel.Value == 0;

    /// <summary>
    /// Increases the recursion level.
    /// </summary>
    public void Enter()
    {
        ++m_RecursionLevel.Value;
    }

    /// <summary>
    /// Decreases the recursion level.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the root recursion level was reached; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">Unbalanced lock/unlock acquisitions of a thread synchronization primitive.</exception>
    public bool Leave()
    {
        int recursionLevel = m_RecursionLevel.Value - 1;
        if (recursionLevel < 0)
        {
            // Recursion level underflow.
            throw new InvalidOperationException("Unbalanced lock/unlock acquisitions of a thread synchronization primitive.");
        }

        m_RecursionLevel.Value = recursionLevel;

        return recursionLevel == 0;
    }
}
