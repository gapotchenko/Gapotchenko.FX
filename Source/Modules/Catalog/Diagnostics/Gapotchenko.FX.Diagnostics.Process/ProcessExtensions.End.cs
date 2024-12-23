﻿using Gapotchenko.FX.Diagnostics.Pal;
using Gapotchenko.FX.Threading.Tasks;
using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

partial class ProcessExtensions
{
    const int DefaultEndTimeout = 3000;

    /// <summary>
    /// Ends a process with a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static ProcessEndMode End(this Process process) => End(process, DefaultEndTimeout);

    /// <summary>
    /// Ends a process with a specified timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static ProcessEndMode End(this Process process, int millisecondsTimeout) =>
        End(process, ProcessEndMode.Complete, millisecondsTimeout);

    /// <summary>
    /// Ends a process using the specified mode with a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static ProcessEndMode End(this Process process, ProcessEndMode mode) => End(process, mode, DefaultEndTimeout);

    /// <summary>
    /// Ends a process using the specified mode and timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static ProcessEndMode End(this Process process, ProcessEndMode mode, int millisecondsTimeout) =>
        TaskBridge.Execute(ct => EndAsync(process, mode, millisecondsTimeout, ct));

    /// <summary>
    /// Ends a process asynchronously using the specified mode and a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, ProcessEndMode mode, CancellationToken cancellationToken) =>
        EndAsync(process, mode, DefaultEndTimeout, cancellationToken);

    /// <summary>
    /// Ends a process asynchronously using the specified mode and a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, ProcessEndMode mode) => EndAsync(process, mode, DefaultEndTimeout);

    /// <summary>
    /// Ends a process asynchronously with a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process) => EndAsync(process, DefaultEndTimeout);

    /// <summary>
    /// Ends a process asynchronously with the specified timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, int millisecondsTimeout) =>
        EndAsync(process, ProcessEndMode.Complete, millisecondsTimeout);

    /// <summary>
    /// Ends a process asynchronously using the specified mode and timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, ProcessEndMode mode, int millisecondsTimeout) =>
        EndAsync(process, mode, millisecondsTimeout, CancellationToken.None);

    /// <summary>
    /// Ends a process asynchronously with a default timeout.
    /// </summary>
    /// <remarks>
    /// The default timeout is 3 seconds.
    /// </remarks>
    /// <param name="process">The process to end.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, CancellationToken cancellationToken) =>
        EndAsync(process, DefaultEndTimeout, cancellationToken);

    /// <summary>
    /// Ends a process asynchronously with the specified timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    public static Task<ProcessEndMode> EndAsync(this Process process, int millisecondsTimeout, CancellationToken cancellationToken) =>
        EndAsync(process, ProcessEndMode.Complete, millisecondsTimeout, cancellationToken);

    /// <summary>
    /// Ends a process asynchronously using the specified mode and timeout.
    /// </summary>
    /// <param name="process">The process to end.</param>
    /// <param name="mode">The mode of ending a process.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to end.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The mode in which the process has been ended.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an infinite timeout.</exception>
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

        mode = InterpretEndMode(mode, ProcessEndMode.Graceful, ProcessEndMode.Close | ProcessEndMode.Interrupt);

        if ((mode & ProcessEndMode.Graceful) != 0)
        {
            if ((mode & ProcessEndMode.Close) == ProcessEndMode.Close &&
                await TryCloseProcessAsync(process, cancellationToken).ConfigureAwait(false))
            {
                return ProcessEndMode.Close;
            }

            if ((mode & ProcessEndMode.Interrupt) == ProcessEndMode.Interrupt &&
                await TryInterruptProcessAsync(process, cancellationToken).ConfigureAwait(false))
            {
                return ProcessEndMode.Interrupt;
            }

            if (await process.WaitForExitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false))
                return ProcessEndMode.Graceful;
        }

        mode = InterpretEndMode(mode, ProcessEndMode.Forceful, ProcessEndMode.Kill | ProcessEndMode.Exit);

        if ((mode & ProcessEndMode.Forceful) != 0)
        {
            if ((mode & ProcessEndMode.Exit) == ProcessEndMode.Exit &&
                TryExitProcess(process))
            {
                return ProcessEndMode.Exit;
            }

            if ((mode & ProcessEndMode.Kill) == ProcessEndMode.Kill &&
                await TryKillProcessAsync(process, cancellationToken).ConfigureAwait(false))
            {
                return ProcessEndMode.Kill;
            }
        }

        return ProcessEndMode.None;
    }

    static ProcessEndMode InterpretEndMode(ProcessEndMode mode, ProcessEndMode group, ProcessEndMode groupModes)
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

    static async Task<bool> TryCloseProcessAsync(Process process, CancellationToken cancellationToken)
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

    static async Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var adapter = PalServices.AdapterOrDefault;
        if (adapter != null)
            return await adapter.TryInterruptProcessAsync(process, cancellationToken).ConfigureAwait(false);
        else
            return false;
    }

    static bool TryExitProcess(Process process)
    {
        if (!ProcessHelper.IsCurrentProcess(process))
            return false;
        Environment.Exit(1);
        return true;
    }

    static async Task<bool> TryKillProcessAsync(Process process, CancellationToken cancellationToken)
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
