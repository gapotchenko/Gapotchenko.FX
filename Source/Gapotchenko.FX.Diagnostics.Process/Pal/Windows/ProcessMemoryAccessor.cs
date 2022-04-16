﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows
{
#if NET
    [SupportedOSPlatform("windows")]
#endif
    sealed class ProcessMemoryAccessor : IProcessMemoryAccessor
    {
        public ProcessMemoryAccessor(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        readonly IntPtr m_hProcess;

        public int PageSize => SystemInfo.PageSize;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length)
                throw new ArgumentException();

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
            {
                if (throwOnError)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != 0)
                        throw new Win32Exception(error);
                }

                return -1;
            }

            return actualCount.ToInt32();
        }
    }
}