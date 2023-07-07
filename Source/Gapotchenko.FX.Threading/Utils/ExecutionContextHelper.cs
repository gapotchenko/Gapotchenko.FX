using Gapotchenko.FX.Collections.Concurrent;

namespace Gapotchenko.FX.Threading.Utils;

static class ExecutionContextHelper
{
    /// <summary>
    /// Suppresses the flow of the execution context across synchronous threads if it was not suppressed yet.
    /// </summary>
    public static FlowScope SuppressFlow(Action<bool> action)
    {
        return new(action);
    }

    public class FlowScope
    {
        internal FlowScope(Action<bool> action)
        {
            m_Action = action;

            m_Tracker = new AsyncLocal<FlowState>(Handler)
            {
                Value = new FlowState(true, false)
            };
        }

        readonly Action<bool> m_Action;
        readonly AsyncLocal<FlowState> m_Tracker;

        record struct FlowState(bool Active, bool ActionHandled);

        void Handler(AsyncLocalValueChangedArgs<FlowState> args)
        {
            if (!args.ThreadContextChanged)
                return;

            if (args.CurrentValue.Equals(args.PreviousValue))
                return;

            if (!m_Restored)
                return;

            Apply(args.CurrentValue);
        }

        void Apply(FlowState state)
        {
            if (state.Active)
            {
                if (!state.ActionHandled)
                {
                    m_Action(m_Value);
                    state.ActionHandled = true;
                }

                if (state.ActionHandled)
                {
                    m_GCRoots.TryRemove(this);
                    state = default;
                }

                m_Tracker.Value = state;
            }
        }

        volatile bool m_Restored;
        volatile bool m_Value;

        /// <summary>
        /// Restores the flow of the execution context across synchronous threads to the original state.
        /// </summary>
        public void Restore(bool value)
        {
            if (m_Restored)
                throw new InvalidOperationException();

            m_Value = value;

            var state = m_Tracker.Value;
            if (state.Active)
                Apply(state);
            else
            {
                m_Action(m_Value);
                m_GCRoots.Add(this);
            }

            m_Restored = true;
        }

        static ConcurrentHashSet<FlowScope> m_GCRoots = new();
    }
}
