using Gapotchenko.FX.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Runtime.CompilerServices
{
    /// <summary>
    /// Provides intrinsic compilation services.
    /// </summary>
    public static unsafe class Intrinsics
    {
        static Patcher _Patcher = _CreatePatcher();

        static Patcher _CreatePatcher()
        {
            if (!CodeSafetyStrategy.UnsafeCodeRecommended)
            {
                Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901003, "Intrinsic compiler cannot be activated because code safety strategy does not recommend usage of unsafe code.");
                return null;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var arch = RuntimeInformation.ProcessArchitecture;
                switch (arch)
                {
                    case Architecture.X64:
                        return new PatcherWindowsX64();

                    default:
                        Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901004, "Intrinsic compiler does not support {0} architecture for Windows host platform.", arch);
                        break;
                }
            }
            else
            {
                Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901005, "Intrinsic compiler does not support the current host platform.");
            }

            return null;
        }

        /// <summary>
        /// Intrinsic patcher base.
        /// </summary>
        abstract class Patcher
        {
            public enum PatchResult
            {
                Success,
                UnexpectedEpilogue
            }

            public abstract PatchResult PatchMethod(MethodInfo method, byte[] code);

            protected static byte* Write(byte* dest, params byte[] data)
            {
                int size = data.Length;
                fixed (byte* src = data)
                    MemoryOperations.BlockCopy(src, dest, size);
                return dest + size;
            }
        }

        /// <summary>
        /// Intrinsic patcher for Windows OS and AMD-based 64-bit processor architecture.
        /// </summary>
        sealed class PatcherWindowsX64 : Patcher
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

                // Ensure that changes in code are atomic by using the constrained region.
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
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

                IntPtr m_Address;
                IntPtr m_Size;
                NativeMethods.Page m_OldProtect;

                public void Dispose()
                {
                    // Restore the original memory protection at the end of a scope.
                    NativeMethods.VirtualProtect(m_Address, m_Size, m_OldProtect, out _);
                }
            }

            static class NativeMethods
            {
                [Flags]
                public enum Page : uint
                {
                    NoAccess = 0x01,
                    ReadOnly = 0x02,
                    ReadWrite = 0x04,
                    WriteCopy = 0x08,
                    Execute = 0x10,
                    ExecuteRead = 0x20,
                    ExecuteReadWrite = 0x40,
                    ExecuteWriteCopy = 0x80,
                    Guard = 0x100,
                    NoCache = 0x200,
                    WriteCombine = 0x400
                }

                [DllImport("kernel32.dll", SetLastError = true)]
                public static extern bool VirtualProtect(
                    IntPtr lpAddress,
                    IntPtr dwSize,
                    Page flNewProtect,
                    out Page lpflOldProtect);
            }
        }

        /// <summary>
        /// Initializes intrinsic methods of a specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void InitializeType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var patcher = _Patcher;
            if (patcher == null)
                return;

            var arch = RuntimeInformation.ProcessArchitecture;

            var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                foreach (var attr in method.GetCustomAttributes<MachineCodeIntrinsicAttribute>(false))
                {
                    if (attr.Architecture != arch)
                        continue;

                    ValidateMethod(method);

                    Patcher.PatchResult patchResult;
                    try
                    {
                        patchResult = patcher.PatchMethod(method, attr.Code);
                    }
                    catch (Exception e) when (!e.IsControlFlowException())
                    {
                        // Give up on patching the code if an error occurs.
                        _Patcher = null;

                        Log.TraceSource.TraceEvent(
                            TraceEventType.Error,
                            1932901002,
                            string.Format("Unexpected error occurred during compilation of intrinsic method '{0}'. Giving up on intrinsics for the current environment.", method) + Environment.NewLine + e);

                        return;
                    }

                    switch (patchResult)
                    {
                        case Patcher.PatchResult.Success:
                            Log.TraceSource.TraceEvent(TraceEventType.Information, 1932901000, "Intrinsic method '{0}' compiled successfully.", method);
                            break;

                        case Patcher.PatchResult.UnexpectedEpilogue:
                            Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901001, "Unexpected machine code epilogue encountered for intrinsic method '{0}'. Compilation discarded.", method);
                            break;
                    }

                    break;
                }
            }
        }

        static void ValidateMethod(MethodInfo method)
        {
#if !NET40
            if ((method.MethodImplementationFlags & MethodImplAttributes.NoInlining) == 0)
            {
                throw new Exception(
                    string.Format(
                        "Intrinsic method '{0}' declared in type '{1}' is not marked with {2} implementation flag.",
                        method,
                        method.DeclaringType,
                        "NoInlining"));
            }
#endif
        }
    }
}
