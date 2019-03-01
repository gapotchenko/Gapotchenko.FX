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
    /// Provides support for thread-safe lazy execution.
    /// </summary>
    [DebuggerDisplay("IsExecuted={IsExecuted}")]
#if TF_HOST_PROTECTION
    [HostProtection(Synchronization = true, ExternalThreading = true)]
#endif
    public struct ExecuteOnce
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteOnce"/> struct.
        /// </summary>
        /// <param name="action">The action.</param>
        public ExecuteOnce(Action action) :
            this(action, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteOnce"/> struct.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="syncLock">
        /// An object used as the mutually exclusive lock for action execution.
        /// When the given value is null, an unique synchronization lock object is used.
        /// </param>
        public ExecuteOnce(Action action, object syncLock)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _Action = action;
            _SyncLock = syncLock;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object _SyncLock;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Action _Action;

        /// <summary>
        /// Ensures that the action was executed.
        /// </summary>
        public void EnsureExecuted() => LazyInitializerEx.EnsureInitialized(ref _SyncLock, ref _Action);

        /// <summary>
        /// Gets a value indicating whether the action was executed.
        /// </summary>
        public bool IsExecuted => Volatile.Read(ref _Action) == null && _SyncLock != null; // check for _SyncLock is needed to cover uninitialized struct scenario
    }
}
