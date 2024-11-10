using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tasks;

/// <summary>
/// Specifies the mode of operation for <see cref="DebuggableParallel"/> primitive.
/// </summary>
public enum DebuggableParallelMode
{
    /// <summary>
    /// Instructs <see cref="DebuggableParallel"/> to automatically select sequential or parallel execution mode
    /// based on a current debugger presence.
    /// This is the default mode.
    /// </summary>
    /// <remarks>
    /// The debugger presence is determined using <see cref="Debugger.IsAttached"/> property.
    /// </remarks>
    Auto,

    /// <summary>
    /// Instructs <see cref="DebuggableParallel"/> to always execute operations sequentially.
    /// </summary>
    AlwaysSequential,

    /// <summary>
    /// Instructs <see cref="DebuggableParallel"/> to always execute operations in parallel.
    /// </summary>
    AlwaysParallel
}
