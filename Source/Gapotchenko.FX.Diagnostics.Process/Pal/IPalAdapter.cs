using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal
{
    interface IPalAdapter
    {
        int GetParentProcessId(Process process);
        string? GetProcessImageFileName(Process process);
        void ReadProcessCommandLineArguments(Process process, out string? commandLine, out IEnumerable<string>? arguments);
        IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process);
        Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken);
    }
}
