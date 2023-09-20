#if !HAS_TARGET_PLATFORM || WINDOWS

using Gapotchenko.FX.IO.Properties;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.IO.Pal.Windows;

#if NET && !WINDOWS
[SupportedOSPlatform("windows")]
#endif
sealed class PalAdapter : IPalAdapter
{
    PalAdapter()
    {
    }

    public static PalAdapter Instance { get; } = new PalAdapter();

    public bool IsCaseSensitive => false;

    public string GetShortPath(string path)
    {
        int bufferSize = path.Length;

        var sb = new StringBuilder(bufferSize);
        int r = NativeMethods.GetShortPathName(path, sb, bufferSize);
        if (r == 0 || r > bufferSize)
            return path;

        return sb.ToString();
    }

    public string GetRealPath(string path)
    {
        using var handle = NativeMethods.CreateFile(
            path,
            0,
            NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE,
            IntPtr.Zero,
            NativeMethods.CREATION_DISPOSITION_OPEN_EXISTING,
            NativeMethods.FILE_FLAG_BACKUP_SEMANTICS,
            IntPtr.Zero);

        if (handle.IsInvalid)
        {
            throw
                Marshal.GetLastWin32Error() switch
                {
                    NativeMethods.ERROR_PATH_NOT_FOUND => new IOException(string.Format(Resources.FileSystemEntryXDoesNotExsit, path)),
                    var error => new Win32Exception(error)
                };
        }

        var buffer = new StringBuilder(10);
        for (; ; )
        {
            int result = NativeMethods.GetFinalPathNameByHandle(handle, buffer, buffer.Capacity, NativeMethods.FILE_NAME_NORMALIZED);
            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (result > buffer.Capacity)
                buffer.EnsureCapacity(result);
            else
                break;
        }

        if (buffer.Length >= SpecialPrefix.Length &&
            buffer[0] == SpecialPrefix[0] &&
            buffer[1] == SpecialPrefix[1] &&
            buffer[2] == SpecialPrefix[2] &&
            buffer[3] == SpecialPrefix[3])
        {
            if (!path.StartsWith(SpecialPrefix, StringComparison.Ordinal))
                return buffer.ToString(SpecialPrefix.Length, buffer.Length - SpecialPrefix.Length); // remove "\\?\" prefix.
        }

        return buffer.ToString();
    }

    const string SpecialPrefix = @"\\?\";
}

#endif
