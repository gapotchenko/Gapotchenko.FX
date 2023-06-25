namespace Gapotchenko.FX.Threading;

struct AsyncSemaphoreCoreImpl
{
    // The implementation is currently based on SemaphoreSlim provided by .NET BCL.
    // Not sure if this is the best idea because SemaphoreSlim seems to use OS resources (wait handles).
    readonly SemaphoreSlim m_Semaphore;

    public AsyncSemaphoreCoreImpl(int initialCount)
    {
        m_Semaphore = new(initialCount);
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
