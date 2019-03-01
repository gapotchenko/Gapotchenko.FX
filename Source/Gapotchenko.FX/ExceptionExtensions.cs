using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Exception extensions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Checks whether exception signifies cancellation of a logical execution thread.
        /// There is a predefined set of such exceptions:
        /// <see cref="ThreadInterruptedException"/>, <see cref="ThreadAbortException"/>, <see cref="TaskCanceledException"/> and
        /// <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if exception signifies logical thread cancellation; otherwise, <c>false</c>.</returns>
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
        /// Checks whether exception is intended to affect the control flow of code execution.
        /// There is a predefined set of such exceptions:
        /// all the cancellation exceptions reported by <see cref="IsCancellationException(Exception)"/>, and
        /// <see cref="StackOverflowException"/>.
        /// The list can be semantically extended by deriving a custom exception from <see cref="IControlFlowException"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if exception represents a control flow exception; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool IsControlFlowException(this Exception exception) =>
            IsCancellationException(exception) ||
            exception is StackOverflowException ||
            exception is IControlFlowException;

        /// <summary>
        /// Rethrows a control flow exception if there is any.
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
