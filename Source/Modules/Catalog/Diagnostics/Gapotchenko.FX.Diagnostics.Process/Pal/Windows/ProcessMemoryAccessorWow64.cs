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
sealed class ProcessMemoryAccessorWow64(IntPtr hProcess) : IProcessMemoryAccessor
{
    public int PageSize => SystemInfo.Native.PageSize;

    public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset + count > buffer.Length)
            throw new ArgumentException();

        long actualCount = 0;
        int status;

        fixed (byte* p = buffer)
        {
            status = NativeMethods.NtWow64ReadVirtualMemory64(
                hProcess,
                address.ToInt64(),
                p + offset,
                count,
                ref actualCount);
        }

        if (status != NativeMethods.STATUS_SUCCESS)
            return -1;

        return (int)actualCount;
    }
}

#endif
