// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET7_0_OR_GREATER
#define TFF_INTPTR_MINMAXVALUE
#endif

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension members for <see cref="IntPtr"/> structure.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IntPtrPolyfills
{
    /// <inheritdoc cref="IntPtrPolyfills"/>
    extension(IntPtr)
    {
        /// <summary>
        /// Represents the smallest possible value of <see cref="IntPtr"/>.
        /// </summary>
#if TFF_INTPTR_MINMAXVALUE
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static IntPtr MinValue =>
#if TFF_INTPTR_MINMAXVALUE
            IntPtr.MinValue;
#else
            m_MinValue;
#endif

        /// <summary>
        /// Represents the largest possible value of <see cref="IntPtr"/>.
        /// </summary>
#if TFF_INTPTR_MINMAXVALUE
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static IntPtr MaxValue =>
#if TFF_INTPTR_MINMAXVALUE
            IntPtr.MaxValue;
#else
            m_MaxValue;
#endif
    }

#if !TFF_INTPTR_MINMAXVALUE

    static readonly IntPtr m_MaxValue = unchecked((nint)((nuint)UIntPtr.MaxValue >> 1));

    static readonly IntPtr m_MinValue = ~(nint)m_MaxValue;

#endif
}
