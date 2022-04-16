using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows
{
#if NET
    [SupportedOSPlatform("windows")]
#endif
    static partial class ProcessEnvironment
    {
        public static StringDictionary ReadVariables(Process process) => _ReadVariablesCore(process.Handle);

        static StringDictionary _ReadVariablesCore(IntPtr hProcess)
        {
            int retryCount = 3;
            bool RetryPolicy() => --retryCount > 0;

        Again:
            try
            {
                var stream = _GetEnvStream(hProcess);
                var reader = new ProcessBinaryReader(new BufferedStream(stream), Encoding.Unicode);
                var env = _ReadEnv(reader);

                if (env.Count == 0)
                {
                    // Empty environment may indicate that a process environment block has not been initialized yet.
                    if (RetryPolicy())
                        goto Again;
                }

                return env;
            }
            catch (EndOfStreamException)
            {
                // There may be a race condition in environment block initialization of a recently started process.
                if (RetryPolicy())
                    goto Again;
                else
                    throw;
            }
        }

        static Stream _GetEnvStream(IntPtr hProcess)
        {
            var pEnv = _GetPEnv(hProcess);
            if (pEnv.CanBeRepresentedByNativePointer)
            {
                int dataSize;
                if (!_HasReadAccessToProcessMemory(hProcess, pEnv, out dataSize))
                    throw new Exception("Unable to read process environment block.");

                var provider = new ProcessMemoryAccessor(hProcess);
                return new ProcessMemoryStream(provider, pEnv, dataSize);
            }
            else if (pEnv.Size == 8 && IntPtr.Size == 4)
            {
                // Accessing a 64-bit process from 32-bit host.

                int dataSize;
                try
                {
                    if (!_HasReadAccessToProcessMemoryWow64(hProcess, pEnv.ToInt64(), out dataSize))
                        throw new Exception("Unable to read process environment block with WOW64 API.");
                }
                catch (EntryPointNotFoundException)
                {
                    // Windows 10 does not provide NtWow64QueryVirtualMemory64 API call.
                    dataSize = -1;
                }

                var adapter = new ProcessMemoryAccessorWow64(hProcess);
                return new ProcessMemoryStream(adapter, pEnv, dataSize);
            }
            else
            {
                throw new Exception("Unable to access process memory due to unsupported bitness cardinality.");
            }
        }

        static StringDictionary _ReadEnv(ProcessBinaryReader br)
        {
            var env = new StringDictionary();

            for (; ; )
            {
                var s = br.ReadCString();
                if (s.Length == 0)
                {
                    // End of environment block.
                    break;
                }

                int j = s.IndexOf('=');
                if (j <= 0)
                    continue;

                string name = s.Substring(0, j);
                string value = s.Substring(j + 1);

                env[name] = value;
            }

            return env;
        }

        static unsafe bool _TryReadIntPtr32(IntPtr hProcess, IntPtr ptr, out IntPtr readPtr)
        {
            int data;
            const int dataSize = sizeof(int);

            var res_len = IntPtr.Zero;
            bool status = NativeMethods.ReadProcessMemory(hProcess, ptr, &data, new IntPtr(dataSize), ref res_len);

            readPtr = new IntPtr(data);
            return status && (int)res_len == dataSize;
        }

        static unsafe bool _TryReadIntPtr(IntPtr hProcess, IntPtr ptr, out IntPtr readPtr)
        {
            IntPtr data;
            int dataSize = IntPtr.Size;

            var res_len = IntPtr.Zero;
            bool status = NativeMethods.ReadProcessMemory(hProcess, ptr, &data, new IntPtr(dataSize), ref res_len);

            readPtr = data;
            return status && (int)res_len == dataSize;
        }

        static unsafe bool _TryReadIntPtrWow64(IntPtr hProcess, long ptr, out long readPtr)
        {
            long data;
            const int dataSize = sizeof(long);

            long res_len = 0;
            int status = NativeMethods.NtWow64ReadVirtualMemory64(hProcess, ptr, &data, dataSize, ref res_len);

            readPtr = data;
            return status == NativeMethods.STATUS_SUCCESS && res_len == dataSize;
        }

        static UniPtr _GetPEnv(IntPtr hProcess)
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

                    if (!_TryReadIntPtr(hProcess, ptr + 0x80, out var pEnv))
                        throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                    return pEnv;
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

        static bool _HasReadAccessToProcessMemory(IntPtr hProcess, IntPtr address, out int size)
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

        static bool _HasReadAccessToProcessMemoryWow64(IntPtr hProcess, long address, out int size)
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
