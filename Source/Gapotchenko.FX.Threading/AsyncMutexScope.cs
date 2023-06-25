using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines a disposable scope of a mutex access to a resource.
/// The scope can be disposed to unlock the mutex.
/// </summary>
public struct AsyncMutexScope : IDisposable
{
    internal AsyncMutexScope(IAsyncMutex? mutex)
    {
        m_Mutex = mutex;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IAsyncMutex? m_Mutex;

    /// <summary>
    /// Gets a value indicating whether the mutex was locked in this scope.
    /// </summary>
    public bool WasLocked => m_Mutex != null;

    /// <summary>
    /// Unlocks the mutex if it was locked in this scope.
    /// </summary>
    public void Dispose()
    {
        m_Mutex?.Unlock();
    }
}
