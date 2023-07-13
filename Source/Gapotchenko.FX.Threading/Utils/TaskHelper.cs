// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Threading.Utils;

static partial class TaskHelper
{
    // This is a partial class that has most of its code in the neighboring files.

    /// <summary>
    /// Represents a non-existing task result.
    /// </summary>
    public struct VoidResult
    {
    }

    /// <summary>
    /// Synchronously disposes the specified disposable object when the task completes.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable object that will be disposed when the task completes.</param>
    public static void DisposeOnCompetion(Task task, IDisposable disposable)
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }

    // This is an optimized implementation to avoid boxing of disposable value types.
    /// <inheritdoc cref="DisposeOnCompetion(Task, IDisposable)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DisposeOnCompetion<TDisposable>(Task task, TDisposable disposable)
        where TDisposable : struct, IDisposable
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
