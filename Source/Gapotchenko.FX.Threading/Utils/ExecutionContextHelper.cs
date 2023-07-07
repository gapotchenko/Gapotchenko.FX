namespace Gapotchenko.FX.Threading.Utils;

static class ExecutionContextHelper
{
    /// <summary>
    /// Modifies ambient data that is local to a given asynchronous control flow
    /// by replaying the specified action in all control flow branches.
    /// </summary>
    /// <remarks>
    /// The approach allows to overcome the limitations imposed by <see cref="AsyncLocal{T}"/> class
    /// which only supports the inward flow of the ambient data.
    /// </remarks>
    /// <param name="action">The <see cref="Action{T}"/> that directly or indirectly modifies an <see cref="AsyncLocal{T}.Value"/> property.</param>
    public static AsyncLocalModificationContext<TValue> ModifyAsyncLocal<TValue>(Action<TValue> action) =>
        new(action);

    public class AsyncLocalModificationContext<TValue>
    {
        internal AsyncLocalModificationContext(Action<TValue> action)
        {
            m_Action = action;

            m_FlowTracker = new(FlowStateChanged)
            {
                Value = new FlowState(false, this)
            };
        }

        readonly Action<TValue> m_Action;
        readonly AsyncLocal<FlowState?> m_FlowTracker;

        record FlowState(bool ActionHandled, object GCRoot);

        void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState?> args)
        {
            if (!args.ThreadContextChanged)
                return;

            if (m_Committed &&
                args.CurrentValue != args.PreviousValue)
            {
                var state = args.CurrentValue;
                if (state != null)
                    UpdateFlowState(state);
            }
        }

        void UpdateFlowState(FlowState state)
        {
            var newState = TickFlowState(state);
            if (newState != state)
                m_FlowTracker.Value = newState;
        }

        FlowState? TickFlowState(FlowState state)
        {
            bool actionHandled = state.ActionHandled;

            if (!actionHandled)
            {
                m_Action(m_CommittedValue.Value!);
                actionHandled = true;
            }

            FlowState? newState;
            if (actionHandled)
                newState = null;
            else
                newState = state;

            return newState;
        }

        Volatile<TValue?> m_CommittedValue;
        volatile bool m_Committed;

        /// <summary>
        /// Restores the flow of the execution context across synchronous threads to the original state.
        /// </summary>
        public void Commit(TValue value)
        {
            if (m_Committed)
                throw new InvalidOperationException();

            m_CommittedValue.Value = value;

            var state = m_FlowTracker.Value;
            if (state != null)
                UpdateFlowState(state);
            else
                m_Action(value);

            m_Committed = true;
        }
    }
}
