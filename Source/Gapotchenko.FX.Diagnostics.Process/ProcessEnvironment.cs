using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.Diagnostics
{
    static partial class ProcessEnvironment
    {
        public static StringDictionary ReadVariables(Process process)
        {
            return _ReadVariablesCore(process.Handle);
        }

        static StringDictionary _ReadVariablesCore(IntPtr hProcess)
        {
            var penv = _GetPenv(hProcess);

            const int maxEnvSize = 32767;
            byte[] envData;

            if (penv.CanBeRepresentedByNativePointer)
            {
                int dataSize;
                if (!_HasReadAccess(hProcess, penv, out dataSize))
                    throw new Exception("Unable to read environment block.");

                if (dataSize > maxEnvSize)
                    dataSize = maxEnvSize;

                envData = new byte[dataSize];
                var res_len = IntPtr.Zero;
                bool b = NativeMethods.ReadProcessMemory(
                    hProcess,
                    penv,
                    envData,
                    new IntPtr(dataSize),
                    ref res_len);

                if (!b || (int)res_len != dataSize)
                    throw new Exception("Unable to read environment block data.");
            }
            else if (penv.Size == 8 && IntPtr.Size == 4)
            {
                // Accessing a 64-bit process from 32-bit host.

                int dataSize;

                try
                {
                    if (!_HasReadAccessWow64(hProcess, penv.ToInt64(), out dataSize))
                        throw new Exception("Unable to read environment block with WOW64 API.");
                }
                catch (EntryPointNotFoundException)
                {
                    // Windows 10 does not provide NtWow64QueryVirtualMemory64 API call.
                    dataSize = maxEnvSize;
                }

                if (dataSize > maxEnvSize)
                    dataSize = maxEnvSize;

                envData = new byte[dataSize];
                long res_len = 0;
                int result = NativeMethods.NtWow64ReadVirtualMemory64(
                    hProcess,
                    penv.ToInt64(),
                    envData,
                    dataSize,
                    ref res_len);

                if (result != NativeMethods.STATUS_SUCCESS || res_len != dataSize)
                    throw new Exception("Unable to read environment block data with WOW64 API.");
            }
            else
            {
                throw new Exception("Unable to access process memory due to unsupported bitness cardinality.");
            }

            return _EnvToDictionary(envData);
        }

        static StringDictionary _EnvToDictionary(byte[] env)
        {
            var result = new StringDictionary();

            int len = env.Length;
            if (len < 4)
                return result;

            int n = len - 3;
            for (int i = 0; i < n; ++i)
            {
                byte c1 = env[i];
                byte c2 = env[i + 1];
                byte c3 = env[i + 2];
                byte c4 = env[i + 3];

                if (c1 == 0 && c2 == 0 && c3 == 0 && c4 == 0)
                {
                    len = i + 3;
                    break;
                }
            }

            char[] environmentCharArray = Encoding.Unicode.GetChars(env, 0, len);

            for (int i = 0; i < environmentCharArray.Length; i++)
            {
                int startIndex = i;
                while ((environmentCharArray[i] != '=') && (environmentCharArray[i] != '\0'))
                {
                    i++;
                }
                if (environmentCharArray[i] != '\0')
                {
                    if ((i - startIndex) == 0)
                    {
                        while (environmentCharArray[i] != '\0')
                        {
                            i++;
                        }
                    }
                    else
                    {
                        string str = new string(environmentCharArray, startIndex, i - startIndex);
                        i++;
                        int num3 = i;
                        while (environmentCharArray[i] != '\0')
                        {
                            i++;
                        }
                        string str2 = new string(environmentCharArray, num3, i - num3);
                        result[str] = str2;
                    }
                }
            }

            return result;
        }

        static bool _TryReadIntPtr32(IntPtr hProcess, IntPtr ptr, out IntPtr readPtr)
        {
            bool result;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                int dataSize = sizeof(Int32);
                var data = Marshal.AllocHGlobal(dataSize);
                var res_len = IntPtr.Zero;
                bool b = NativeMethods.ReadProcessMemory(
                    hProcess,
                    ptr,
                    data,
                    new IntPtr(dataSize),
                    ref res_len);
                readPtr = new IntPtr(Marshal.ReadInt32(data));
                Marshal.FreeHGlobal(data);
                if (!b || (int)res_len != dataSize)
                    result = false;
                else
                    result = true;
            }
            return result;
        }

        static bool _TryReadIntPtr(IntPtr hProcess, IntPtr ptr, out IntPtr readPtr)
        {
            bool result;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                int dataSize = IntPtr.Size;
                var data = Marshal.AllocHGlobal(dataSize);
                var res_len = IntPtr.Zero;
                bool b = NativeMethods.ReadProcessMemory(
                    hProcess,
                    ptr,
                    data,
                    new IntPtr(dataSize),
                    ref res_len);
                readPtr = Marshal.ReadIntPtr(data);
                Marshal.FreeHGlobal(data);
                if (!b || (int)res_len != dataSize)
                    result = false;
                else
                    result = true;
            }
            return result;
        }

        static bool _TryReadIntPtrWow64(IntPtr hProcess, long ptr, out long readPtr)
        {
            bool result;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                int dataSize = sizeof(long);
                var data = Marshal.AllocHGlobal(dataSize);
                long res_len = 0;
                int status = NativeMethods.NtWow64ReadVirtualMemory64(
                    hProcess,
                    ptr,
                    data,
                    dataSize,
                    ref res_len);
                readPtr = Marshal.ReadInt64(data);
                Marshal.FreeHGlobal(data);
                if (status != NativeMethods.STATUS_SUCCESS || res_len != dataSize)
                    result = false;
                else
                    result = true;
            }
            return result;
        }

        static UniPtr _GetPenv(IntPtr hProcess)
        {
            int processBitness = _GetProcessBitness(hProcess);

            if (processBitness == 64)
            {
                if (Environment.Is64BitProcess)
                {
                    // Accessing a 64-bit process from 64-bit host.

                    IntPtr pPeb = _GetPeb64(hProcess);

                    if (!_TryReadIntPtr(hProcess, pPeb + 0x20, out var ptr))
                        throw new Exception("Unable to read PEB.");

                    if (!_TryReadIntPtr(hProcess, ptr + 0x80, out var penv))
                        throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                    return penv;
                }
                else
                {
                    // Accessing a 64-bit process from 32-bit host.

                    var pPeb = _GetPeb64(hProcess);

                    if (!_TryReadIntPtrWow64(hProcess, pPeb.ToInt64() + 0x20, out var ptr))
                        throw new Exception("Unable to read PEB.");

                    if (!_TryReadIntPtrWow64(hProcess, ptr + 0x80, out var penv))
                        throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                    return new UniPtr(penv);
                }
            }
            else
            {
                // Accessing a 32-bit process from 32-bit host.

                IntPtr pPeb = _GetPeb32(hProcess);

                if (!_TryReadIntPtr32(hProcess, pPeb + 0x10, out var ptr))
                    throw new Exception("Unable to read PEB.");

                if (!_TryReadIntPtr32(hProcess, ptr + 0x48, out var penv))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                return penv;
            }
        }

        static int _GetProcessBitness(IntPtr hProcess)
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

        static IntPtr _GetPeb32(IntPtr hProcess)
        {
            if (Environment.Is64BitProcess)
            {
                var ptr = IntPtr.Zero;
                int res_len = 0;
                int pbiSize = IntPtr.Size;
                int status = NativeMethods.NtQueryInformationProcess(
                    hProcess,
                    NativeMethods.ProcessWow64Information,
                    ref ptr,
                    pbiSize,
                    ref res_len);
                if (res_len != pbiSize)
                    throw new Exception("Unable to query process information.");
                return ptr;
            }
            else
            {
                return _GetPebNative(hProcess);
            }
        }

        static IntPtr _GetPebNative(IntPtr hProcess)
        {
            var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();
            int res_len = 0;
            int pbiSize = Marshal.SizeOf(pbi);
            int status = NativeMethods.NtQueryInformationProcess(
                hProcess,
                NativeMethods.ProcessBasicInformation,
                ref pbi,
                pbiSize,
                ref res_len);
            if (res_len != pbiSize)
                throw new Exception("Unable to query process information.");
            return pbi.PebBaseAddress;
        }

        static UniPtr _GetPeb64(IntPtr hProcess)
        {
            if (Environment.Is64BitProcess)
            {
                return _GetPebNative(hProcess);
            }
            else
            {
                // Get PEB via WOW64 API.
                var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION_WOW64();
                int res_len = 0;
                int pbiSize = Marshal.SizeOf(pbi);
                int status = NativeMethods.NtWow64QueryInformationProcess64(
                    hProcess,
                    NativeMethods.ProcessBasicInformation,
                    ref pbi,
                    pbiSize,
                    ref res_len);
                if (res_len != pbiSize)
                    throw new Exception("Unable to query process information.");
                return new UniPtr(pbi.PebBaseAddress);
            }
        }

        static bool _HasReadAccess(IntPtr hProcess, IntPtr address, out int size)
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

        static bool _HasReadAccessWow64(IntPtr hProcess, long address, out int size)
        {
            size = 0;

            NativeMethods.MEMORY_BASIC_INFORMATION_WOW64 memInfo;
            var memInfoType = typeof(NativeMethods.MEMORY_BASIC_INFORMATION_WOW64);
            int memInfoLength = Marshal.SizeOf(memInfoType);
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

                memInfo = (NativeMethods.MEMORY_BASIC_INFORMATION_WOW64)Marshal.PtrToStructure(hMemInfoAligned, memInfoType);
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
