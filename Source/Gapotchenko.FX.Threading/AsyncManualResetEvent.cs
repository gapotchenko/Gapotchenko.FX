namespace Gapotchenko.FX.Threading;

// TODO

/// <summary>
/// Represents a synchronization event primitive that, when signaled, must be reset manually.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
public sealed class AsyncManualResetEvent
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
    }

    /// <summary>
    /// Sets the state of the event to signaled,
    /// which allows one or more threads waiting on the event to proceed.
    /// </summary>
    public void Set()
    {
    }

    /// <summary>
    /// Sets the state of the event to non-signaled,
    /// which causes threads to block.
    /// </summary>
    public void Reset()
    {

    }

    /// <summary>
    /// Indicates whether the event is set.
    /// </summary>
    public bool IsSet { get; }

}

