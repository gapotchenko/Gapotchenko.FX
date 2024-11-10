// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

/// <summary>
/// The interface of a recursive lockable synchronization primitive
/// that can be entirely exited and then reentered.
/// The primitive supports both synchronous and asynchronous operations.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IAsyncReentrableLockable : IAsyncRecursiveLockable, IReentrableLockable
{
    /// <summary>
    /// Asynchronously enters the lock the specified number of times, that is, recursively,
    /// waiting if necessary until the lock can be entered.
    /// </summary>
    /// <inheritdoc cref="IReentrableLockable.Enter(int, CancellationToken)"/>
    Task EnterAsync(int recursionLevel, CancellationToken cancellationToken = default);
}
