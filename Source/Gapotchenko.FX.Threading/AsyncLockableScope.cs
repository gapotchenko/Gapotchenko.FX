// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IAsyncLockable? m_Lockable;

    /// <summary>
    /// Gets a value indicating whether the synchronization primitive was locked in this scope.
    /// </summary>
    public bool WasLocked => m_Lockable != null;

    /// <summary>
    /// Unlocks the synchronization primitive if it was locked in this scope.
    /// </summary>
    public void Dispose()
    {
        m_Lockable?.Unlock();
    }
}
