using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics
{
    /// <summary>
    /// Provides extended operations for <see cref="Process"/> class.
    /// </summary>
    public static partial class ProcessExtensions
    {
        /// <summary>
        /// Reads the environment variables from the environment block of a process.
        /// </summary>
        /// <param name="process">The process. It can be any process running on local machine.</param>
        /// <returns>The environment variables.</returns>
        public static StringDictionary ReadEnvironmentVariables(this Process process)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException();

            return ProcessEnvironment.ReadVariables(process);
        }

        static int _GetParentProcessID(IntPtr handle)
        {
            var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();

            int size = 0;
            int status = NativeMethods.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), ref size);
            if (status != 0)
                throw new Exception("Unable to get parent process. NtQueryInformationProcess failed.");

            return pbi.InheritedFromUniqueProcessId.ToInt32();
        }

        /// <summary>
        /// Gets the parent process.
        /// </summary>
        /// <param name="process">The process to get the parent for.</param>
        /// <returns>The parent process or <c>null</c> if it is no longer running or there is no parent.</returns>
        public static Process GetParent(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException();

            int parentPID = _GetParentProcessID(process.Handle);
            try
            {
                var parentProcess = Process.GetProcessById(parentPID);
                if (!_IsValidParentProcess(parentProcess, process))
                    return null;
                return parentProcess;
            }
            catch (ArgumentException)
            {
                // "Process with an Id of NNN is not running."
                return null;
            }
        }

        static bool _IsValidParentProcess(Process parentProcess, Process childProcess)
        {
            if (parentProcess.StartTime > childProcess.StartTime)
            {
                // The parent process was started after the child process.
                // This condition indicates that the real parent process has exited before, and
                // its process ID has been reused by another unrelated process.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enumerates parent processes.
        /// </summary>
        /// <remarks>
        /// The closest parents are returned first.
        /// </remarks>
        /// <param name="process">The process to get the parents for.</param>
        /// <returns>The sequence of parent processes.</returns>
        public static IEnumerable<Process> EnumerateParents(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            for (; ; )
            {
                var parent = process.GetParent();
                if (parent == null)
                    break;

                yield return parent;

                process = parent;
            }
        }

        /// <summary>
        /// <para>
        /// Gets file name of a process image.
        /// </para>
        /// <para>
        /// Usually the returned value corresponds to the value of <see cref="ProcessModule.FileName"/> property of a main process module.
        /// The difference becomes apparent when the current process cannot access the module information due to security restrictions.
        /// While <see cref="ProcessModule.FileName"/> may not work in that situation, this method always works.
        /// </para>
        /// </summary>
        /// <param name="process">The process to get image file name for.</param>
        /// <returns>The file name of a process image.</returns>
        public static string GetImageFileName(Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return process.MainModule.FileName;

            ProcessModule mainModule = null;
            try
            {
                mainModule = process.MainModule;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Win32Exception)
            {
            }

            if (mainModule != null)
                return mainModule.FileName;

            var sb = new StringBuilder(NativeMethods.MAX_PATH);
            for (; ; )
            {
                uint dwSize = (uint)sb.Capacity;
                var result = NativeMethods.QueryFullProcessImageName(process.Handle, 0, sb, ref dwSize);
                if (!result)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (errorCode == NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                    {
                        const int MaxCapacity = 32768;

                        int capacity = sb.Capacity;
                        if (capacity < MaxCapacity)
                        {
                            sb.Capacity = Math.Min(capacity * 2, MaxCapacity);
                            continue;
                        }
                    }

                    throw new Win32Exception(errorCode);
                }

                break;
            }

            return sb.ToString();
        }

    }
}
