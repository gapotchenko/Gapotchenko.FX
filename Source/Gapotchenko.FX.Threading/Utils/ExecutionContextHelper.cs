// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023
//
// This file contains an implementation of the backbone algorithm that makes
// reentrancy tracking in asynchronous .NET code possible. Before that, the
// tricks like that were thought to be impossible in .NET because
// AsyncLocal<T> class only supports the inward flow of ambient data.
//
// Needless to say that this whole situation led to the industry stagnation
// circa 2015-2023 because nobody had enough persistence in solving that
// puzzle. In turn, that led to a plethora of half-baked attempts in cracking
// asynchronous recursion that never really worked - they were either too slow
// (by using StackTrace) or unreliable (by using Task.CurrentId which is prone
// to collisions).

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
    public static AsyncLocalModificationContext ModifyAsyncLocal(Action action) =>
        new(action);

    /// <inheritdoc cref="ModifyAsyncLocal(Action)"/>
    public static AsyncLocalModificationContext<TValue> ModifyAsyncLocal<TValue>(Action<TValue> action) =>
        new(action);

    public abstract class AsyncLocalModificationContextBase
    {
        protected AsyncLocalModificationContextBase()
        {
            Debug.Assert(
                m_FlowState.Value is var flowState &&
                (flowState == null || flowState.Context.m_State is State.Committed or State.Discarded),
                $"{nameof(ModifyAsyncLocal)} does not support recursion - be sure to Commit or Discard the previously created context before creating a new one.");

            m_FlowState.Value = new FlowState(this, false);
        }

        record FlowState(AsyncLocalModificationContextBase Context, bool ActionHandled);

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

        protected void ValidateCommit()
        {
            if (m_State != State.Initialized)
                throw new InvalidOperationException();
        }

        protected void DoCommit()
        {
            m_State = State.Committed;

            // Apply the changes to the current control flow branch.
            if (m_FlowState.Value is not null and var flowState)
                UpdateFlowState(flowState); // either using the flow state
            else
                DoAction(); // or directly
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

        protected virtual void DoAction()
        {
            Debug.Assert(m_State == State.Committed);
        }
    }

    public sealed class AsyncLocalModificationContext : AsyncLocalModificationContextBase
    {
        internal AsyncLocalModificationContext(Action action)
        {
            m_Action = action;
        }

        readonly Action m_Action;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void Commit()
        {
            ValidateCommit();
            DoCommit();
        }

        protected override void DoAction()
        {
            base.DoAction();
            m_Action();
        }
    }

    public sealed class AsyncLocalModificationContext<TValue> : AsyncLocalModificationContextBase
    {
        internal AsyncLocalModificationContext(Action<TValue> action)
        {
            m_Action = action;
        }

        readonly Action<TValue> m_Action;

        // Using volatile access to ensure that a committed value is always visible in the committed state.
        Volatile<TValue?> m_CommittedValue;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void Commit(TValue value)
        {
            ValidateCommit();
            m_CommittedValue.Value = value;
            DoCommit();
        }

        protected override void DoAction()
        {
            base.DoAction();
            m_Action(m_CommittedValue.Value!);
        }
    }
}
