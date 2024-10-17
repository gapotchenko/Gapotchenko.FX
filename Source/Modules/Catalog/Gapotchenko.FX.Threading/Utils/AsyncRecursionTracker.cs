// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

/// <summary>
/// Provides recursion tracking for threads and asynchronous tasks.
/// </summary>
readonly struct AsyncRecursionTracker()
{
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

#if UNUSED
    /// <summary>
    /// Increases the recursion level.
    /// </summary>
    public void Enter()
    {
        ExecutionContextHelper.AsyncLocalBarrier();
        EnterNoBarrier();
    }
#endif

    /// <summary>
    /// Increases the recursion level
    /// without issuing <see cref="ExecutionContextHelper.AsyncLocalBarrier"/>.
    /// </summary>
    public void EnterNoBarrier() => EnterNoBarrier(1);

    /// <summary>
    /// Increases the recursion level by the specified value
    /// without issuing <see cref="ExecutionContextHelper.AsyncLocalBarrier"/>.
    /// </summary>
    /// <param name="level">The value by which to increase the recursion level.</param>
    public void EnterNoBarrier(int level)
    {
        Debug.Assert(level >= 0);

        int existingRecursionLevel = m_RecursionLevel.Value;
        int newRecursionLevel = existingRecursionLevel + level;
        if (newRecursionLevel < existingRecursionLevel)
        {
            // Recursion level overflow.
            throw new LockRecursionException("Maximum recursion level has been reached for asynchronous thread synchronization primitives.");
        }

        m_RecursionLevel.Value = newRecursionLevel;
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

    /// <summary>
    /// Decreases the recursion level to zero.
    /// </summary>
    /// <returns>
    /// The recursion level before the decrease.
    /// </returns>
    public int ExitAll()
    {
        ExecutionContextHelper.AsyncLocalBarrier();

        int recursionLevel = m_RecursionLevel.Value;
        m_RecursionLevel.Value = 0;
        return recursionLevel;
    }

    readonly AsyncLocal<int> m_RecursionLevel = new();
}
