// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Utils;

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
        m_RecursionLevel.Value = checked(m_RecursionLevel.Value + 1);
    }

    /// <summary>
    /// Decreases the recursion level.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the root recursion level was reached; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="SynchronizationLockException">The thread synchronization primitive is being unlocked without being locked.</exception>
    public bool Leave()
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
}
