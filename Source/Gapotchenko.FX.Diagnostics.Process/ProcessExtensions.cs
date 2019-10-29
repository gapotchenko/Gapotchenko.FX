using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    }
}
