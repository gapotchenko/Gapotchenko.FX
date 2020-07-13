using System;

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    sealed class ProcessMemoryAdapterWow64 : IProcessMemoryAdapter
    {
        public ProcessMemoryAdapterWow64(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        IntPtr m_hProcess;

        public int PageSize => 4096;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
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
