namespace Gapotchenko.FX.Threading;

struct AsyncMutexCoreImpl
{
    // The implementation is based on a semaphore.
    AsyncSemaphoreCoreImpl m_Semaphore = new(1);

    public AsyncMutexCoreImpl()
    {
    }

    public void Lock(CancellationToken cancellationToken)
    {
        m_Semaphore.Wait(cancellationToken);
    }

    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(timeout, cancellationToken);
    }

    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    public Task LockAsync(CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(cancellationToken);
    }

    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(timeout, cancellationToken);
    }

    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Unlocks the mutex.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot unlock a non-locked mutex.</exception>
    public void Unlock()
    {
        try
        {
            m_Semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            throw new InvalidOperationException("Cannot unlock a non-locked mutex.");
        }
    }

    public readonly bool IsLocked => m_Semaphore.CurrentCount != 1;
}
