using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Gapotchenko.FX.Diagnostics
{
    partial class ProcessExtensions
    {
        /// <summary>
        /// Instructs the <see cref="Process"/> component to asynchronously wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="millisecondsTimeout">The amount of time, in milliseconds, to wait for the associated process to exit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>true</c> if the associated process has exited; otherwise, <c>false</c>.</returns>
        public static Task<bool> WaitForExitAsync(this Process process, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            // IMPROVE: Needs a more efficient implementation without polling.

            return Poll.WaitUntilAsync(
                () =>
                {
                    // This is not a call to Process.HasExited property in order to inherit the desired exception behavior.
                    return
#if NET40
                        TaskEx
#else
                        Task
#endif
                        .FromResult(process.WaitForExit(0));
                },
                100,
                millisecondsTimeout);
        }

        /// <summary>
        /// Instructs the <see cref="Process"/> component to wait asynchronously and indefinitely for the associated process to exit.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default) =>
            process.WaitForExitAsync(Timeout.Infinite, cancellationToken);

        /// <summary>
        /// Instructs the <see cref="Process"/> component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void WaitForExit(this Process process, int milliseconds, CancellationToken cancellationToken) =>
            TaskBridge.Execute(WaitForExitAsync(process, milliseconds, cancellationToken));

        /// <summary>
        /// Instructs the <see cref="Process"/> component to wait indefinitely for the associated process to exit.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void WaitForExit(this Process process, CancellationToken cancellationToken) =>
            WaitForExit(process, Timeout.Infinite, cancellationToken);
    }
}
