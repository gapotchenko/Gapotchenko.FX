// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides recursion tracking for threads and asynchronous tasks.
/// </summary>
readonly struct AsyncRecursionTracker
{
    public AsyncRecursionTracker()
    {
    }

    /// <summary>
    /// Indicates whether the current recursion level is greater than zero.
    /// </summary>
    public bool IsEntered
    {
        get
        {
            ExecutionContextHelper.AsyncLocalBarrier();
            return m_RecursionLevel.Value != 0;
        }
    }

    /// <summary>
    /// Increases the recursion level.
    /// </summary>
    public void Enter()
    {
        // No async local barrier here because the method call is always preceded
        // by a call of IsEntered property which does the barrier.

        int recursionLevel = m_RecursionLevel.Value + 1;
        if (recursionLevel == 0)
        {
            // Recursion level overflow.
            throw new LockRecursionException("Maximum recursion level has been reached for asynchronous thread synchronization primitives.");
        }

        m_RecursionLevel.Value = recursionLevel;
    }

    /// <summary>
    /// Decreases the recursion level.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the root (zero) recursion level was reached; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="SynchronizationLockException">The thread synchronization primitive is being unlocked without being locked.</exception>
    public bool Exit()
    {
        ExecutionContextHelper.AsyncLocalBarrier();

        int recursionLevel = m_RecursionLevel.Value - 1;
        if (recursionLevel < 0)
        {
            // Recursion level underflow.
            throw new SynchronizationLockException("The thread synchronization primitive is being unlocked without being locked.");
        }

        m_RecursionLevel.Value = recursionLevel;

        return recursionLevel == 0;
    }

    readonly AsyncLocal<int> m_RecursionLevel = new();
}
