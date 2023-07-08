// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

// ATTENTION: A holy grail algorithm ahead!
//
// This file contains an implementation of the backbone algorithm that makes
// reentrancy tracking in asynchronous .NET code possible. Before this
// invention, reentrancy tracking was widely considered to be inconceivable in
// asynchronous .NET code because AsyncLocal<T> class only supports the inward
// flow of ambient data.
//
// Needless to say, this whole situation even led to some industry stagnation
// circa 2015-2022 because nobody had enough persistence in solving that
// puzzle. In turn, that led to a plethora of half-baked attempts of cracking
// asynchronous recursion that never really worked - they were either too slow
// (by using StackTrace) or unreliable (by using Task.CurrentId which is prone
// to collisions). That translated to all sorts of problems and pains when
// people were trying to write asynchronous .NET code to solve their business
// needs.
//
// In contrast, this algorithm is fast and mathematically sound making
// primitives like AsyncRecursiveMutex realistically possible in .NET.
//
// The idea behind the algorithm is based on an obvious mathematical property:
// to go from a source state S to a destination state D we apply the state
// modification function F. So, D = F(S). Now, whenever we want to go to
// another destination state D' knowing only a source state S' and the state
// modification function F, we apply the same transform: D' = F(S'). This can
// be repeated again and again, until we apply the changes to all the states
// of interest, thus making the changes equal in all those states as if they
// were propagated naturally. In this way, the barrier of outward state
// propagation imposed by AsyncLocal<T> primitive ceases to exist, enabling
// the existence of algorithms that use not only inward but outward
// propagation of ambient data.

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

partial class ExecutionContextHelper
{
    /// <summary>
    /// Modifies ambient data that is local to a given asynchronous control flow
    /// by automatically replaying the specified action in all control flow branches
    /// to make the changes equivalent in all of them.
    /// </summary>
    /// <remarks>
    /// This approach allows to overcome the limitations imposed by <see cref="AsyncLocal{T}"/> class
    /// which only supports the inward flow of the ambient data.
    /// </remarks>
    /// <param name="action">The <see cref="Action{T}"/> that directly or indirectly modifies an <see cref="AsyncLocal{T}.Value"/> property.</param>
    /// <returns>An <see cref="AsyncLocalModificationOperation"/> instance that can be used to either commit or discard the modification.</returns>
    public static AsyncLocalModificationOperation ModifyAsyncLocal(Action action) =>
        new(action);

    /// <returns>An <see cref="AsyncLocalModificationOperation{T}"/> instance that can be used to either commit or discard the modification.</returns>
    /// <inheritdoc cref="ModifyAsyncLocal(Action)"/>
    public static AsyncLocalModificationOperation<T> ModifyAsyncLocal<T>(Action<T> action) =>
        new(action);

    public abstract class AsyncLocalModificationOperationBase : IDisposable
    {
        protected AsyncLocalModificationOperationBase()
        {
            Debug.Assert(
                m_FlowState.Value is var flowState &&
                (flowState == null || flowState.Operation.m_State is State.Committed or State.Discarded),
                $"{nameof(ModifyAsyncLocal)} does not support recursion - be sure to commit, discard, or dispose the previously created modification operation before creating a new one.");

            m_FlowState.Value = new FlowState(this, false);
        }

        record FlowState(AsyncLocalModificationOperationBase Operation, bool ActionHandled);

        static readonly AsyncLocal<FlowState?> m_FlowState = new(FlowStateChanged);

        static void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState?> args)
        {
            if (!args.ThreadContextChanged)
                return;

            var flowState = args.CurrentValue;
            if (flowState == null)
                return;

            UpdateFlowState(flowState, args.PreviousValue);
        }

