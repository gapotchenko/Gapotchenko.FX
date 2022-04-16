using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics.Pal.Linux
{
#if NET
    [SupportedOSPlatform("linux")]
#endif
    sealed class PalAdapter : IPalAdapter
    {
        PalAdapter()
        {
        }

        public static PalAdapter Instance { get; } = new PalAdapter();

        public int GetParentProcessId(Process process)
        {
            using var tr = File.OpenText($"/proc/{process.Id}/status");

            for (; ; )
            {
                var line = tr.ReadLine();
                if (line == null)
                    break;

                const string key = "PPid:";
                if (line.StartsWith(key, StringComparison.Ordinal))
                {
#if TFF_MEMORY
                    var s = line.AsSpan(key.Length).Trim();
#else
                    var s = line.Substring(key.Length).Trim();
#endif
                    return int.Parse(s, provider: NumberFormatInfo.InvariantInfo);
                }
            }

            throw new Exception("Cannot determine parent process ID.");
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
