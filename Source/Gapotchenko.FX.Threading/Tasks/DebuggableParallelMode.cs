using System;

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Defines modes of operation for <see cref="DebuggableParallel"/> primitive.
/// </summary>
public enum DebuggableParallelMode
{
    /// <summary>
    /// The sequential or parallel execution is automatically selected based on presence of an attached debugger.
    /// This is the default mode.
    /// </summary>
    Auto,

    /// <summary>
    /// <see cref="DebuggableParallel"/> always executes operations sequentially.
    /// </summary>
    AlwaysSequential,

    /// <summary>
    /// <see cref="DebuggableParallel"/> always executes operations in parallel.
    /// </summary>
    AlwaysParallel
}
