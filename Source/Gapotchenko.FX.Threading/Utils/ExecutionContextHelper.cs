using System.Diagnostics;
using static Gapotchenko.FX.Threading.Utils.ExecutionContextHelper;

namespace Gapotchenko.FX.Threading.Utils;

static class ExecutionContextHelper
{
    /// <summary>
    /// Suppresses the flow of the execution context across synchronous threads if it was not suppressed yet.
    /// </summary>
    public static FlowScope SuppressFlow(Action<bool> action)
    {
        bool flowHandled = ExecutionContext.IsFlowSuppressed();
        //if (!flowHandled)
        //    ExecutionContext.SuppressFlow();
        return new(true, action);
    }

    public class FlowScope
    {
        internal FlowScope(bool flowHandled, Action<bool> action)
        {
            m_Action = action;

            m_Tracker = new AsyncLocal<FlowState>(Handler)
            {
                Value = new FlowState(true, false, flowHandled)
            };
        }

        readonly Action<bool> m_Action;

        record struct FlowState(bool Active, bool ActionHandled, bool FlowHandled);

        readonly AsyncLocal<FlowState>? m_Tracker;

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
                if (!state.FlowHandled)
                {
                    if (ExecutionContext.IsFlowSuppressed())
                    {
                        ExecutionContext.RestoreFlow();
                        state.FlowHandled = true;
                    }
                }

                if (!state.ActionHandled)
                {
                    if (state.FlowHandled)
                    {
                        m_Action(m_Value);
                        state.ActionHandled = true;
                    }
                    else
                    {
                        m_Action(m_Value);
                    }
                }

                if (state.FlowHandled && state.ActionHandled)
                    state = default;

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
            }

            m_Restored = true;
        }
    }
}
