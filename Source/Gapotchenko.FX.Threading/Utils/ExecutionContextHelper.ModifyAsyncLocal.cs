// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

// ATTENTION: A holy grail algorithm ahead!
//
//    _________
//   |o^o^o^o^o|
//   {   _!_   }
//    \   !   /
//     `.   .'
//       )=(
//      ( + )
//       ) (
//   .--'   `--.
//   `---------'
//
// This file contains an implementation of the backbone algorithm that makes
// reentrancy tracking in asynchronous .NET code possible. Before this
// invention, reentrancy tracking was widely considered to be inconceivable in
// asynchronous .NET code because AsyncLocal<T> class only supports the inward
// flow of ambient data.
//
// Needless to say, this whole situation even led to some industry downdraft
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
// be repeated over and over, until we apply the changes to all the states
// of interest, thus making the changes in all these states equivalent as if
// they were propagated naturally. In this way, the barrier of outward state
// propagation imposed by AsyncLocal<T> primitive ceases to exist, enabling
// the existence of algorithms that use not only inward but outward
// propagation of ambient data.
//
// Copyright © 2023 Oleksiy Gapotchenko
// Published under MIT license terms and conditions.

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

    /// <summary>
    /// Represents an operation that modifies ambient data associated with an asynchronous control flow.
    /// </summary>
    public abstract class AsyncLocalModificationOperationBase : IDisposable
    {
        protected AsyncLocalModificationOperationBase()
        {
            // Flush a pending operation first, if any.
            var flowState = m_FlowState.Value;
            if (flowState != null)
                UpdateFsm(flowState);

            Debug.Assert(
                m_FlowState.Value is null,
                $"{nameof(ModifyAsyncLocal)} does not support recursion. Before creating a new modification operation, the previously created operation must be committed, discarded, or disposed.");

            m_FlowState.Value = new FlowState(this, false);
        }

        record FlowState(AsyncLocalModificationOperationBase Operation, bool ActionHandled);

        static readonly AsyncLocal<FlowState?> m_FlowState = new(FlowStateChanged);

        static void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState?> args)
        {
            if (args.ThreadContextChanged &&
                args.CurrentValue is not null and var flowState)
            {
                UpdateFsm(flowState);
            }
        }

        /// <summary>
        /// Updates a finite state machine associated with the current asynchronous control flow.
        /// </summary>
        void UpdateFsm()
        {
            var flowState = m_FlowState.Value;
            if (flowState != null)
            {
                if (flowState.Operation != this)
                    return;
                UpdateFsm(flowState);
            }
        }

        /// <summary>
        /// Updates a finite state machine associated with the specified flow state.
        /// </summary>
        /// <param name="currentFlowState">The current flow state.</param>
        static void UpdateFsm(FlowState currentFlowState)
        {
            switch (currentFlowState.Operation.m_State)
            {
                case OperationState.Initialized:
                    // Operation is not completed yet.
                    break;

                case OperationState.Committed:
                    // Propagate the changes to the current control flow branch.
                    var newFlowState = GetNextCommittedFlowState(currentFlowState);
                    if (newFlowState != currentFlowState)
                        m_FlowState.Value = newFlowState;
                    break;

                case OperationState.Discarded:
                    // Delete all existing flow states for the operation.
                    m_FlowState.Value = null;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets a next flow state of a committed operation.
        /// </summary>
        /// <param name="state">The current flow state.</param>
        static FlowState? GetNextCommittedFlowState(FlowState state)
        {
            // A full FSM (Finite State Machine) coding style is used here,
            // despite the fact that FlowState.ActionHandled = true is never stored in the state
            // (such a state is considered as completed and gets erased with a null value for
            // better memory reclamation).

            // A low-hanging optimization is not to have ActionHandled field at all,
            // but that would make the code more entangled and harder to maintain.

            bool actionHandled = state.ActionHandled;

            if (!actionHandled)
            {
                // Replay the changes in the current control flow branch.
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

        enum OperationState
        {
            Initialized,
            Committed,
            Discarded
        }

        // Using volatile access to ensure that a state is always visible.
        volatile OperationState m_State;

        protected void ValidateCommit()
        {
            if (m_State != OperationState.Initialized)
                throw new InvalidOperationException();
        }

        protected void DoCommit()
        {
            ChangeState(OperationState.Committed);
        }

        /// <summary>
        /// Applies the changes to the current state.
        /// </summary>
        void ApplyChanges()
        {
            Debug.Assert(m_State == OperationState.Committed);

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
            if (m_State != OperationState.Initialized)
                throw new InvalidOperationException();

            DoDiscard();
        }

        void DoDiscard()
        {
            ChangeState(OperationState.Discarded);
            ForgetAction();
        }

        /// <summary>
        /// Forgets the state-modifying action for a quicker memory reclamation.
        /// </summary>
        protected abstract void ForgetAction();

        void ChangeState(OperationState state)
        {
            Debug.Assert(state != OperationState.Initialized);
            Debug.Assert(m_State == OperationState.Initialized);

            m_State = state;

            // Notify FSM about the operation state change so it could be acted upon.
            UpdateFsm();
        }

        public void Dispose()
        {
            // Discard the operation unless an explicit order was received.
            if (m_State == OperationState.Initialized)
                DoDiscard();
        }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
            m_CommittedValue = default;
        }
    }
}
