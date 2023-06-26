namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a mutex that supports both synchronous and asynchronous operations.
/// Mutex is a synchronization primitive that ensures that only one thread can access a resource at any given time.
/// </summary>
public interface IAsyncMutex : IAsyncLockable
{
    /// <summary>
    /// Gets a value indicating whether the mutex is locked by a thread.
    /// </summary>
    bool IsLocked { get; }
}
