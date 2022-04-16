using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal.MacOS
{
#if NET
    [SupportedOSPlatform("macos")]
#endif
    sealed class PalAdapter : IPalAdapter
    {
        PalAdapter()
        {
        }

        public static PalAdapter Instance { get; } = new PalAdapter();

        public int GetParentProcessId(Process process)
        {
            throw new NotImplementedException();
        }

        public string? GetProcessImageFileName(Process process) => null;

        public StringDictionary ReadProcessEnvironmentVariables(Process process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

