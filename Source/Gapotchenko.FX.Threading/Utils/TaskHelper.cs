// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

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

    [Conditional("TFF_THREAD_ABORT")]
    public static void ClearThreadAbort()
    {
#if TFF_THREAD_ABORT
        try
        {
            // Allow the task to finish gracefully.
            Thread.ResetAbort();
        }
        catch (ThreadStateException)
        {
            // Was not aborted with Thread.Abort().
        }
        catch (PlatformNotSupportedException)
        {
            // Not supported.
        }
#endif
    }

    /// <summary>
    /// Disposes the specified disposable object when the task completes.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable object that will be disposed when the task completes.</param>
    public static void ContinueWithDispose(Task task, IDisposable disposable)
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }

    // This optimized implementation helps to avoid boxing of value types.
    /// <inheritdoc cref="ContinueWithDispose(Task, IDisposable)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ContinueWithDispose<TDisposable>(Task task, TDisposable disposable)
        where TDisposable : struct, IDisposable
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
