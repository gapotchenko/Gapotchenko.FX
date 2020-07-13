using Gapotchenko.FX.Diagnostics.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

#nullable enable

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
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            return ImplementationServices.Adapter.ReadProcessEnvironmentVariables(process);
        }

        /// <summary>
        /// Gets the parent process.
        /// </summary>
        /// <param name="process">The process to get the parent for.</param>
        /// <returns>The parent process or <c>null</c> if it is no longer running or there is no parent.</returns>
        public static Process? GetParent(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            int parentPID = ImplementationServices.Adapter.GetParentProcessID(process);
            try
            {
                var parentProcess = Process.GetProcessById(parentPID);
                if (!ImplementationServices.IsValidParentProcess(parentProcess, process))
                    return null;
                return parentProcess;
            }
            catch (ArgumentException)
            {
                // "Process with an Id of NNN is not running."
                return null;
            }
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
        public static string GetImageFileName(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            var adapter = ImplementationServices.AdapterOrDefault;
            if (adapter != null)
                return adapter.GetProcessImageFileName(process);
            else
                return process.MainModule.FileName;
        }
    }
}
