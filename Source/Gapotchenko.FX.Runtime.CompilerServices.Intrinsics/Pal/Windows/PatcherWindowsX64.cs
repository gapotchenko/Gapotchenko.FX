using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

/// <summary>
/// Intrinsic patcher for Windows OS and AMD-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed unsafe class PatcherWindowsX64 : Patcher
{
    static byte* _GetPointerToMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        var p = (byte*)method.MethodHandle.GetFunctionPointer();

        p = _SkipJumps(p);

        return p;
    }

    static byte* _SkipJumps(byte* p)
    {
        while (*p == 0xe9)
        {
            var delta = *(int*)(p + 1) + 5;
            p += delta;
        }
        return p;
    }

    static readonly byte[][] _Prologues = new[]
    {
            new byte[] { 0x48, 0x83, 0xec, 0x18, 0x48, 0x89, 0x34, 0x24 }, // Mono 5.18.1, x64
            new byte[] { 0x48, 0x83, 0xec, 0x28 },
            new byte[] { 0x48, 0x89, 0x54, 0x24 },
            new byte[] { 0x53, 0x48, 0x83, 0xec, 0x20 },
            new byte[] { 0x55, 0x48, 0x83, 0xec, 0x20 },
            new byte[] { 0x55, 0x57, 0x56, 0x48, 0x83, 0xec, 0x30 },
            new byte[] { 0x56, 0x48, 0x83, 0xec, 0x20 },
            new byte[] { 0x57, 0x56, 0x48, 0x83, 0xec, 0x28 }, // Windows 10 x64, NGen 4.7.2
        };

    static bool _IsSupportedPrologue(byte* buffer)
    {
        foreach (var prologue in _Prologues)
        {
            var match = true;
            for (var i = 0; i < prologue.Length; i++)
            {
                if (buffer[i] != prologue[i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return true;
        }
        return false;
    }

    public override PatchResult PatchMethod(MethodInfo method, byte[] code)
    {
        var p = _GetPointerToMethodInstructions(method);
        if (!_IsSupportedPrologue(p))
            return PatchResult.UnexpectedEpilogue;

        int codeSize = code.Length;

#if TFF_CER
        // Ensure that code changes are atomic by using the constrained region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            // Temporarily allow memory modification in order to apply the intrinsic code.
            using (var scope = new VirtualProtectScope(p, codeSize + 1, NativeMethods.Page.ExecuteReadWrite))
            {
                // Put the intrinsic code.
                p = Write(p, code);

                // End method with a 'ret' instruction.
                Write(p, 0xc3);
            }
        }

        return PatchResult.Success;
    }

    struct VirtualProtectScope : IDisposable
    {
        public VirtualProtectScope(void* address, int size, NativeMethods.Page protect)
        {
            m_Address = new IntPtr(address);
            m_Size = new IntPtr(size);

            if (!NativeMethods.VirtualProtect(m_Address, m_Size, protect, out m_OldProtect))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        readonly IntPtr m_Address;
        readonly IntPtr m_Size;
        readonly NativeMethods.Page m_OldProtect;

        public void Dispose()
        {
            // Restore the original memory protection at the end of a scope.
            NativeMethods.VirtualProtect(m_Address, m_Size, m_OldProtect, out _);
        }
    }
}
