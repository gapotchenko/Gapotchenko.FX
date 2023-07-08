﻿// Gapotchenko.FX
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
// puzzle. In turn, that led to a plethora of half-baked attempts in cracking
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
// were propagated. In this way, the barrier of outward state propagation
// imposed by AsyncLocal<T> primitive ceases to exist.

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

            switch (flowState.Operation.m_State)
            {
                case State.Initialized:
                    // Nothing is ready yet.
                    break;

                case State.Committed:
                    if (flowState != args.PreviousValue)
                    {
                        // Propagate the changes to the current control flow branch.
                        UpdateFlowState(flowState);
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

        /// <summary>
        /// Updates the flow state using the finite state machine.
        /// </summary>
        /// <param name="state">The existing flow state.</param>
        static void UpdateFlowState(FlowState state)
        {
            var newState = GetNextFlowState(state);
            if (newState != state)
                m_FlowState.Value = newState;
        }

        /// <summary>
        /// Gets a next flow state of the finite state machine.
        /// </summary>
        /// <param name="state">The existing flow state.</param>
        static FlowState? GetNextFlowState(FlowState state)
        {
            // A full FSM (Finite State Machine) coding style is used here,
            // despite the fact that FlowState.ActionHandled = true is never stored in the state
            // (such a state is considered as completed and gets erased by null).

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
            else
                ApplyChanges(); // or directly
        }

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

            if (m_FlowState.Value is not null)
                m_FlowState.Value = null;
        }

        /// <summary>
        /// Applies the changes to the current state.
        /// </summary>
        void ApplyChanges()
        {
            Debug.Assert(m_State == State.Committed);

            DoAction();
        }

        /// <summary>
        /// Executes the state-modifying action.
        /// </summary>
        protected abstract void DoAction();

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
            m_Action();
        }
    }

    public sealed class AsyncLocalModificationOperation<T> : AsyncLocalModificationOperationBase
    {
        internal AsyncLocalModificationOperation(Action<T> action)
        {
            m_Action = action;
        }

        readonly Action<T> m_Action;

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

        protected override void DoAction()
        {
            m_Action(m_CommittedValue.Value!);
        }
    }
}