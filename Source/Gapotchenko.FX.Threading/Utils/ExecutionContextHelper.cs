using Gapotchenko.FX.Collections.Concurrent;

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

            m_FlowTracker =
                new AsyncLocal<FlowState>(FlowStateChanged)
                {
                    Value = new FlowState(true, false)
                };
        }

        readonly Action<TValue> m_Action;
        readonly AsyncLocal<FlowState> m_FlowTracker;

        record struct FlowState(bool IsActive, bool ActionHandled);

        void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState> args)
        {
            if (args.ThreadContextChanged &&
                m_Committed &&
                args.CurrentValue != args.PreviousValue)
            {
                var state = args.CurrentValue;
                if (state.IsActive)
                    UpdateFlowState(state);
            }
        }

        void UpdateFlowState(in FlowState state)
        {
            var newState = TickFlowState(state);
            if (newState != state) // avoid boxing if the state hasn't been changed
                m_FlowTracker.Value = newState;
        }

        FlowState TickFlowState(FlowState state)
        {
            if (!state.ActionHandled)
            {
                m_Action(m_CommittedValue.Value!);
                state.ActionHandled = true;
            }

            if (state.ActionHandled)
            {
                state = default;
                m_GCRoots.TryRemove(this);
            }

            return state;
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
            if (state.IsActive)
                UpdateFlowState(state);
            else
            {
                m_Action(value);
                m_GCRoots.Add(this);
            }

            m_Committed = true;
        }

        static ConcurrentHashSet<AsyncLocalModificationContext<TValue>> m_GCRoots = new();
    }
}
