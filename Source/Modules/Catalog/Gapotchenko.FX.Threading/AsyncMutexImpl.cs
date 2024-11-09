// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

readonly struct AsyncMutexImpl : IAsyncMutex
{
    public AsyncMutexImpl()
    {
    }

    public void Enter(CancellationToken cancellationToken)
    {
        m_Semaphore.Wait(cancellationToken);
    }

    public bool TryEnter() => TryEnter(0, CancellationToken.None);

    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(timeout, cancellationToken);
    }

    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    public Task EnterAsync(CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(cancellationToken);
    }

    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(timeout, cancellationToken);
    }

    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Unlocks the mutex.
    /// </summary>
    /// <exception cref="SynchronizationLockException">The mutex is being unlocked without being locked.</exception>
    public void Exit()
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

    public readonly bool IsEntered => m_Semaphore.CurrentCount != 1;

    // The implementation is based on a semaphore.
    readonly AsyncSemaphoreImpl m_Semaphore = new(1, 1);

    bool ILockable.IsRecursive => false;
}
