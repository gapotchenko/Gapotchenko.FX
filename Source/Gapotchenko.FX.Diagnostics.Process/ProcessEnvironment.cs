using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics
{
    static class ProcessEnvironment
    {
        public static StringDictionary ReadVariables(Process process)
        {
            return _ReadVariablesCore(process.Handle);
        }

        static StringDictionary _ReadVariablesCore(IntPtr hProcess)
        {
            IntPtr penv = _GetPenv(hProcess);

            int dataSize;
            if (!_HasReadAccess(hProcess, penv, out dataSize))
                throw new Exception("Unable to read environment block.");

            const int maxEnvSize = 32767;
            if (dataSize > maxEnvSize)
                dataSize = maxEnvSize;

            var envData = new byte[dataSize];
            var res_len = IntPtr.Zero;
            bool b = NativeMethods.ReadProcessMemory(
                hProcess,
                penv,
                envData,
                new IntPtr(dataSize),
                ref res_len);
            if (!b || (int)res_len != dataSize)
                throw new Exception("Unable to read environment block data.");

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
                IntPtr res_len = IntPtr.Zero;
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
                IntPtr res_len = IntPtr.Zero;
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

        static IntPtr _GetPenv(IntPtr hProcess)
        {
            int processBitness = _GetProcessBitness(hProcess);

            if (processBitness == 64)
            {
                if (!Environment.Is64BitProcess)
                    throw new InvalidOperationException("The current process should run in 64 bit mode to be able to get the environment of another 64 bit process.");

                IntPtr pPeb = _GetPeb64(hProcess);

                IntPtr ptr;
                if (!_TryReadIntPtr(hProcess, pPeb + 0x20, out ptr))
                    throw new Exception("Unable to read PEB.");

                IntPtr penv;
                if (!_TryReadIntPtr(hProcess, ptr + 0x80, out penv))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                return penv;
            }
            else
            {
                IntPtr pPeb = _GetPeb32(hProcess);

                IntPtr ptr;
                if (!_TryReadIntPtr32(hProcess, pPeb + 0x10, out ptr))
                    throw new Exception("Unable to read PEB.");

                IntPtr penv;
                if (!_TryReadIntPtr32(hProcess, ptr + 0x48, out penv))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                return penv;
            }
        }

        static int _GetProcessBitness(IntPtr hProcess)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                bool wow64;
                if (!NativeMethods.IsWow64Process(hProcess, out wow64))
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

        static IntPtr _GetPeb64(IntPtr hProcess)
        {
            return _GetPebNative(hProcess);
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
    }
}
