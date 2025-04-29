using Gapotchenko.FX.Runtime.CompilerServices.Pal;
using Gapotchenko.FX.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Provides intrinsic compilation services.
/// </summary>
public static unsafe class Intrinsics
{
    /// <summary>
    /// Initializes intrinsic methods of the specified type.
    /// </summary>
    /// <param name="type">The type with intrinsic methods to initialize.</param>
    public static void InitializeType(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var patcher = m_Patcher;
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
                    // Give up on code patching if an error occurs.
                    m_Patcher = null;

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
                        Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901001, "Unexpected machine code epilogue encountered in intrinsic method '{0}'. Compilation discarded.", method);
                        break;
                }

                break;
            }
        }

        static void ValidateMethod(MethodInfo method)
        {
            if ((method.MethodImplementationFlags & MethodImplAttributes.NoInlining) == 0)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Intrinsic method '{0}' declared in type '{1}' is not marked with {2} implementation flag.",
                        method,
                        method.DeclaringType,
                        nameof(MethodImplAttributes.NoInlining)));
            }
        }
    }

    static Patcher? m_Patcher = CreatePatcher();

    static Patcher? CreatePatcher()
    {
        if (!CodeSafetyStrategy.UnsafeCodeRecommended)
        {
            Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901003, "Intrinsic compiler is not activated because code safety strategy does not recommend unsafe code usage.");
            return null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var arch = RuntimeInformation.ProcessArchitecture;
            switch (arch)
            {
                case Architecture.X64:
                    return new Pal.Windows.PatcherWindowsX64();

                default:
                    Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901004, "Intrinsic compiler does not support {0} architecture for {1} host platform.", arch, "Windows");
                    break;
            }
        }
        else
        {
            Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901005, "Intrinsic compiler does not support the current host platform '{0}'.", RuntimeInformation.OSDescription);
        }

        return null;
    }
}
