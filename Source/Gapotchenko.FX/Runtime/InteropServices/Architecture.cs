using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !TFF_RUNTIME_INFORMATION

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// <para>
    /// Indicates the processor architecture.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public enum Architecture
    {
        /// <summary>
        /// An Intel-based 32-bit processor architecture.
        /// </summary>
        X86,

        /// <summary>
        /// An AMD-based 64-bit processor architecture.
        /// </summary>
        X64,

        /// <summary>
        /// A 32-bit ARM processor architecture.
        /// </summary>
        Arm,

        /// <summary>
        /// A 64-bit ARM processor architecture.
        /// </summary>
        Arm64
    }
}

#else

[assembly: TypeForwardedTo(typeof(Architecture))]

#endif
