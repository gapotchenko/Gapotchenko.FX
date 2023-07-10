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

    public static void EnqueueDisposeOnCompletion(Task task, IDisposable disposable)
    {
        task.ContinueWith(
            _ => disposable.Dispose(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
