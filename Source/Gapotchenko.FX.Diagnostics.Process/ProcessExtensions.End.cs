using Gapotchenko.FX.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics
{
    partial class ProcessExtensions
    {
        /// <summary>
        /// Ends a process according to the specified mode.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="mode">The mode of ending a process.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static ProcessEndMode End(this Process process, ProcessEndMode mode, int millisecondsTimeout) =>
            TaskBridge.Execute(ct => EndAsync(process, mode, millisecondsTimeout, ct));

        /// <summary>
        /// Ends a process.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static ProcessEndMode End(this Process process, int millisecondsTimeout) =>
            End(process, ProcessEndMode.Complete, millisecondsTimeout);

        /// <summary>
        /// Ends a process asynchronously according to the specified mode.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="mode">The mode of ending a process.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static async Task<ProcessEndMode> EndAsync(
            this Process process,
            ProcessEndMode mode,
            int millisecondsTimeout,
            CancellationToken cancellationToken)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");

            mode = _ConditionEndMode(mode, ProcessEndMode.Graceful, ProcessEndMode.Close | ProcessEndMode.Interrupt);

            if ((mode & ProcessEndMode.Graceful) != 0)
            {
                if ((mode & ProcessEndMode.Close) == ProcessEndMode.Close &&
                    await _TryCloseProcessAsync(process, cancellationToken).ConfigureAwait(false))
                {
                    return ProcessEndMode.Close;
                }

                if ((mode & ProcessEndMode.Interrupt) == ProcessEndMode.Interrupt &&
                    await _TryInterruptProcessAsync(process, cancellationToken).ConfigureAwait(false))
                {
                    return ProcessEndMode.Interrupt;
                }

                if (await process.WaitForExitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false))
                    return ProcessEndMode.Graceful;
            }

            mode = _ConditionEndMode(mode, ProcessEndMode.Forceful, ProcessEndMode.Kill | ProcessEndMode.Exit);

            if ((mode & ProcessEndMode.Forceful) != 0)
            {
                if ((mode & ProcessEndMode.Exit) == ProcessEndMode.Exit &&
                    _TryExitProcess(process))
                {
                    return ProcessEndMode.Exit;
                }

                if ((mode & ProcessEndMode.Kill) == ProcessEndMode.Kill &&
                    await _TryKillProcessAsync(process, cancellationToken).ConfigureAwait(false))
                {
                    return ProcessEndMode.Kill;
                }
            }

            return ProcessEndMode.None;
        }

        /// <summary>
        /// Ends a process asynchronously according to the specified mode.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="mode">The mode of ending a process.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static Task<ProcessEndMode> EndAsync(this Process process, ProcessEndMode mode, int millisecondsTimeout) =>
            EndAsync(process, mode, millisecondsTimeout, CancellationToken.None);

        /// <summary>
        /// Ends a process asynchronously.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static Task<ProcessEndMode> EndAsync(this Process process, int millisecondsTimeout) =>
            EndAsync(process, ProcessEndMode.Complete, millisecondsTimeout);

        /// <summary>
        /// Ends a process asynchronously.
        /// </summary>
        /// <param name="process">The process to end.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mode in which the process has been ended.</returns>
        public static Task<ProcessEndMode> EndAsync(this Process process, int millisecondsTimeout, CancellationToken cancellationToken) =>
            EndAsync(process, ProcessEndMode.Complete, millisecondsTimeout, cancellationToken);

        static ProcessEndMode _ConditionEndMode(ProcessEndMode mode, ProcessEndMode group, ProcessEndMode groupModes)
        {
            var mask = groupModes & ~group;
            var maskedMode = mode & mask;

            if ((mode & group) != 0)
            {
                if (maskedMode == 0)
                    mode |= mask;
            }
            else if (maskedMode != 0 && maskedMode != mask)
            {
                mode |= group;
            }

            return mode;
        }

        static async Task<bool> _TryCloseProcessAsync(Process process, CancellationToken cancellationToken)
        {
            try
            {
                if (process.MainWindowHandle == IntPtr.Zero)
                    return false;
            }
            catch (InvalidOperationException)
            {
                // The MainWindowHandle is not defined because the process has exited.
                return true;
            }

            for (int i = 0; i < 5; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    if (!process.CloseMainWindow())
                        return false;
                }
                catch (InvalidOperationException)
                {
                    // The process has already exited.
                    return true;
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                }

                if (await process.WaitForExitAsync(100, cancellationToken).ConfigureAwait(false))
                    return true;
            }

            return false;
        }

        static async Task<bool> _TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool isCurrentProcess = false;

            // Try to attach the current process to the console of another process.
            if (!NativeMethods.AttachConsole(process.Id))
            {
                // A process can be attached to at most one console.
                // Some important error codes:
                //   - If the calling process is already attached to a console, the error code returned is ERROR_ACCESS_DENIED (5).
                //   - If the specified process does not have a console, the error code returned is ERROR_INVALID_HANDLE (6).
                //   - If the specified process does not exist, the error code returned is ERROR_INVALID_PARAMETER (87).

                int error = Marshal.GetLastWin32Error();
                switch (error)
                {
                    case NativeMethods.ERROR_INVALID_PARAMETER:
                        // Process does not exist anymore.
                        return true;

                    case NativeMethods.ERROR_ACCESS_DENIED:
                        // Process is already attached to a console.
                        isCurrentProcess = _IsCurrentProcess(process);
                        if (!isCurrentProcess)
                            return false;
                        break;

                    default:
                        return false;
                }
            }
            try
            {
                if (!isCurrentProcess)
                {
                    // Disable CTRL+C handling for the current process.
                    if (!NativeMethods.SetConsoleCtrlHandler(null, true))
                    {
                        // Cannot set CTRL+C handler.
                        // Should give up now, otherwise the current process would CTRL+C itself.
                        return false;
                    }
                }

                for (int i = 0; i < 5; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        bool result = NativeMethods.GenerateConsoleCtrlEvent(NativeMethods.CTRL_C_EVENT, 0);
                        if (!result)
                            throw new Win32Exception();
                    }
                    catch (Exception e) when (!e.IsControlFlowException())
                    {
                        // Cannot send CTRL+C event for some reason.
                        return false;
                    }

                    if (await process.WaitForExitAsync(100, cancellationToken).ConfigureAwait(false))
                        return true;
                }

                return false;
            }
            finally
            {
                if (!isCurrentProcess)
                {
                    // Detach the current process from the console of another process.
                    NativeMethods.FreeConsole();

                    // Re-enable CTRL+C handling, otherwise child processes would inherit the disabled state.
                    NativeMethods.SetConsoleCtrlHandler(null, false);
                }
            }
        }

        static bool _TryExitProcess(Process process)
        {
            if (!_IsCurrentProcess(process))
                return false;
            Environment.Exit(1);
            return true;
        }

        static bool _ProcessesAreEquivalent(Process a, Process b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            return
                a.Id == b.Id &&
                a.StartTime == b.StartTime;
        }

        static bool _IsCurrentProcess(Process process) => _ProcessesAreEquivalent(process, Process.GetCurrentProcess());

        static async Task<bool> _TryKillProcessAsync(Process process, CancellationToken cancellationToken)
        {
            for (int i = 0; i < 5; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                    // The process has already exited.
                    return true;
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                }

                if (await process.WaitForExitAsync(100, cancellationToken).ConfigureAwait(false))
                    return true;
            }

            return false;
        }

    }
}
