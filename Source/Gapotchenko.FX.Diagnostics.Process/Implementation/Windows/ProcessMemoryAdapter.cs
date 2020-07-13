using System;

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    sealed class ProcessMemoryAdapter : IProcessMemoryAdapter
    {
        public ProcessMemoryAdapter(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        IntPtr m_hProcess;

        public int PageSize => 4096;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            var actualCount = IntPtr.Zero;
            bool result;

            fixed (byte* p = buffer)
            {
                result = NativeMethods.ReadProcessMemory(
                    m_hProcess,
                    address,
                    p + offset,
                    new IntPtr(count),
                    ref actualCount);
            }

            if (!result)
                return -1;

            return actualCount.ToInt32();
        }
    }
}
