using System;
using System.Runtime.InteropServices;

#nullable enable

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    sealed class ProcessMemoryAdapterWow64 : IProcessMemoryAdapter
    {
        public ProcessMemoryAdapterWow64(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        IntPtr m_hProcess;

        public int PageSize => SystemInfo.Native.PageSize;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            long actualCount = 0;
            int result;

            fixed (byte* p = buffer)
            {
                result = NativeMethods.NtWow64ReadVirtualMemory64(
                    m_hProcess,
                    address.ToInt64(),
                    p + offset,
                    count,
                    ref actualCount);
            }

            if (result != NativeMethods.STATUS_SUCCESS)
                return -1;

            return (int)actualCount;
        }
    }
}
