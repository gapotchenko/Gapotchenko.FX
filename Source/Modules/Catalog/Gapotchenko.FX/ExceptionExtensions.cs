namespace Gapotchenko.FX;

/// <summary>
/// Exception extensions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExceptionExtensions
{
    /// <summary>
    /// Gets a value indicating whether the exception signifies a thread, task or operation cancellation.
    /// </summary>
    /// <remarks>
    /// There is a predefined set of such exceptions:
    /// <see cref="ThreadInterruptedException"/>, <see cref="ThreadAbortException"/>, <see cref="TaskCanceledException"/>,
    /// <see cref="OperationCanceledException"/>.
    /// </remarks>
    /// <param name="exception">The exception.</param>
    /// <returns>
    /// <see langword="true"/> if the exception signifies a thread or task cancellation;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool IsCancellationException(this Exception exception) =>
        ConsiderAggregation(
            exception ?? throw new ArgumentNullException(nameof(exception)),
            IsCancellationExceptionCore);

    static bool IsCancellationExceptionCore(Exception exception) =>
        exception is
            ThreadAbortException or
            ThreadInterruptedException or
            OperationCanceledException;

    /// <summary>
    /// Gets a value indicating whether the exception is intended to affect the control flow of the code execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is a predefined set of such exceptions:
    /// all the cancellation exceptions reported by <see cref="IsCancellationException(Exception)"/>, and
    /// <see cref="StackOverflowException"/>.
    /// This list can be semantically extended by deriving a custom exception from <see cref="IControlFlowException"/>.
    /// </para>
    /// </remarks>
    /// <param name="exception">The exception.</param>
    /// <returns>
    /// <see langword="true"/> if the exception represents a control flow exception;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool IsControlFlowException(this Exception exception) =>
        ConsiderAggregation(
            exception ?? throw new ArgumentNullException(nameof(exception)),
            IsControlFlowExceptionCore);

    static bool IsControlFlowExceptionCore(Exception exception) =>
        IsCancellationExceptionCore(exception) ||
        exception is StackOverflowException ||
        exception is IControlFlowException;

    static bool ConsiderAggregation(Exception e, Predicate<Exception> p)
    {
        if (p(e))
            return true;

        if (e is AggregateException aggregateException)
        {
            if (aggregateException.InnerExceptions.AnyAndAll(x => ConsiderAggregation(x, p)))
                return true;
        }

        return false;
    }

    static bool AnyAndAll<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            // Sequence is empty.
            return false;
        }

        do
        {
            if (!predicate(enumerator.Current))
                return false;
        }
        while (enumerator.MoveNext());

        return true;
    }

    /// <summary>
    /// Rethrows a control flow exception if it is represented by the exception itself,
    /// or there is one in the chain of its inner exceptions.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static void RethrowControlFlowException(this Exception exception)
    {
        var i = exception ?? throw new ArgumentNullException(nameof(exception));
        do
        {
            if (i.IsControlFlowException())
                throw i;
            i = i.InnerException;
        }
        while (i != null);
    }

    /// <summary>
    /// Returns a collection of nested inner exceptions that caused the current exception.
    /// </summary>
    /// <param name="exception">The current exception.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of nested inner <see cref="Exception"/> values that caused the current exception.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    public static IEnumerable<Exception> InnerExceptions(this Exception exception)
    {
        var i = exception ?? throw new ArgumentNullException(nameof(exception));
        for (; ; )
        {
            i = i.InnerException;
            if (i == null)
                break;
            yield return i;
        }
    }

    /// <summary>
    /// Returns a collection of nested inner exceptions that caused the current exception including self.
    /// </summary>
    /// <param name="exception">The current exception.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of nested inner <see cref="Exception"/> values that caused the current exception including self.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    public static IEnumerable<Exception> SelfAndInnerExceptions(this Exception exception)
    {
        var i = exception ?? throw new ArgumentNullException(nameof(exception));
        do
        {
            yield return i;
            i = i.InnerException;
        }
        while (i != null);
    }
}
