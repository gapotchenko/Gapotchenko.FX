using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !TFF_RUNTIME_INFORMATION

namespace System.Runtime.InteropServices
{
    using Resources = Gapotchenko.FX.Properties.Resources;

    /// <summary>
    /// Represents an operating system platform.
    /// </summary>
    public readonly struct OSPlatform : IEquatable<OSPlatform>
    {
        /// <summary>
        /// Gets an object that represents Linux operating system.
        /// </summary>
        public static OSPlatform Linux { get; } = new OSPlatform("LINUX");

        /// <summary>
        /// Gets an object that represents macOS operating system.
        /// </summary>
        public static OSPlatform OSX { get; } = new OSPlatform("OSX");

        /// <summary>
        /// Gets an object that represents Windows operating system.
        /// </summary>
        public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");

        OSPlatform(string osPlatform)
        {
            _Value = osPlatform;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string _Value;

        /// <summary>
        /// Creates a new <see cref="OSPlatform"/> instance.
        /// </summary>
        /// <param name="osPlatform">The name of the platform.</param>
        /// <returns>An <see cref="OSPlatform"/> instance that represents the operating system specified by <paramref name="osPlatform"/> .</returns>
        public static OSPlatform Create(string osPlatform)
        {
            if (osPlatform == null)
                throw new ArgumentNullException(nameof(osPlatform));
            if (osPlatform.Length == 0)
                throw new ArgumentException(Resources.ValueCannotBeEmpty, nameof(osPlatform));

            return new OSPlatform(osPlatform);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <c>true</c> if the current object is equal to the object specified by <paramref name="other" /> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(OSPlatform other) => string.Equals(_Value, other._Value, StringComparison.Ordinal);

        /// <summary>
        /// Determines whether a specified <see cref="Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is OSPlatform other && Equals(other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => _Value ?? string.Empty;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(OSPlatform left, OSPlatform right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(OSPlatform left, OSPlatform right) => !(left == right);
    }
}

#else

[assembly: TypeForwardedTo(typeof(OSPlatform))]

#endif
