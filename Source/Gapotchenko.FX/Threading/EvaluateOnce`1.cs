using Gapotchenko.FX.Properties;
using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Threading
{
    /// <summary>
    /// Provides a thread-safe evaluation strategy which delays the evaluation of an expression until its value is needed.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="EvaluateOnce{T}"/> is a struct and thus sometimes it may be a better choice than <see cref="Lazy{T}"/> in terms of performance and memory allocation.
    /// </para>
    /// <para>
    /// <see cref="EvaluateOnce{T}"/> is thread-safe.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
#if TF_HOST_PROTECTION
    [HostProtection(Synchronization = true, ExternalThreading = true)]
#endif
    public struct EvaluateOnce<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> struct.
        /// </summary>
        /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
        public EvaluateOnce(Func<T> valueFactory) :
            this(valueFactory, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> struct.
        /// </summary>
        /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
        /// <param name="syncLock">
        /// An object used as the mutually exclusive lock for value evaluation.
        /// When the given value is <c>null</c>, an unique synchronization lock object is used.
        /// </param>
        public EvaluateOnce(Func<T> valueFactory, object syncLock)
        {
            _ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _SyncLock = syncLock;
            _Value = default;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        T _Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object _SyncLock;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Func<T> _ValueFactory;

        /// <summary>
        /// Gets the lazily evaluated value of the current <see cref="EvaluateOnce{T}"/> instance.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value => LazyInitializerEx.EnsureInitialized(ref _Value, ref _SyncLock, ref _ValueFactory);

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this <see cref="EvaluateOnce{T}"/> instance.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                Thread.MemoryBarrier();
                return
                    _ValueFactory == null &&
                    _SyncLock != null; // check for _SyncLock is needed to cover uninitialized struct scenario
            }
        }

        /// <summary>
        /// Creates and returns a string representation of the <see cref="Value"/> property for this instance.
        /// </summary>
        /// <returns>
        /// The result of calling the <see cref="Object.ToString()"/> method on the <see cref="Value"/> property for this instance,
        /// if the value has been created (that is, if the <see cref="IsValueCreated"/> property returns <c>true</c>).
        /// Otherwise, a string indicating that the value has not been created.
        /// </returns>
        public override string ToString() => IsValueCreated ? Value.ToString() : Resources.ValueNotCreated;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal T ValueForDebugDisplay => IsValueCreated ? Value : default(T);
    }
}
