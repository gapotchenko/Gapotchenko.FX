// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

readonly struct AsyncSemaphoreImpl
{
    // The implementation is currently based on SemaphoreSlim provided by .NET BCL.
    // Not sure if this is the best idea because SemaphoreSlim seems to use OS resources (wait handles).
    readonly SemaphoreSlim m_Semaphore;

    public AsyncSemaphoreImpl(int initialCount)
    {
        m_Semaphore = new(initialCount);
    }

    public AsyncSemaphoreImpl(int initialCount, int maxCount)
    {
        m_Semaphore = new(initialCount, maxCount);
    }

    public void Wait(CancellationToken cancellationToken)
    {
        m_Semaphore.Wait(cancellationToken);
    }

    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(timeout, cancellationToken);
    }

    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    public Task WaitAsync(CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(cancellationToken);
    }

    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(timeout, cancellationToken);
    }

    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return m_Semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    public void Release()
    {
        m_Semaphore.Release();
    }

    public readonly int CurrentCount => m_Semaphore.CurrentCount;
}
