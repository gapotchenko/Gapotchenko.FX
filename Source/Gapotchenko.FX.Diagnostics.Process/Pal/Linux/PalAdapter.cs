#if !HAS_TARGET_PLATFORM || LINUX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static System.FormattableString;

namespace Gapotchenko.FX.Diagnostics.Pal.Linux;

#if NET && !LINUX
[SupportedOSPlatform("linux")]
#endif
sealed class PalAdapter : IPalAdapter
{ 
    PalAdapter()
    {
    }

    public static PalAdapter Instance { get; } = new PalAdapter();

    static string GetProcEntryFilePath(int pid, string name) => Invariant($"/proc/{pid}/{name}");

    public int GetParentProcessId(Process process)
    {
        string filePath = GetProcEntryFilePath(process.Id, "status");
        using var tr = File.OpenText(filePath);

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

        throw new Exception(string.Format("PPid entry not found in \"{0}\" file.", filePath));
    }

    public string? GetProcessImageFileName(Process process) => null;

    public void ReadProcessCommandLineArguments(Process process, out string? commandLine, out IEnumerable<string>? arguments)
    {
        commandLine = null;
        arguments = ReadArguments(process.Id);
    }

    static IEnumerable<string> ReadArguments(int pid)
    {
        using var br = new ProcessBinaryReader(
            File.OpenRead(GetProcEntryFilePath(pid, "cmdline")),
            Encoding.UTF8);

        for (; ; )
        {
            if (br.PeekChar() == -1)
            {
                // EOF
                break;
            }

            yield return br.ReadCString();
        }
    }

    public IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process)
    {
        using var br = new ProcessBinaryReader(
            File.OpenRead(GetProcEntryFilePath(process.Id, "environ")),
            Encoding.UTF8);

        var env = new Dictionary<string, string>(StringComparer.InvariantCulture);

        for (; ; )
        {
            if (br.PeekChar() == -1)
            {
                // End of environment block.
                break;
            }

            var s = br.ReadCString();

            int j = s.IndexOf('=');
            if (j <= 0)
                continue;

            string name = s.Substring(0, j);
            string value = s.Substring(j + 1);

            env[name] = value;
        }

        return env;
    }

    public async Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken)
    {
        int pid = process.Id;

        for (int i = 0; i < 5; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int result = NativeMethods.kill(pid, NativeMethods.SIGINT);
            if (result == -1)
                return false;

            if (await process.WaitForExitAsync(100, cancellationToken).ConfigureAwait(false))
                return true;
        }

        return false;
    }
}

#endif
