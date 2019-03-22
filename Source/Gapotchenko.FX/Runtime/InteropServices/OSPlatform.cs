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

        public bool Equals(OSPlatform other) => string.Equals(_Value, other._Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is OSPlatform other && Equals(other);

        public override int GetHashCode() => _Value?.GetHashCode() ?? 0;

        public override string ToString() => _Value ?? string.Empty;

        public static bool operator ==(OSPlatform left, OSPlatform right) => left.Equals(right);

        public static bool operator !=(OSPlatform left, OSPlatform right) => !(left == right);
    }
}

#else

[assembly: TypeForwardedTo(typeof(OSPlatform))]

#endif