        /// <summary>
        /// Updates the flow state using the finite state machine.
        /// </summary>
        /// <param name="currentFlowState">The current flow state.</param>
        /// <param name="previousFlowState">The previous flow state.</param>
        static void UpdateFlowState(FlowState currentFlowState, Optional<FlowState?> previousFlowState = default)
        {
            switch (currentFlowState.Operation.m_State)
            {
                case State.Initialized:
                    // Nothing is ready yet.
                    break;

                case State.Committed:
                    if (!previousFlowState.HasValue || currentFlowState != previousFlowState.Value)
                    {
                        // Propagate the changes to the current control flow branch.
                        var newFlowState = GetNextFlowState(currentFlowState);
                        if (newFlowState != currentFlowState)
                            m_FlowState.Value = newFlowState;
                    }
                    break;

                case State.Discarded:
                    // Delete all existing flow states.
                    m_FlowState.Value = null;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        static void UpdateFlowState_Old(FlowState currentFlowState)
        {
            var newFlowState = GetNextFlowState(currentFlowState);
            if (newFlowState != currentFlowState)
                m_FlowState.Value = newFlowState;
        }

        /// <summary>
        /// Gets a next flow state of the finite state machine.
        /// </summary>
        /// <param name="state">The existing flow state.</param>
        static FlowState? GetNextFlowState(FlowState state)
        {
            // A full FSM (Finite State Machine) coding style is used here,
            // despite the fact that FlowState.ActionHandled = true is never stored in the state
            // (such a state is considered as completed and gets erased with a null value for
            // memory preservation).

            // A low-hanging optimization is not to have ActionHandled field at all,
            // but that would make the code more entangled and harder to maintain.

            bool actionHandled = state.ActionHandled;

            if (!actionHandled)
            {
                // Replay the changes to the current control flow branch.
                state.Operation.ApplyChanges();
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

        // Using volatile access to ensure that a state is always visible.
        volatile State m_State;

        protected void ValidateCommit()
        {
            if (m_State != State.Initialized)
                throw new InvalidOperationException();
        }

        protected void DoCommit()
        {
            Debug.Assert(m_State == State.Initialized);

            m_State = State.Committed;

            // Apply the changes to the current control flow branch.
            if (m_FlowState.Value is not null and var flowState)
                UpdateFlowState(flowState); // either using the flow state
            //else
            //    ApplyChanges(); // or directly
        }

        /// <summary>
        /// Applies the changes to the current state.
        /// </summary>
        void ApplyChanges()
        {
            Debug.Assert(m_State == State.Committed);

            InvokeAction();
        }

        /// <summary>
        /// Executes the state-modifying action.
        /// </summary>
        protected abstract void InvokeAction();

        /// <summary>
        /// Discards the changes.
        /// </summary>
        public void Discard()
        {
            if (m_State != State.Initialized)
                throw new InvalidOperationException();

            DiscardCore();
        }

        void DiscardCore()
        {
            Debug.Assert(m_State == State.Initialized);

            m_State = State.Discarded;

            if (m_FlowState.Value is not null and var flowState)
                UpdateFlowState(flowState);

            ForgetAction();
        }

        /// <summary>
        /// Forgets the state-modifying action for a quicker memory reclamation.
        /// </summary>
        protected abstract void ForgetAction();

        public void Dispose()
        {
            // Discard unless an explicit order was received.
            if (m_State == State.Initialized)
                DiscardCore();
        }
    }

    public sealed class AsyncLocalModificationOperation : AsyncLocalModificationOperationBase
    {
        internal AsyncLocalModificationOperation(Action action)
        {
            Debug.Assert(action != null);
            m_Action = action;
        }

        Action? m_Action;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void Commit()
        {
            ValidateCommit();
            DoCommit();
        }

        protected override void InvokeAction()
        {
            Debug.Assert(m_Action != null);
            m_Action();
        }

        protected override void ForgetAction()
        {
            m_Action = null;
        }
    }

    public sealed class AsyncLocalModificationOperation<T> : AsyncLocalModificationOperationBase
    {
        internal AsyncLocalModificationOperation(Action<T> action)
        {
            Debug.Assert(action != null);
            m_Action = action;
        }

        Action<T>? m_Action;

        // Using volatile access to ensure that a committed value is always visible in the committed state.
        Volatile<T?> m_CommittedValue;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void Commit(T value)
        {
            ValidateCommit();
            m_CommittedValue.Value = value;
            DoCommit();
        }

        protected override void InvokeAction()
        {
            Debug.Assert(m_Action != null);
            m_Action(m_CommittedValue.Value!);
        }

        protected override void ForgetAction()
        {
            m_Action = null;
        }
    }
}
