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

    public static void EnqueueDisposeOnCompletion(Task task, IDisposable disposable)
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
