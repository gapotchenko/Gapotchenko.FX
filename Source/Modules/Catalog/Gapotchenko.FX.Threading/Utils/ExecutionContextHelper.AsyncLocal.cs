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
// flow of ambient data that is local to a given asynchronous control flow.
//
// Needless to say, this whole situation even led to some industry downdraft
// circa 2015-2022 because nobody had enough persistence in solving that
// puzzle. In turn, that led to a plethora of half-baked attempts at cracking
// asynchronous recursion that never really worked - they were either too slow
// (by using StackTrace) or unreliable (by using Task.CurrentId which is prone
// to collisions). That translated to all sorts of problems and pains when
// people were trying to write reentrant asynchronous .NET code to solve their
// business needs.
//
// In contrast, the proposed algorithm is fast and mathematically sound,
// making such primitives as recursive mutex realistically possible in
// asynchronous .NET code.
//
// The idea behind the algorithm is based on an obvious mathematical property:
// to go from a source state S to a destination state D we apply the state
// modification function F. So, D = F(S). Now, whenever we want to go to
// another destination state D' knowing only a source state S' and the state
// modification function F, we apply the same transformation: D' = F(S'). The
// process can be repeated over and over, until we apply the changes to all
// the states of interest, thus making the changes in all these states
// equivalent as if they were propagated naturally. In this way, the barrier
// of outward state propagation imposed by AsyncLocal<T> ceases to exist. This
// makes it possible to implement algorithms that use not only inward, but
// also outward propagation of the AsyncLocal<T> data.
//
// (Well yes, this is one of those ideas akin to Münchhausen pulling himself
// out of a mire by his own hair. Or the ideas of time travels proposed by
// various theoretical physicists based on reversing and replaying the changes
// of the universe's state.)
//
// Copyright © 2023 Oleksiy Gapotchenko
// Published under the terms and conditions of MIT License.

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Utils;

partial class ExecutionContextHelper
{
    /// <summary>
    /// Modifies <see cref="AsyncLocal{T}"/> ambient data that is local to a given asynchronous control flow
    /// by automatically replaying the specified action in all control flow branches
    /// to make equivalent <see cref="AsyncLocal{T}"/> data changes in all of them.
    /// </summary>
    /// <remarks>
    /// This approach allows to overcome the limitations imposed by <see cref="AsyncLocal{T}"/> class
    /// which only supports the inward flow of the ambient data.
    /// By using this method, the outward flow of data is also possible.
    /// The upper limit of the outward flow is determined by the location of the call to this method.
    /// </remarks>
    /// <param name="action">The <see cref="Action{T}"/> that directly or indirectly modifies an <see cref="AsyncLocal{T}.Value"/> property.</param>
    /// <returns>
    /// An <see cref="AsyncLocalModificationOperation"/> instance
    /// that can be used to either commit or discard <see cref="AsyncLocal{T}"/> modifications
    /// performed by the specified <paramref name="action"/>.
    /// </returns>
    public static AsyncLocalModificationOperation ModifyAsyncLocal(Action action) => new(action);

    /// <returns>
    /// An <see cref="AsyncLocalModificationOperation{T}"/> instance
    /// that can be used to either commit or discard <see cref="AsyncLocal{T}"/> modifications
    /// performed by the specified <paramref name="action"/>.
    /// </returns>
    /// <inheritdoc cref="ModifyAsyncLocal(Action)"/>
    public static AsyncLocalModificationOperation<T> ModifyAsyncLocal<T>(Action<T> action) => new(action);

    /// <summary>
    /// Synchronizes <see cref="AsyncLocal{T}"/> data access as follows:
    /// <see cref="AsyncLocal{T}"/> data modifications made prior to the call to <see cref="AsyncLocalBarrier"/>
    /// cannot be reordered to execute after the call to <see cref="AsyncLocalBarrier"/>.
    /// </summary>
    public static void AsyncLocalBarrier() => AsyncLocalModificationOperationBase.Barrier();

    /// <summary>
    /// Represents an operation that modifies ambient <see cref="AsyncLocal{T}"/> data associated with an asynchronous control flow.
    /// </summary>
    public abstract class AsyncLocalModificationOperationBase : IDisposable
    {
        protected AsyncLocalModificationOperationBase()
        {
            Debug.Assert(
                m_FlowState.Value is null,
                $"{nameof(ModifyAsyncLocal)} does not support recursion. Before creating a new modification operation, the previously created operation must be committed, discarded, or disposed. Additionally, a call to {nameof(AsyncLocalBarrier)} may be required.");

            m_FlowState.Value = new FlowState(this, false);
        }

