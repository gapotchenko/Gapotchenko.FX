using System;
using System.Diagnostics;

#nullable enable

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides a strategy which delays the execution of an action until its explicitly asserted with <see cref="EnsureExecuted"/> method.
    /// </summary>
    /// <remarks>
    /// <see cref="LazyExecution"/> is not thread-safe.
    /// For thread-safe lazy execution, please use <see cref="Threading.ExecuteOnce"/> struct.
    /// </remarks>
    [DebuggerDisplay("IsExecuted={IsExecuted}")]
    public struct LazyExecution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyExecution"/> struct.
        /// </summary>
        /// <param name="action">The action.</param>
        public LazyExecution(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            m_Action = Empty.Nullify(action);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Action? m_Action;

        /// <summary>
        /// Ensures that the action was executed.
        /// </summary>
        public void EnsureExecuted()
        {
            var emptyAction = Fn.Empty;
            if (m_Action != emptyAction)
            {
                m_Action?.Invoke();
                m_Action = emptyAction;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the action was executed.
        /// </summary>
        public bool IsExecuted => m_Action == Fn.Empty;
    }
}
