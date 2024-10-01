using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

partial class ProcessExtensions
{
    /// <summary>
    /// Instructs the <see cref="Process"/> component to wait asynchronously and indefinitely for the associated process to exit.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.</exception>
    /// <exception cref="SystemException">There is no process associated with this <see cref="Process"/> object.</exception>
    /// <exception cref="SystemException">You are attempting to call <see cref="WaitForExitAsync(Process, CancellationToken)"/> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
    /// <exception cref="Win32Exception">The wait setting could not be accessed.</exception>
#if NET5_0_OR_GREATER
    public static Task WaitForExitAsync(Process process, CancellationToken cancellationToken = default) =>
        (process ?? throw new ArgumentNullException(nameof(process)))
        .WaitForExitAsync(cancellationToken);
#else
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default) =>
        process.WaitForExitAsync(Timeout.Infinite, cancellationToken);
#endif

    /// <summary>
    /// Instructs the <see cref="Process"/> component to asynchronously wait the specified number of milliseconds for the associated process to exit.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to exit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> if the associated process has exited; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an infinite timeout.</exception>
    /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.</exception>
    /// <exception cref="SystemException">There is no process associated with this <see cref="Process"/> object.</exception>
    /// <exception cref="SystemException">You are attempting to call <see cref="WaitForExitAsync(Process, int, CancellationToken)"/> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
    /// <exception cref="Win32Exception">The wait setting could not be accessed.</exception>
    public static Task<bool> WaitForExitAsync(this Process process, int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        // IMPROVE: Needs a more efficient implementation without polling.

        return Poll.WaitUntilAsync(
            () => Task.FromResult(process.WaitForExit(0)), // this is not a call to Process.HasExited property to preserve a desired exception semantics
            100,
            millisecondsTimeout,
            cancellationToken);
    }

    /// <summary>
    /// Instructs the <see cref="Process"/> component to wait the specified number of milliseconds for the associated process to exit.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to exit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an infinite timeout.</exception>
    /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.</exception>
    /// <exception cref="SystemException">There is no process associated with this <see cref="Process"/> object.</exception>
    /// <exception cref="SystemException">You are attempting to call <see cref="WaitForExit(Process, int, CancellationToken)"/> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
    /// <exception cref="Win32Exception">The wait setting could not be accessed.</exception>
    public static void WaitForExit(this Process process, int millisecondsTimeout, CancellationToken cancellationToken) =>
        TaskBridge.Execute(WaitForExitAsync(process, millisecondsTimeout, cancellationToken));

    /// <summary>
    /// Instructs the <see cref="Process"/> component to wait indefinitely for the associated process to exit.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.</exception>
    /// <exception cref="SystemException">There is no process associated with this <see cref="Process"/> object.</exception>
    /// <exception cref="SystemException">You are attempting to call <see cref="WaitForExit(Process, CancellationToken)"/> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
    /// <exception cref="Win32Exception">The wait setting could not be accessed.</exception>
    public static void WaitForExit(this Process process, CancellationToken cancellationToken) =>
        WaitForExit(process, Timeout.Infinite, cancellationToken);
}
