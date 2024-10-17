namespace Gapotchenko.FX.Threading.Tests.Utils;

static class TaskExtensions
{
    public static Task<TResult> Then<TResult>(this Task task, Func<TResult> continuationFunction) =>
        task.ContinueWith(
            _ => continuationFunction(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);

    public static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task, Func<TResult, TNewResult> continuationFunction) =>
        task.ContinueWith(
            task => continuationFunction(task.Result),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
}
