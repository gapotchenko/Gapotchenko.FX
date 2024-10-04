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
public readonly struct LockableScope : IDisposable
{
    internal LockableScope(ILockable? lockable)
    {
        m_Lockable = lockable;
    }

    /// <summary>
    /// Gets a value indicating whether the scope holds a lock on the synchronization primitive.
    /// </summary>
    public bool HasLock => m_Lockable != null;

    /// <summary>
    /// Unlocks the synchronization primitive if the scope holds a lock on it.
    /// </summary>
    public void Dispose()
    {
        m_Lockable?.Exit();
    }

    readonly ILockable? m_Lockable;
}
