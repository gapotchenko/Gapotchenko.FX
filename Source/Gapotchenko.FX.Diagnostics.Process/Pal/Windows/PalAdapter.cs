using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows
{
    sealed partial class PalAdapter : IPalAdapter
    {
        PalAdapter()
        {
        }

        public static PalAdapter Instance { get; } = new PalAdapter();

        public int GetParentProcessId(Process process)
        {
            var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();

            int size = 0;
            int status = NativeMethods.NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf(pbi), ref size);
            if (status != 0)
                throw new Exception("Unable to get parent process. NtQueryInformationProcess failed.");

            return pbi.InheritedFromUniqueProcessId.ToInt32();
        }

        public StringDictionary ReadProcessEnvironmentVariables(Process process) => ProcessEnvironment.ReadVariables(process);

        public async Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
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
                        isCurrentProcess = PalServices.IsCurrentProcess(process);
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

        public string? GetProcessImageFileName(Process process)
        {
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
