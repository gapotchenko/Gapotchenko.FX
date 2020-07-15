using System;
using System.Runtime.CompilerServices;

#if !TFF_MATHF

namespace System
{
    /// <summary>
    /// <para>
    /// Provides constants and static methods for trigonometric, logarithmic, and other common mathematical functions.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public static class MathF
    {
        /// <summary>
        /// Represents the natural logarithmic base, specified by the constant, e.
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
        /// </summary>
        public const float PI = (float)Math.PI;
    }
}

#else

[assembly: TypeForwardedTo(typeof(MathF))]

#endif
