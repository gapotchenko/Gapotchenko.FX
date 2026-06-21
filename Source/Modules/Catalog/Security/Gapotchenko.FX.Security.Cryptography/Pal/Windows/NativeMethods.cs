// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if !HAS_TARGET_PLATFORM || WINDOWS

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Security.Cryptography.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    public const uint STATUS_SUCCESS = 0x00000000;
    public const uint STATUS_OBJECT_NAME_NOT_FOUND = 0xC0000034;

    [DllImport("bcrypt.dll")]
    public static extern uint BCryptGetFipsAlgorithmMode([MarshalAs(UnmanagedType.U1), Out] out bool pfEnabled);
}

#endif
