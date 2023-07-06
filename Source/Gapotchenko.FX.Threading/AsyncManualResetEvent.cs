// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

// TODO

/// <summary>
/// Represents a synchronization primitive that, when signaled, allows one or more threads waiting on it to proceed.
/// <see cref="AsyncManualResetEvent"/> must be reset manually.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncManualResetEvent : IAsyncEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncManualResetEvent"/> class
    /// with an initial state of non-signaled.
    /// </summary>
    public AsyncManualResetEvent()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncManualResetEvent"/> class
    /// with a Boolean value indicating whether to set the initial state to signaled.
    /// </summary>
    /// <param name="initialState">
    /// <see langword="true"/> to set the initial state to signaled;
    /// <see langword="false"/> to set the initial state to non-signaled.
    /// </param>
    public AsyncManualResetEvent(bool initialState)
    {
        m_State = initialState ? 1 : 0;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile int m_State;

    /// <inheritdoc/>
    public void Set()
    {
        if (Interlocked.Exchange(ref m_State, 1) == 0)
        {
            // TODO
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        m_State = 0;
    }

    /// <inheritdoc/>
    public bool IsSet => m_State != 0;

    bool IAsyncEvent.IsAutoReset => false;
}

