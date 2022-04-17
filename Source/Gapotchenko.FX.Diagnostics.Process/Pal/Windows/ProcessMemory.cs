using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows
{
#if NET
    [SupportedOSPlatform("windows")]
#endif
    static class ProcessMemory
    {
        public static int GetBitness(IntPtr hProcess)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                if (!NativeMethods.IsWow64Process(hProcess, out var wow64))
                    return 32;
                if (wow64)
                    return 32;
                return 64;
            }
            else
            {
                return 32;
            }
        }

        public static unsafe bool TryReadIntPtr(IntPtr hProcess, IntPtr address, out IntPtr value)
        {
            IntPtr data;
            int dataSize = IntPtr.Size;

            nint res_len = 0;
            bool status = NativeMethods.ReadProcessMemory(hProcess, address, &data, dataSize, ref res_len);

            value = data;
            return status && res_len == dataSize;
        }

        public static unsafe bool TryReadIntPtrWow64(IntPtr hProcess, UniPtr address, out UniPtr value)
        {
            long data;
            const int dataSize = sizeof(long);

            long res_len = 0;
            int status = NativeMethods.NtWow64ReadVirtualMemory64(hProcess, address.ToInt64(), &data, dataSize, ref res_len);

            value = new UniPtr(data);
            return status == NativeMethods.STATUS_SUCCESS && res_len == dataSize;
        }

        public static bool HasReadAccess(IntPtr hProcess, IntPtr address, out int size)
        {
            size = 0;

            var memInfo = new NativeMethods.MEMORY_BASIC_INFORMATION();
            int result = NativeMethods.VirtualQueryEx(
                hProcess,
                address,
                ref memInfo,
                Marshal.SizeOf(memInfo));

            if (result == 0)
                return false;

            if (memInfo.Protect == NativeMethods.PAGE_NOACCESS || memInfo.Protect == NativeMethods.PAGE_EXECUTE)
                return false;

            try
            {
                size = Convert.ToInt32(memInfo.RegionSize.ToInt64() - (address.ToInt64() - memInfo.BaseAddress.ToInt64()));
            }
            catch (OverflowException)
            {
                return false;
            }

            if (size <= 0)
                return false;

            return true;
        }

        public static bool HasReadAccessWow64(IntPtr hProcess, long address, out int size)
        {
            size = 0;

            NativeMethods.MEMORY_BASIC_INFORMATION_WOW64 memInfo;

            int memInfoLength = Marshal.SizeOf<NativeMethods.MEMORY_BASIC_INFORMATION_WOW64>();

            const int memInfoAlign = 8;

            long resultLength = 0;
            int result;

            IntPtr hMemInfo = Marshal.AllocHGlobal(memInfoLength + memInfoAlign * 2);
            try
            {
                // Align to 64 bits.
                var hMemInfoAligned = new IntPtr(hMemInfo.ToInt64() & ~(memInfoAlign - 1L));

                result = NativeMethods.NtWow64QueryVirtualMemory64(
                    hProcess,
                    address,
                    NativeMethods.MEMORY_INFORMATION_CLASS.MemoryBasicInformation,
                    hMemInfoAligned,
                    memInfoLength,
                    ref resultLength);

                memInfo = Marshal.PtrToStructure<NativeMethods.MEMORY_BASIC_INFORMATION_WOW64>(hMemInfoAligned);
            }
            finally
            {
                Marshal.FreeHGlobal(hMemInfo);
            }

            if (result != NativeMethods.STATUS_SUCCESS)
                return false;

            if (memInfo.Protect == NativeMethods.PAGE_NOACCESS || memInfo.Protect == NativeMethods.PAGE_EXECUTE)
                return false;

            try
            {
                size = Convert.ToInt32(memInfo.RegionSize - (address - memInfo.BaseAddress));
            }
            catch (OverflowException)
            {
                return false;
            }

            if (size <= 0)
                return false;

            return true;
        }
    }
}
