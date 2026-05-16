// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET6_0_OR_GREATER
#define TFF_MARSHAL_GETLASTPINVOKEERROR
#endif

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides polyfills for <see cref="Marshal"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class MarshalPolyfills
{
    /// <summary>
    /// Provides member polyfills for <see cref="Marshal"/> class.
    /// </summary>
    extension(Marshal)
    {
        /// <summary>
        /// Gets the last platform invoke error on the current thread.
        /// </summary>
        /// <returns>The last platform invoke error.</returns>
#if TFF_MARSHAL_GETLASTPINVOKEERROR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int GetLastPInvokeError()
        {
#if TFF_MARSHAL_GETLASTPINVOKEERROR
            return Marshal.GetLastPInvokeError();
#else
            return Marshal.GetLastWin32Error();
#endif
        }
    }
}
