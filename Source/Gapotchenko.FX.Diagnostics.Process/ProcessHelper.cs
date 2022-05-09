using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics
{
    static class ProcessHelper
    {
        static bool ProcessesAreEquivalent(Process? a, Process? b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;

            return
                a.Id == b.Id &&
                a.StartTime == b.StartTime;
        }

        public static bool IsCurrentProcess(Process? process) => ProcessesAreEquivalent(process, Process.GetCurrentProcess());

        public static bool IsValidParentProcess(Process parentProcess, Process childProcess)
        {
            if (parentProcess.StartTime > childProcess.StartTime)
            {
                // The parent process was started after the child process.
                // This condition indicates that a real parent process has exited before, and
                // its process ID has been reused by another unrelated process.
                return false;
            }

            return true;
        }
    }
}
