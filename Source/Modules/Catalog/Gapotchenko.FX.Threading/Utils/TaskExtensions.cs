namespace Gapotchenko.FX.Threading.Utils;

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

    public static Task Finally(this Task task, Action continuationAction) =>
        task.ContinueWith(
            _ => continuationAction(),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
}
