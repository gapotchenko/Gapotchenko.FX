using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Exception extensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExceptionExtensions
    {
        /// <summary>
        /// <para>
        /// Checks whether exception signifies a cancellation of a thread or task.
        /// </para>
        /// <para>
        /// There is a predefined set of such exceptions:
        /// <see cref="ThreadInterruptedException"/>, <see cref="ThreadAbortException"/>, <see cref="TaskCanceledException"/> and
        /// <see cref="OperationCanceledException"/>.
        /// </para>
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if exception signifies a cancellation of a thread or task; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool IsCancellationException(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return
                exception is ThreadAbortException ||
                exception is ThreadInterruptedException ||
                exception is OperationCanceledException ||
                exception is TaskCanceledException;
        }

        /// <summary>
        /// <para>
        /// Checks whether exception is intended to affect the control flow of code execution.
        /// </para>
        /// <para>
        /// There is a predefined set of such exceptions:
        /// all the cancellation exceptions reported by <see cref="IsCancellationException(Exception)"/>, and
        /// <see cref="StackOverflowException"/>.
        /// </para>
        /// <para>
        /// The list can be semantically extended by deriving a custom exception from <see cref="IControlFlowException"/>.
        /// </para>
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if exception represents a control flow exception; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool IsControlFlowException(this Exception exception) =>
            IsCancellationException(exception) ||
            exception is StackOverflowException ||
            exception is IControlFlowException;

        /// <summary>
        /// Rethrows a control flow exception if it is represented by the exception itself, or there is any in a chain of its inner exceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
}
