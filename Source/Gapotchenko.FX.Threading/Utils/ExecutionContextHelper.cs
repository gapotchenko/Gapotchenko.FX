// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

static class ExecutionContextHelper
{
    /// <summary>
    /// Modifies ambient data that is local to a given asynchronous control flow
    /// by replaying the specified action in all control flow branches
    /// to make the data equivalent in all of them.
    /// </summary>
    /// <remarks>
    /// This approach allows to overcome the limitations imposed by <see cref="AsyncLocal{T}"/> class
    /// which only supports the inward flow of the ambient data.
    /// </remarks>
    /// <param name="action">The <see cref="Action{T}"/> that directly or indirectly modifies an <see cref="AsyncLocal{T}.Value"/> property.</param>
    public static AsyncLocalModificationContext<TValue> ModifyAsyncLocal<TValue>(Action<TValue> action) =>
        new(action);

    public sealed class AsyncLocalModificationContext<TValue>
    {
        internal AsyncLocalModificationContext(Action<TValue> action)
        {
            m_Action = action;

            Debug.Assert(
                m_FlowState.Value is var flowState &&
                (flowState == null || flowState.Context.m_State is State.Committed or State.Discarded),
                $"{nameof(ModifyAsyncLocal)} does not support recursion.");
            m_FlowState.Value = new FlowState(this, false);
        }

        readonly Action<TValue> m_Action;

        record FlowState(AsyncLocalModificationContext<TValue> Context, bool ActionHandled);

        static readonly AsyncLocal<FlowState?> m_FlowState = new(FlowStateChanged);

        static void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState?> args)
        {
            if (!args.ThreadContextChanged)
                return;

            var flowState = args.CurrentValue;
            if (flowState == null)
                return;

            switch (flowState.Context.m_State)
            {
                case State.Initialized:
                    // Nothing is ready yet.
                    break;

                case State.Committed:
                    if (args.CurrentValue != args.PreviousValue)
                    {
                        // Propagate the changes to the current control flow branch.
                        UpdateFlowState(flowState);
                    }
                    break;

                case State.Discarded:
                    // Delete any existing flow state - they all are discarded.
                    m_FlowState.Value = null;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        static void UpdateFlowState(FlowState state)
        {
            var newState = GetNextFlowState(state);
            if (newState != state)
                m_FlowState.Value = newState;
        }

        static FlowState? GetNextFlowState(FlowState state)
        {
            bool actionHandled = state.ActionHandled;

            if (!actionHandled)
            {
                // Replay the changes to the current control flow branch.
                state.Context.DoAction();
                actionHandled = true;
            }

            FlowState? newState;
            if (actionHandled)
            {
                // All actions are done - no need to hold a flow state anymore.
                newState = null;
            }
            else
            {
                newState = state;
            }

            return newState;
        }

        enum State
        {
            Initialized,
            Committed,
            Discarded
        }

        // Using volatile access to ensure that a committed value is always visible in the committed state.
        volatile State m_State;
        Volatile<TValue?> m_CommittedValue;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void Commit(TValue value)
        {
            if (m_State != State.Initialized)
                throw new InvalidOperationException();

            m_CommittedValue.Value = value;
            m_State = State.Committed;

            // Apply the changes to the current control flow branch.
            if (m_FlowState.Value is not null and var flowState)
                UpdateFlowState(flowState); // either using the flow state
            else
                m_Action(value); // or directly
        }

        /// <summary>
        /// Discards the changes.
        /// </summary>
        public void Discard()
        {
            if (m_State != State.Initialized)
                throw new InvalidOperationException();

            if (m_FlowState.Value is not null)
                m_FlowState.Value = null;

            m_State = State.Discarded;
        }

        void DoAction()
        {
            Debug.Assert(m_State == State.Committed);

            m_Action(m_CommittedValue.Value!);
        }
    }
}
