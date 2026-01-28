// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !HAS_TARGET_PLATFORM || LINUX

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics.Pal.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
static class NativeMethods
{
    public const int SIGINT = 2;

    [DllImport("libc")]
    public static extern int kill(int pid, int signal);
}

#endif
