#if !HAS_TARGET_PLATFORM || WINDOWS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows;

#if NET && !WINDOWS
[SupportedOSPlatform("windows")]
#endif
sealed class PalAdapter : IPalAdapter
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
            throw new Exception("NtQueryInformationProcess failed.");

        return pbi.InheritedFromUniqueProcessId.ToInt32();
    }

    public string GetProcessImageFileName(Process process)
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
                    // Guesswork is needed because dwSize is not updated with the required buffer size.

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

    public void ReadProcessCommandLineArguments(Process process, out string? commandLine, out IEnumerable<string>? arguments)
    {
        arguments = null;
        commandLine = ProcessEnvironment.ReadCommandLine(process.Handle);
    }

    public IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process) =>
        ProcessEnvironment.ReadVariables(process.Handle);

    public async Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
    {
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
                    isCurrentProcess = ProcessHelper.IsCurrentProcess(process);
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
                        return false;
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
}

#endif
