// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !HAS_TARGET_PLATFORM || WINDOWS

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class ProcessMemoryAccessor(IntPtr hProcess) : IProcessMemoryAccessor
{
    public int PageSize => SystemInfo.PageSize;

    public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset + count > buffer.Length)
            throw new ArgumentException();

        nint actualCount = 0;
        bool status;

        fixed (byte* p = buffer)
        {
            status = NativeMethods.ReadProcessMemory(
                hProcess,
                address,
                p + offset,
                count,
                ref actualCount);
        }

        if (!status)
        {
            if (throwOnError)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new Win32Exception(error);
            }

            return -1;
        }

        return (int)actualCount;
    }
}

#endif
