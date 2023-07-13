// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines a disposable scope of an asynchronous lockable access to a resource.
/// The scope can be disposed to unlock the synchronization primitive.
/// </summary>
public readonly struct AsyncLockableScope : IDisposable
{
    internal AsyncLockableScope(IAsyncLockable? lockable)
    {
        m_Lockable = lockable;
    }

    readonly IAsyncLockable? m_Lockable;

    /// <summary>
    /// Gets a value indicating whether this scope holds a lock on the synchronization primitive.
    /// </summary>
    public bool HasLock => m_Lockable != null;

    /// <summary>
    /// Unlocks the synchronization primitive if this scope holds a lock on it.
    /// </summary>
    public void Dispose()
    {
        m_Lockable?.Unlock();
    }
}
