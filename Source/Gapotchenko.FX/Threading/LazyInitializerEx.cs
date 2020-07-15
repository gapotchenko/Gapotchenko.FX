using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading
{
    /// <summary>
    /// Provides extended routines for lazy initialization.
    /// </summary>
#if TFF_HOST_PROTECTION
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
#endif
    public static class LazyInitializerEx
    {
        /// <summary>
        /// Initializes a target reference or value type by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference or value of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget>(ref TTarget target, ref bool initialized, object syncLock, Func<TTarget> valueFactory)
        {
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            return LazyInitializer.EnsureInitialized(ref target, ref initialized, ref syncLock, valueFactory);
        }

        // ------------------------------------------------------------------------------------------------------------

#nullable enable
        /// <summary>
        /// Initializes a target <see cref="Object"/> type with the type's default constructor if it hasn't already been initialized.
        /// </summary>
        /// <param name="target">A reference of <see cref="Object"/> type to initialize if it has not already been initialized.</param>
        /// <returns>The initialized reference of <see cref="Object"/> type.</returns>
        public static object EnsureInitialized(ref object? target)
        {
            var result = target;
            if (result != null)
                return result;

            result = new object();
            return Interlocked.CompareExchange(ref target, result, null) ?? result;
        }
#nullable restore

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target entity by executing supplied action if it hasn't already been initialized.
        /// </summary>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="action">The action that is called to initialize the target entity.</param>
        public static void EnsureInitialized(ref bool initialized, ref object syncLock, Action action)
        {
            if (Volatile.Read(ref initialized))
                return;
            _EnsureInitializedCore(ref initialized, EnsureInitialized(ref syncLock), action);
        }

        static void _EnsureInitializedCore(ref bool initialized, object syncLock, Action action)
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    action();
                    Volatile.Write(ref initialized, true);
                }
            }
        }

        /// <summary>
        /// Initializes a target entity by executing supplied action if it hasn't already been initialized.
        /// </summary>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="action">The action that is called to initialize the target entity.</param>
        [CLSCompliant(false)]
        public static void EnsureInitialized(ref bool initialized, object syncLock, Action action)
        {
            if (Volatile.Read(ref initialized))
                return;
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            _EnsureInitializedCore(ref initialized, syncLock, action);
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target entity by executing supplied action if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="action">The action that is called to initialize the target entity.</param>
        /// <param name="state">The state passed to the action.</param>
        public static void EnsureInitialized<TState>(ref bool initialized, ref object syncLock, Action<TState> action, TState state)
        {
            if (Volatile.Read(ref initialized))
                return;
            _EnsureInitializedCore(ref initialized, EnsureInitialized(ref syncLock), action, state);
        }

        static void _EnsureInitializedCore<TState>(ref bool initialized, object syncLock, Action<TState> action, TState state)
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    action(state);
                    Volatile.Write(ref initialized, true);
                }
            }
        }

        /// <summary>
        /// Initializes a target entity by executing supplied action if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the action.</typeparam>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="action">The action that is called to initialize the target entity.</param>
        /// <param name="state">The state passed to the action.</param>
        public static void EnsureInitialized<TState>(ref bool initialized, object syncLock, Action<TState> action, TState state)
        {
            if (Volatile.Read(ref initialized))
                return;
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            _EnsureInitializedCore(ref initialized, syncLock, action, state);
        }

        // ------------------------------------------------------------------------------------------------------------

        [DoesNotReturn]
        static void _ThrowArgumentNullException_SyncLock()
        {
            // This should be a separate method to avoid performance degradation.
            throw new ArgumentNullException("syncLock");
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target reference or value type by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <typeparam name="TState">The type of the state passed to the value factory.</typeparam>
        /// <param name="target">A reference or value of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <param name="state">The state passed to the value factory.</param>
        public static TTarget EnsureInitialized<TTarget, TState>(ref TTarget target, ref bool initialized, ref object syncLock, Func<TState, TTarget> valueFactory, TState state)
        {
            if (Volatile.Read(ref initialized))
                return target;
            return _EnsureInitializedCore(ref target, ref initialized, EnsureInitialized(ref syncLock), valueFactory, state);
        }

        static TTarget _EnsureInitializedCore<TTarget, TState>(ref TTarget target, ref bool initialized, object syncLock, Func<TState, TTarget> valueFactory, TState state)
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    target = valueFactory(state);
                    Volatile.Write(ref initialized, true);
                }
            }
            return target;
        }

        /// <summary>
        /// Initializes a target reference or value type by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <typeparam name="TState">The type of the state passed to the value factory.</typeparam>
        /// <param name="target">A reference or value of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="initialized">A reference to a <see cref="Boolean"/> value that determines whether the target has already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <param name="state">The state passed to the value factory.</param>
        public static TTarget EnsureInitialized<TTarget, TState>(ref TTarget target, ref bool initialized, object syncLock, Func<TState, TTarget> valueFactory, TState state)
        {
            if (Volatile.Read(ref initialized))
                return target;
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            return _EnsureInitializedCore(ref target, ref initialized, syncLock, valueFactory, state);
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target entity by executing supplied action if it hasn't already been initialized.
        /// </summary>
        /// <param name="syncLock">
        /// A reference to an object used as the mutually exclusive lock for initializing target.
        /// If <paramref name="syncLock"/> is null, a new object will be instantiated.</param>
        /// <param name="action">
        /// A reference to an action that is called to initialize the target entity.
        /// Once entity is initialized, the action is filled with a null value indicating that the target has already been initialized.
        /// </param>
        public static void EnsureInitialized(ref object syncLock, ref Action action)
        {
            if (Volatile.Read(ref action) == null)
                return;
            _EnsureInitializedCore(ref syncLock, ref action);
        }

        static void _EnsureInitializedCore(ref object syncLock, ref Action action)
        {
            lock (EnsureInitialized(ref syncLock))
            {
                var actionValue = Volatile.Read(ref action);
                if (actionValue != null)
                {
                    actionValue();
                    Volatile.Write(ref action, null);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target reference or value type by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference or value of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If <paramref name="syncLock"/> is <c>null</c>, a new object will be instantiated.</param>
        /// <param name="valueFactory">
        /// A reference to a function that is called to initialize the target.
        /// Once target is initialized, the <paramref name="valueFactory"/> is set to <c>null</c> indicating that the target has already been initialized.
        /// </param>
        public static TTarget EnsureInitialized<TTarget>(ref TTarget target, ref object syncLock, ref Func<TTarget> valueFactory)
        {
            if (Volatile.Read(ref valueFactory) == null)
                return target;
            return _EnsureInitializedCore(ref target, ref syncLock, ref valueFactory);
        }

        static TTarget _EnsureInitializedCore<TTarget>(ref TTarget target, ref object syncLock, ref Func<TTarget> valueFactory)
        {
            lock (EnsureInitialized(ref syncLock))
            {
                var valueFactoryValue = Volatile.Read(ref valueFactory);
                if (valueFactoryValue != null)
                {
                    target = valueFactoryValue();
                    Volatile.Write(ref valueFactory, null);
                }
            }
            return target;
        }

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <typeparam name="TState">The type of the state passed to the value factory.</typeparam>
        /// <param name="target">A reference of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <param name="state">The state passed to the value factory.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget, TState>(ref TTarget target, object syncLock, Func<TState, TTarget> valueFactory, TState state) where TTarget : class
        {
            var result = Volatile.Read(ref target);
            if (result != null)
                return result;
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            return _EnsureInitializedCore(ref target, syncLock, valueFactory, state);
        }

        /// <summary>
        /// Initializes a target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <typeparam name="TState">The type of the state passed to the value factory.</typeparam>
        /// <param name="target">A reference of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <param name="state">The state passed to the value factory.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget, TState>(ref TTarget target, ref object syncLock, Func<TState, TTarget> valueFactory, TState state) where TTarget : class
        {
            var result = Volatile.Read(ref target);
            if (result != null)
                return result;
            return _EnsureInitializedCore(ref target, EnsureInitialized(ref syncLock), valueFactory, state);
        }

        static TTarget _EnsureInitializedCore<TTarget, TState>(ref TTarget target, object syncLock, Func<TState, TTarget> valueFactory, TState state) where TTarget : class
        {
            TTarget result;
            lock (syncLock)
            {
                result = Volatile.Read(ref target);
                if (result == null)
                {
                    result = valueFactory(state);
                    Volatile.Write(ref target, result);
                }
            }
            return result;
        }

        // ------------------------------------------------------------------------------------------------------------

#nullable enable

        /// <summary>
        /// Initializes a target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget>(ref TTarget? target, object syncLock, Func<TTarget> valueFactory) where TTarget : class
        {
            var result = Volatile.Read(ref target);
            if (result != null)
                return result;
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            return _EnsureInitializedCore(ref target, syncLock, valueFactory);
        }

        /// <summary>
        /// Initializes a target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference of type <typeparamref name="TTarget"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget>(ref TTarget? target, ref object? syncLock, Func<TTarget> valueFactory) where TTarget : class
        {
            var result = Volatile.Read(ref target);
            if (result != null)
                return result;
            return _EnsureInitializedCore(ref target, EnsureInitialized(ref syncLock), valueFactory);
        }

        static TTarget _EnsureInitializedCore<TTarget>(ref TTarget? target, object syncLock, Func<TTarget> valueFactory) where TTarget : class
        {
            TTarget? result;
            lock (syncLock)
            {
                result = Volatile.Read(ref target);
                if (result == null)
                {
                    result = valueFactory();
                    Volatile.Write(ref target, result);
                }
            }
            return result;
        }

#nullable restore

        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes an optional target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference of type <see cref="Optional{TTarget}"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">An object used as the mutually exclusive lock for initializing target.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget>(ref Optional<TTarget> target, object syncLock, Func<TTarget> valueFactory)
        {
            if (syncLock == null)
                _ThrowArgumentNullException_SyncLock();
            return LazyInitializer.EnsureInitialized(ref target.m_Value, ref target.m_HasValue, ref syncLock, valueFactory);
        }

        /// <summary>
        /// Initializes an optional target reference by using a specified function if it hasn't already been initialized.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target to be initialized.</typeparam>
        /// <param name="target">A reference of type <see cref="Optional{TTarget}"/> to initialize if it hasn't already been initialized.</param>
        /// <param name="syncLock">A reference to an object used as the mutually exclusive lock for initializing target. If syncLock is null, a new object will be instantiated.</param>
        /// <param name="valueFactory">The function that is called to initialize the reference or value.</param>
        /// <returns>The initialized value of type <typeparamref name="TTarget"/>.</returns>
        public static TTarget EnsureInitialized<TTarget>(ref Optional<TTarget> target, ref object syncLock, Func<TTarget> valueFactory)
        {
            return LazyInitializer.EnsureInitialized(ref target.m_Value, ref target.m_HasValue, ref syncLock, valueFactory);
        }
    }
}
