namespace Gapotchenko.FX.Threading.Utils;

static class ExecutionContextHelpers
{
    /// <summary>
    /// Suppresses the flow of the execution context across synchronous threads if it was not suppressed yet.
    /// </summary>
    public static FlowScope SuppressFlow()
    {
        bool flowWasSuppressed = ExecutionContext.IsFlowSuppressed();
        if (!flowWasSuppressed)
            ExecutionContext.SuppressFlow();
        return new(flowWasSuppressed);
    }

    public readonly struct FlowScope : IDisposable
    {
        internal FlowScope(bool flowWasSuppressed)
        {
            m_FlowWasSuppressed = flowWasSuppressed;
        }

        readonly bool m_FlowWasSuppressed;

        /// <summary>
        /// Restores the flow of the execution context across synchronous threads to the original state.
        /// </summary>
        public void Restore()
        {
            if (!m_FlowWasSuppressed && ExecutionContext.IsFlowSuppressed())
                ExecutionContext.RestoreFlow();
        }

        /// <inheritdoc cref="Restore"/>
        public void Dispose() => Restore();
    }
}
