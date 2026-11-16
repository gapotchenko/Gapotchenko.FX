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
        public static nint MinValue =>
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
        public static nint MaxValue =>
#if TFF_INTPTR_MINMAXVALUE
            IntPtr.MaxValue;
#else
            m_MaxValue;
#endif
    }

#if !TFF_INTPTR_MINMAXVALUE

    static readonly nint m_MaxValue = unchecked((nint)((nuint)UIntPtr.MaxValue >> 1));

    static readonly nint m_MinValue = ~m_MaxValue;

#endif
}
