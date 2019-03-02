using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

#if !TFF_WEAKREFERENCE_1

namespace System
{
    /// <summary>
    /// Represents a typed weak reference, which references an object while still allowing that object to be reclaimed by garbage collection.
    /// This is a polyfill provided by Gapotchenko FX.
    /// </summary>
    /// <typeparam name="T">The type of the object referenced.</typeparam>
    [Serializable]
    public sealed class WeakReference<T> : ISerializable
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class that references the specified object.
        /// </summary>
        /// <param name="target">The object to reference, or null.</param>
        public WeakReference(T target) :
            this(target, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class that references the specified object and
        /// uses the specified resurrection tracking.
        /// </summary>
        /// <param name="target">The object to reference, or null.</param>
        /// <param name="trackResurrection">
        /// <c>true</c> to track the object after finalization;
        /// <c>false</c> to track the object only until finalization.
        /// </param>
        public WeakReference(T target, bool trackResurrection)
        {
            _Create(target, trackResurrection);
        }

        internal WeakReference(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            var target = (T)info.GetValue("TrackedObject", typeof(T));
            bool trackResurrection = info.GetBoolean("TrackResurrection");
            _Create(target, trackResurrection);
        }

        WeakReference _WR;

        void _Create(T target, bool trackResurrection)
        {
            _WR = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        ///  Populates a <see cref="SerializationInfo"/> object with all the data
        ///  necessary to serialize the current <see cref="WeakReference{T}"/> object.
        /// </summary>
        /// <param name="info">An object that holds all the data necessary to serialize or deserialize the current <see cref="WeakReference{T}"/> object.</param>
        /// <param name="context">The location where serialized data is stored and retrieved.</param>
        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("TrackedObject", _WR.Target, typeof(T));
            info.AddValue("TrackResurrection", _WR.TrackResurrection);
        }

        /// <summary>
        /// Sets the target object that is referenced by this <see cref="WeakReference{T}"/> object.
        /// </summary>
        /// <param name="target">The new target object.</param>
        public void SetTarget(T target)
        {
            _WR.Target = target;
        }

        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current <see cref="WeakReference{T}"/> object.
        /// </summary>
        /// <param name="target">
        /// When this method returns, contains the target object, if it is available.
        /// This parameter is treated as uninitialized.
        /// </param>
        /// <returns><c>true</c> if the target was retrieved; otherwise, <c>false</c>.</returns>
        public bool TryGetTarget(out T target)
        {
            var t = (T)_WR.Target;
            target = t;
            return t != null;
        }
    }
}

#else

[assembly: TypeForwardedTo(typeof(WeakReference<>))]

#endif
