// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.CompilerServices.Pal;
using Gapotchenko.FX.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Provides intrinsic compilation services.
/// </summary>
public static class Intrinsics
{
    /// <summary>
    /// Initializes intrinsic methods of the specified type.
    /// </summary>
    /// <param name="type">The type with intrinsic methods to initialize.</param>
    public static void InitializeType(
#if NET
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.NonPublicMethods)]
#endif
        Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var adapter = m_Adapter;
        if (adapter == null || m_GiveUpOnPatching)
            return;

        var arch = RuntimeInformation.ProcessArchitecture;

        var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var intrinsicAttribute =
                method.GetCustomAttributes<MachineCodeIntrinsicAttribute>(false)
                .Where(x => x.Architecture == arch || x.AdditionalArchitectures.Contains(arch))
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault(x =>
                    x.RequiredFeatures.All(IsFeatureSupported) &&
                    (x.SupportedOSPlatforms is [] || x.SupportedOSPlatforms.Any(IsOSPlatform)));

            if (intrinsicAttribute is null)
                continue;

            ValidateMethod(method);

            Adapter.PatchResult patchResult;
            try
            {
                patchResult = adapter.PatchMethod(method, intrinsicAttribute.Code);
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                // Give up on code patching if an error occurs.
                m_GiveUpOnPatching = true;

                Log.TraceSource.TraceEvent(
                    TraceEventType.Error,
                    1932901002,
                    string.Format("Unexpected error occurred during compilation of intrinsic method '{0}'. Giving up on intrinsic methods for the current environment.", method) + Environment.NewLine + e);

                return;
            }

            switch (patchResult)
            {
                case Adapter.PatchResult.Success:
                    Log.TraceSource.TraceEvent(TraceEventType.Information, 1932901000, "Intrinsic method '{0}' compiled successfully.", method);
                    break;

                case Adapter.PatchResult.UnexpectedPrologue:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901001, "Unexpected machine code prologue encountered in intrinsic method '{0}'. Compilation discarded.", method);
                    break;

                case Adapter.PatchResult.InvalidAlignment:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901006, "Unexpected machine code alignment encountered in intrinsic method '{0}'. Compilation discarded.", method);
                    break;

                case Adapter.PatchResult.NoSpace:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901006, "Not enough available space for intrinsic instructions in method '{0}'. Compilation discarded.", method);
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

    static bool m_GiveUpOnPatching;

    static bool IsOSPlatform(string osPlatform)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Create(osPlatform));
    }

    /// <summary>
    /// Determines whether a specified machine-code intrinsic feature is available to the current process.
    /// </summary>
    /// <param name="feature">The machine-code intrinsic feature to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="feature"/> is supported by the current execution environment;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The result accounts for processor support and, where applicable,
    /// the operating system support required to use the feature.
    /// Unrecognized feature values are considered unsupported.
    /// </remarks>    
    public static bool IsFeatureSupported(MachineCodeIntrinsicFeature feature)
    {
        return m_Adapter?.IsFeatureSupported(feature) ?? false;
    }

    static readonly Adapter? m_Adapter = CreateAdapter();

    static Adapter? CreateAdapter()
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
                case Architecture.X86:
                    return new Pal.OS.Windows.AdapterWindowsX86();

                case Architecture.X64:
                    return new Pal.OS.Windows.AdapterWindowsX64();

                case Architecture.Arm64:
                    return new Pal.OS.Windows.AdapterWindowsArm64();

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
