using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Gapotchenko.FX.Diagnostics.Implementation
{
    interface IImplementationAdapter
    {
        int GetParentProcessId(Process process);
        StringDictionary ReadProcessEnvironmentVariables(Process process);
        Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken);
        string GetProcessImageFileName(Process process);
    }
}
