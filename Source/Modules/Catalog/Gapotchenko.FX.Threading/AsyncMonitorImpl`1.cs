// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// This is an infrastructure type that should never be used by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class AsyncMonitorImpl<TMutex> : IAsyncMonitor
    where TMutex : IAsyncLockable
{
    private protected AsyncMonitorImpl(TMutex mutex, AsyncConditionVariableImpl conditionVariable)
    {
        Mutex = mutex;
        m_ConditionVariable = conditionVariable;
    }

    #region IAsyncLockable

    /// <inheritdoc/>
    public void Enter(CancellationToken cancellationToken = default) => Mutex.Enter(cancellationToken);

    /// <inheritdoc/>
    public Task EnterAsync(CancellationToken cancellationToken = default) => Mutex.EnterAsync(cancellationToken);

    /// <inheritdoc/>
    public bool TryEnter() => Mutex.TryEnter();

    /// <inheritdoc/>
    public bool TryEnter(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Mutex.TryEnter(timeout, cancellationToken);

    /// <inheritdoc/>
    public bool TryEnter(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Mutex.TryEnter(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryEnterAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        Mutex.TryEnterAsync(timeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> TryEnterAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        Mutex.TryEnterAsync(millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public void Exit() => Mutex.Exit();

    /// <inheritdoc/>
    public bool IsEntered => Mutex.IsEntered;

    bool ILockable.IsRecursive => Mutex.IsRecursive;

    #endregion

    #region IConditionVariable

    /// <inheritdoc/>
    public void Wait() => Wait(CancellationToken.None);

    /// <inheritdoc/>
    public void Wait(CancellationToken cancellationToken) => m_ConditionVariable.Wait(Mutex, cancellationToken);

    /// <inheritdoc/>
    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Mutex, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.Wait(Mutex, timeout, cancellationToken);

    /// <inheritdoc/>
    public Task WaitAsync() => WaitAsync(CancellationToken.None);

    /// <inheritdoc/>
    public Task WaitAsync(CancellationToken cancellationToken) => m_ConditionVariable.WaitAsync(Mutex, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Mutex, millisecondsTimeout, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        m_ConditionVariable.WaitAsync(Mutex, timeout, cancellationToken);

    /// <inheritdoc/>
    public void Notify() => m_ConditionVariable.Notify(Mutex);

    /// <inheritdoc/>
    public void NotifyAll() => m_ConditionVariable.NotifyAll(Mutex);

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private protected readonly TMutex Mutex;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly AsyncConditionVariableImpl m_ConditionVariable;
}
