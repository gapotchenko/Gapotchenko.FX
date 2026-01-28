// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !HAS_TARGET_PLATFORM || MACOS

using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Diagnostics.Pal.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
sealed class PalAdapter : IPalAdapter
{
    PalAdapter()
    {
    }

    public static PalAdapter Instance { get; } = new PalAdapter();

    public unsafe int GetParentProcessId(Process process)
    {
        if (!Environment.Is64BitProcess)
            throw new PlatformNotSupportedException();

        const int infoSize = 648; // sizeof(kinfo_proc)
        byte* info = stackalloc byte[infoSize]; // kinfo_proc
        nint infoLength = infoSize;

        int[] mib = [NativeMethods.CTL_KERN, NativeMethods.KERN_PROC, NativeMethods.KERN_PROC_PID, process.Id];
        if (NativeMethods.sysctl(mib, mib.Length, info, &infoLength, null, 0) < 0)
            throw new Exception("sysctl for KERN_PROC failed.");
        if (infoLength == IntPtr.Zero)
            throw new Exception("sysctl returned an unexpected attribute length for KERN_PROC.");

        return *(int*)(info + 560); // info.kp_eproc.e_ppid
    }

    public string? GetProcessImageFileName(Process process) => null;

    public void ReadProcessCommandLineArguments(Process process, out string? commandLine, out IEnumerable<string>? arguments)
    {
        commandLine = null;

        var br = GetProcArgs2BinaryReader(process.Id);
        arguments = ReadArguments(br);
    }

    public IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process)
    {
        var br = GetProcArgs2BinaryReader(process.Id);

        // Skip process arguments.
        foreach (string i in ReadArguments(br))
            _ = i;

        var env = new Dictionary<string, string>(StringComparer.InvariantCulture);

        if (br.PeekChar() == -1)
            return env; // EOF

        // Read process environment variables.
        for (; ; )
        {
            string s = br.ReadCString();
            if (s.Length == 0)
            {
                // End of environment variables block.
                break;
            }

            int j = s.IndexOf('=');
            if (j <= 0)
                continue;

            string name = s[..j];
            string value = s[(j + 1)..];

            env[name] = value;
        }

        return env;
    }

    static ProcessBinaryReader GetProcArgs2BinaryReader(int pid)
    {
        byte[] procArgs = GetProcArgs2(pid);
        return new ProcessBinaryReader(
            new MemoryStream(procArgs, false),
            Encoding.UTF8);
    }

    static unsafe byte[] GetProcArgs2(int pid)
    {
        int[] mib = new int[3];
        mib[0] = NativeMethods.CTL_KERN;
        mib[1] = NativeMethods.KERN_ARGMAX;

        int argMax = 0;
        nint length = sizeof(int);

        if (NativeMethods.sysctl(mib, 2, &argMax, &length, null, 0) == -1)
            throw new Exception("sysctl for KERN_ARGMAX failed.");

        byte[] procArgs = new byte[argMax];

        mib[1] = NativeMethods.KERN_PROCARGS2;
        mib[2] = pid;

        length = argMax;

        fixed (byte* p = procArgs)
        {
            if (NativeMethods.sysctl(mib, 3, p, &length, null, 0) == -1)
                throw new Exception("sysctl for KERN_PROCARGS2 failed.");
        }

        return procArgs;
    }

    static IEnumerable<string> ReadArguments(ProcessBinaryReader br)
    {
        int argc = br.ReadInt32();

        _ = br.ReadCString(); // exec_path

        // Skip zeros.
        SkipZeroChars(br);

        // Read command-line arguments.
        for (int i = 0; i < argc; ++i)
            yield return br.ReadCString();
    }

    static void SkipZeroChars(BinaryReader br)
    {
        for (; ; )
        {
            int c = br.Read();
            if (c == -1)
                break;
            if (c == 0)
                continue;
            --br.BaseStream.Position;
            break;
        }
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

#if !TFF_ENVIRONMENT_PROCESSPATH
    public string? GetProcessPath() => null;
#endif
}

#endif
