// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET7_0_OR_GREATER
#define TFF_UINTPTR_MAXVALUE
#endif

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension members for <see cref="UIntPtr"/> structure.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class UIntPtrPolyfills
{
    /// <inheritdoc cref="UIntPtrPolyfills"/>
    extension(UIntPtr)
    {
        /// <summary>
        /// Represents the largest possible value of <see cref="UIntPtr"/>.
        /// </summary>
#if TFF_UINTPTR_MAXVALUE
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [CLSCompliant(false)]
        public static UIntPtr MaxValue =>
#if TFF_UINTPTR_MAXVALUE
            UIntPtr.MaxValue;
#else
            unchecked((nuint)(nint)(-1));
#endif
    }
}