        sealed record FlowState(AsyncLocalModificationOperationBase Operation, bool ChangesApplied);

        static readonly AsyncLocal<FlowState?> m_FlowState = new(FlowStateChanged);

        static void FlowStateChanged(AsyncLocalValueChangedArgs<FlowState?> args)
        {
            // This is a state transition function which is invoked on a thread context change.
            // It allows us to propagate accumulated data modifications to the currently executing
            // asynchronous control flow.

            // The existence of a state transition function is not obligatory as its absence can be
            // compensated by the presence of strategically placed barriers (see Barrier() method).
            // This would lead to a worse memory reclamation, however.

            if (args.ThreadContextChanged &&
                args.CurrentValue is not null and var flowState)
            {
                // The flow state is managed by a finite state machine (FSM).
                UpdateFsm(flowState);
            }
        }

        /// <summary>
        /// Updates a finite state machine (FSM) associated with the specified flow state,
        /// and sets the current flow state to a new value calculated by the FSM.
        /// </summary>
        /// <param name="currentFlowState">The current flow state.</param>
        static void UpdateFsm(FlowState currentFlowState)
        {
            var newFlowState = GetNextFlowState(currentFlowState);
            if (newFlowState != currentFlowState)
                m_FlowState.Value = newFlowState;
        }

        static FlowState? GetNextFlowState(FlowState state)
        {
            return state.Operation.m_State switch
            {
                OperationState.Initialized => state, // operation is not completed yet
                OperationState.Committed => HandleCommittedState(state), // propagate the changes to the current control flow branch
                OperationState.Discarded => null, // discard all existing flow states associated with the operation
                _ => throw new InvalidOperationException(),
            };

            static FlowState? HandleCommittedState(FlowState state)
            {
                // A full FSM (Finite State Machine) coding style is used here,
                // despite the fact that FlowState.ChangesApplied = true property is never stored in
                // the state object (such a state is considered as completed and thus the object gets
                // erased with a null value for better memory reclamation).

                // A low-hanging optimization is not to have ChangesApplied property at all,
                // but that would make the code more entangled and harder to understand or maintain.

                bool changesApplied = state.ChangesApplied;

                if (!changesApplied)
                {
                    // Apply the changes to the current control flow branch if they weren't applied yet.
                    state.Operation.ApplyChanges();
                    changesApplied = true;
                }

                FlowState? newState;
                if (changesApplied)
                {
                    // All actions are taken - no need to hold the flow state object anymore.
                    newState = null;
                }
                else
                {
                    newState = state;
                }

                return newState;
            }
        }

        /// <summary>
        /// Updates a finite state machine associated with the operation.
        /// </summary>
        void UpdateOperationFsm()
        {
            if (m_FlowState.Value is not null and var flowState &&
                flowState.Operation == this)
            {
                UpdateFsm(flowState);
            }
        }

        internal static void Barrier()
        {
            // Updating the current FSM to complete any pending activities
            // creates an ordered relation between AsyncLocal<T> operations
            // that were issued before and after the barrier.
            if (m_FlowState.Value is not null and var flowState)
                UpdateFsm(flowState);
        }

        enum OperationState
        {
            Initialized,
            Committed,
            Discarded
        }

        // Using volatile access modifier to ensure that the operation state is always visible.
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
        /// Applies the changes to the current asynchronous control flow state
        /// by invoking a state-modifying function supplied by the user.
        /// </summary>
        /// <remarks>
        /// This approach allows the changes to be "replayed" numerous times.
        /// </remarks>
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
            UpdateOperationFsm();
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

        // Using volatile access modifier to ensure that a committed value is always visible in the committed state.
        Volatile<T?> m_CommittedValue;

        /// <summary>
        /// Commits the changes.
        /// </summary>
        /// <param name="value">The value that will be passed to a state-modifying action.</param>
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

#if TFF_RUNTIMEHELPERS_ISREFERENCEORCONTAINSREFERENCES
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
            {
                m_CommittedValue = default;
            }
        }
    }
}
