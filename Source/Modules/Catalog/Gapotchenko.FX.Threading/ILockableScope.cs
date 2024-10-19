// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Defines the interface of a disposable scope of a lockable access to a resource.
/// The scope can be disposed to unlock the synchronization primitive.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILockableScope : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the scope holds a lock on the synchronization primitive.
    /// </summary>
    public bool HasLock { get; }
}
