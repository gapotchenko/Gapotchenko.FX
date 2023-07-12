// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

struct AsyncMutexImpl
{
    // The implementation is based on a semaphore.
    AsyncSemaphoreImpl m_Semaphore = new(1, 1);

    public AsyncMutexImpl()
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
    /// <exception cref="SynchronizationLockException">The mutex is being unlocked without being locked.</exception>
    public void Unlock()
    {
        try
        {
            m_Semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            throw new SynchronizationLockException("The mutex is being unlocked without being locked.");
        }
    }

    public readonly bool IsLocked => m_Semaphore.CurrentCount != 1;
}
