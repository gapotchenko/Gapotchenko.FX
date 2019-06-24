using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gapotchenko.FX.ComponentModel
{
    /// <summary>
    /// Provides methods for manipulating the disposable objects.
    /// May also server as a base class for internal disposable objects that implement a finalizer pattern,
    /// but <see cref="DisposableBase"/> is usually more suitable for that role.
    /// </summary>
    public abstract class Disposable : DisposableBase
    {
        /// <summary>
        /// Tries to clear a disposable object at the specified reference
        /// by calling <see cref="IDisposable.Dispose"/> method and setting its value to <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method does not provide thread safety guarantees except memory model consistency.
        /// As a result, the <see cref="IDisposable.Dispose"/> method of a disposable object may be invoked several times
        /// when the <see cref="Clear{T}(ref T)"/> method is called concurrently from multiple threads.
        /// </para>
        /// <seealso cref="Clear{T}(ref T, bool)"/>
        /// </remarks>
        /// <typeparam name="T">The type of disposable object.</typeparam>
        /// <param name="value">The reference to disposable value.</param>
        /// <returns><c>true</c> when the object has been disposed and the value cleared; <c>false</c> otherwise.</returns>
        public static bool Clear<T>(ref T value) where T : class, IDisposable
        {
            // Intermediate grabbed value is needed to eliminate the chance of NullReferenceException due to unconscious race condition.
            // This method is thread-safe according to .NET memory model because pointer to an object is guaranteed to be atomic.

            // Load.
            var grabbedValue = value;

            // Clear as soon as possible to minimize the chance of a race condition.
            value = null;

            if (grabbedValue == null)
                return false;

            // Dispose.
            _RevertibleDispose(grabbedValue, ref value);
            return true;
        }

        static void _RevertibleDispose<T>(T value, ref T store) where T : class, IDisposable
        {
            // This is a separate method to allow inlining of the caller.

            try
            {
                value.Dispose();
            }
            catch
            {
                store = value;
                throw;
            }
        }

        /// <summary>
        /// Tries to clear a disposable object at the specified reference
        /// by calling <see cref="IDisposable.Dispose"/> method and setting its value to <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method works in different thread safety modes depending on a value of <paramref name="isThreadSafe"/> parameter.
        /// </para>
        /// <para>
        /// When <paramref name="isThreadSafe"/> is <c>false</c>, the method does not provide thread safety guarantees except memory model consistency.
        /// As a result, the <see cref="IDisposable.Dispose"/> method of a disposable object may be invoked several times
        /// when the <see cref="Clear{T}(ref T, bool)"/> method is called concurrently from multiple threads.
        /// </para>
        /// <para>
        /// When <paramref name="isThreadSafe"/> is <c>true</c>, the method provides a publication and execution thread safety.
        /// As a result, the <see cref="IDisposable.Dispose"/> method of a disposable object is invoked exactly once even
        /// when the <see cref="Clear{T}(ref T, bool)"/> method is called concurrently from multiple threads.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type of disposable object.</typeparam>
        /// <param name="value">The reference to disposable value.</param>
        /// <param name="isThreadSafe">
        /// <c>true</c> to make the method usable concurrently by multiple threads;
        /// <c>false</c> to make the method usable by one thread at a time.
        /// </param>
        /// <returns><c>true</c> when the object has been disposed and the value cleared; <c>false</c> otherwise.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool Clear<T>(ref T value, bool isThreadSafe) where T : class, IDisposable
        {
            if (isThreadSafe)
                return _ClearInterlocked(ref value);
            else
                return Clear(ref value);
        }

        static bool _ClearInterlocked<T>(ref T value) where T : class, IDisposable
        {
            // Load and clear.
            var grabbedValue = Interlocked.Exchange(ref value, null);
            if (grabbedValue == null)
                return false;

            // Dispose.
            _RevertibleDisposeInterlocked(grabbedValue, ref value);

            return true;
        }

        static void _RevertibleDisposeInterlocked<T>(T value, ref T store) where T : class, IDisposable
        {
            // This is a separate method to allow inlining of the caller.

            try
            {
                value.Dispose();
            }
            catch
            {
                Interlocked.CompareExchange(ref store, value, null);
                throw;
            }
        }
    }
}
