// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !HAS_TARGET_PLATFORM || WINDOWS

namespace Gapotchenko.FX.Diagnostics.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class SystemInfo
{
    static SystemInfo()
    {
        NativeMethods.GetSystemInfo(out var systemInfo);
        PageSize = checked((int)systemInfo.dwPageSize);
    }

    public static int PageSize { get; }

    public static class Native
    {
        static Native()
        {
            NativeMethods.GetNativeSystemInfo(out var systemInfo);
            PageSize = checked((int)systemInfo.dwPageSize);
        }

        public static int PageSize { get; }
    }
}

#endif
