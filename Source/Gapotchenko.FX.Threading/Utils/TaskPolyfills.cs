#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_TASK_ISCOMPLETEDSUCCESSFULLY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Utils;

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
