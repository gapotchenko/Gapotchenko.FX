using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Represents a non-reentrant asynchronous critical section
/// that ensures that only one thread can access a resource at any given time.
/// </summary>
public sealed class AsyncCriticalSection
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly SemaphoreSlim m_Semaphore = new(1);

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public void Enter(CancellationToken cancellationToken = default)
    {
        m_Semaphore.Wait(cancellationToken);
    }

    /// <summary>
    /// Tries to enter the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully entered the <see cref="AsyncCriticalSection"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryEnter() => TryEnter(0);

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully entered the <see cref="AsyncCriticalSection"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return m_Semaphore.Wait(timeout, cancellationToken);
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>,
    /// using a 32-bit signed integer that specifies the timeout.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> if the current thread successfully entered the <see cref="AsyncCriticalSection"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        return m_Semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncCriticalSection"/> has been entered.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task EnterAsync(CancellationToken cancellationToken = default)
    {
        return m_Semaphore.WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully entered the <see cref="AsyncCriticalSection"/>,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return m_Semaphore.WaitAsync(timeout, cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>,
    /// using a 32-bit signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of <see langword="true"/> if the current thread successfully entered the <see cref="AsyncCriticalSection"/>,
    /// otherwise with a result of <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        return m_Semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    /// <summary>
    /// Leaves the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot leave a non-entered critical section.</exception>
    public void Leave()
    {
        try
        {
            m_Semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            throw new InvalidOperationException("Cannot leave a non-entered critical section.");
        }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="AsyncCriticalSection"/> is entered by a thread.
    /// </summary>
    public bool IsEntered => m_Semaphore.CurrentCount != 1;

    /// <summary>
    /// Defines a disposable scope of a critical section access to a resource.
    /// The scope can be disposed to leave the critical section.
    /// </summary>
    public readonly struct Scope : IDisposable
    {
        internal Scope(AsyncCriticalSection? criticalSection)
        {
            m_CriticalSection = criticalSection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly AsyncCriticalSection? m_CriticalSection;

        /// <summary>
        /// Gets a value indicating whether the critical section has been entered in this scope.
        /// </summary>
        public bool HasEntered => m_CriticalSection != null;

        /// <summary>
        /// Leaves the critical section.
        /// </summary>
        public void Dispose()
        {
            m_CriticalSection?.Leave();
        }
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope EnterScope(CancellationToken cancellationToken = default)
    {
        Enter(cancellationToken);
        return new Scope(this);
    }

    /// <summary>
    /// Tries to enter the critical section and
    /// returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <returns>
    /// A scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// <see cref="Scope.HasEntered"/> property indicates whether the current thread successfully entered the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    public Scope TryEnterScope() => TryEnterScope(0);

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// <see cref="Scope.HasEntered"/> property indicates whether the current thread successfully entered the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope TryEnterScope(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(TryEnter(timeout, cancellationToken) ? this : null);

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="AsyncCriticalSection"/>,
    /// using a <see cref="TimeSpan"/> that specifies the timeout,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// <see cref="Scope.HasEntered"/> property indicates whether the current thread successfully entered the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public Scope TryEnterScope(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(TryEnter(millisecondsTimeout, cancellationToken) ? this : null);

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <returns>
    /// A task that will complete when the <see cref="AsyncCriticalSection"/> has been entered with a result of the scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> EnterScopeAsync(CancellationToken cancellationToken = default)
    {
        await EnterAsync(cancellationToken).ConfigureAwait(false);
        return new Scope(this);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>,
    /// using a <see cref="TimeSpan"/> to measure the time interval,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan"/> that represents <c>-1</c> milliseconds to wait indefinitely,
    /// or a <see cref="TimeSpan"/> that represents <c>0</c> milliseconds to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// <see cref="Scope.HasEntered"/> property indicates whether the current thread successfully entered the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> TryEnterScopeAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new(await TryEnterAsync(timeout, cancellationToken).ConfigureAwait(false) ? this : null);

    /// <summary>
    /// Asynchronously waits to enter the <see cref="AsyncCriticalSection"/>,
    /// using a 32-bit signed integer to measure the time interval,
    /// and returns a disposable scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite"/> which has the value of <c>-1</c> to wait indefinitely,
    /// or <c>0</c> to try to enter the <see cref="AsyncCriticalSection"/> and return immediately.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that will complete with a result of the scope that can be disposed to leave the <see cref="AsyncCriticalSection"/>.
    /// <see cref="Scope.HasEntered"/> property indicates whether the current thread successfully entered the <see cref="AsyncCriticalSection"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is a negative number other than <c>-1</c>, which represents an infinite timeout.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is greater than <see cref="Int32.MaxValue"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public async Task<Scope> TryEnterScopeAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        new(await TryEnterAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false) ? this : null);
}
