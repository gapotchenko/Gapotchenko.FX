// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Diagnostics;

namespace Gapotchenko.FX;

/// <summary>
/// Provides a strategy which delays the execution of an action until it is explicitly asserted with <see cref="EnsureExecuted"/> method.
/// </summary>
/// <remarks>
/// <see cref="LazyExecution"/> is not thread-safe.
/// For thread-safe lazy execution, please use <see cref="Threading.ExecuteOnce"/> structure.
/// </remarks>
[DebuggerDisplay("IsExecuted={IsExecuted}")]
public struct LazyExecution
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyExecution"/> structure.
    /// </summary>
    /// <param name="action">The action.</param>
    public LazyExecution(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        m_Action = Empty.Nullify(action);
    }

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
    public readonly bool IsExecuted => m_Action == Fn.Empty;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Action? m_Action;
}
