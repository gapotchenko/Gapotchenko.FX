// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_TASK_ISCOMPLETEDSUCCESSFULLY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Utils;

/// <summary>
/// Provides internal polyfill methods for <see cref="Task"/>.
/// </summary>
static class TaskPolyfills
{
#if TFF_TASK_ISCOMPLETEDSUCCESSFULLY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool IsCompletedSuccessfully(this Task task) =>
#if TFF_TASK_ISCOMPLETEDSUCCESSFULLY
        task.IsCompletedSuccessfully;
#else
        task.Status == TaskStatus.RanToCompletion;
#endif
}
