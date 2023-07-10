// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines a disposable scope of a lockable access to a resource.
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
    /// Gets a value indicating whether the synchronization primitive is locked by this scope.
    /// </summary>
    public bool IsLocked => m_Lockable != null;

    /// <summary>
    /// Unlocks the synchronization primitive if it ші locked by this scope.
    /// </summary>
    public void Dispose()
    {
        m_Lockable?.Unlock();
    }
}
