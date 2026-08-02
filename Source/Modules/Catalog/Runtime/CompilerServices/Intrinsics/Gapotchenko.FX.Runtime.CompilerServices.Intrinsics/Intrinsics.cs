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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Provides intrinsic compilation services.
/// </summary>
public static class Intrinsics
{
    /// <summary>
    /// Gets or sets the activation mode of the intrinsic compiler.
    /// </summary>
    /// <remarks>
    /// The mode affects subsequent calls to <see cref="InitializeType(Type)"/> and should be configured
    /// before intrinsic types are initialized. Changing the mode does not revert intrinsic methods that
    /// have already been compiled.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is not a defined <see cref="IntrinsicsActivationMode"/> value.
    /// </exception>
    public static IntrinsicsActivationMode ActivationMode
    {
        get => field;
        set
        {
            if (value is not (>= IntrinsicsActivationMode.Auto and <= IntrinsicsActivationMode.PreferablyOn))
                throw new ArgumentOutOfRangeException(nameof(value));
            field = value;
        }
    }

    /// <summary>
    /// Initializes intrinsic methods of the specified type.
    /// </summary>
    /// <param name="type">The type with intrinsic methods to initialize.</param>
    /// <remarks>
    /// <para>
    /// This method must be called exactly once, from the static constructor of
    /// <paramref name="type"/>, before any intrinsic method declared by that type can be invoked.
    /// </para>
    /// <para>
    /// Calling this method more than once for the same type, or calling it while an intrinsic
    /// method of that type can be executing on another thread, results in undefined behavior.
    /// </para>
    /// </remarks>
    public static void InitializeType(
#if NET
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.NonPublicMethods)]
#endif
        Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!IsActive())
            return;

        var adapter = m_Adapter;
        if (adapter == null)
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

            ValidateMethod(method, intrinsicAttribute);
#if NET
            WarnAboutTieredCompilation(method);
#endif

            Adapter.PatchResult patchResult;

            lock (m_PatchingLock)
            {
                if (m_GiveUpOnPatching)
                    return;

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
                        string.Concat(
                            string.Format(
                                "Unexpected error occurred during compilation of intrinsic method '{0}' declared in type '{1}'. Giving up on intrinsic methods for the current environment.",
                                method,
                                method.DeclaringType),
                            Environment.NewLine,
                            e.ToString()));

                    return;
                }

                if (patchResult is Adapter.PatchResult.WriteProtected)
                    m_GiveUpOnPatching = true;
            }

            switch (patchResult)
            {
                case Adapter.PatchResult.Success:
                    Log.TraceSource.TraceEvent(TraceEventType.Information, 1932901000, "Intrinsic method '{0}' declared in type '{1}' compiled successfully.", method, method.DeclaringType);
                    break;

                case Adapter.PatchResult.UnexpectedPrologue:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901001, "Unexpected machine code prologue encountered in intrinsic method '{0}' declared in type '{1}'. Compilation discarded.", method, method.DeclaringType);
                    break;

                case Adapter.PatchResult.InvalidAlignment:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901006, "Unexpected machine code alignment encountered in intrinsic method '{0}' declared in type '{1}'. Compilation discarded.", method, method.DeclaringType);
                    break;

                case Adapter.PatchResult.NoSpace:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901007, "Not enough available space for intrinsic instructions in method '{0}' declared in type '{1}'. Compilation discarded.", method, method.DeclaringType);
                    break;

                case Adapter.PatchResult.WriteProtected:
                    Log.TraceSource.TraceEvent(
                        TraceEventType.Error,
                        1932901008,
                        "Intrinsic method code cannot be made writable on the current platform. Giving up on intrinsic compilation for the current environment.");
                    return;

                case Adapter.PatchResult.UnsupportedUnwindPrologue:
                    Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901009, "Unsupported unwind prologue encountered in intrinsic method '{0}' declared in type '{1}'. Compilation discarded.", method, method.DeclaringType);
                    break;
            }
        }

        static void ValidateMethod(MethodInfo method, MachineCodeIntrinsicAttribute attribute)
        {
            if ((method.MethodImplementationFlags & MethodImplAttributes.NoInlining) == 0)
            {
                throw CreateInvalidMethodException(
                    method,
                    string.Format(
                        "is not marked with {0} (or {1}) implementation flag",
                        $"{nameof(MethodImplAttributes)}.{nameof(MethodImplAttributes.NoInlining)}",
                        $"{nameof(Intrinsics)}.{nameof(MethodImplOptions)}"));
            }

            if (!method.IsStatic)
                throw CreateInvalidMethodException(method, "is not static");

            if (method.ContainsGenericParameters)
                throw CreateInvalidMethodException(method, "contains unassigned generic parameters");

            if (method.IsAbstract)
                throw CreateInvalidMethodException(method, "is abstract");

            if ((method.Attributes & MethodAttributes.PinvokeImpl) != 0)
                throw CreateInvalidMethodException(method, "is a platform invocation method");

            if ((method.MethodImplementationFlags & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL)
                throw CreateInvalidMethodException(method, "is not implemented in IL");

            if ((method.MethodImplementationFlags & MethodImplAttributes.InternalCall) != 0)
                throw CreateInvalidMethodException(method, "is an internal-call method");

            byte[] code =
                attribute.Code ??
                throw CreateInvalidMethodException(method, "has null intrinsic machine code");

            if (code.Length == 0 && method.ReturnType != typeof(void))
                throw CreateInvalidMethodException(method, "has empty intrinsic machine code despite returning a value");

            static Exception CreateInvalidMethodException(MethodInfo method, string reason)
            {
                return new InvalidOperationException(
                    string.Format(
                        "Intrinsic method '{0}' declared in type '{1}' {2}.",
                        method,
                        method.DeclaringType,
                        reason));
            }
        }

#if NET
        static void WarnAboutTieredCompilation(MethodInfo method)
        {
            if ((method.MethodImplementationFlags & MethodImplAttributes.AggressiveOptimization) != 0)
                return;

            Log.TraceSource.TraceEvent(
                TraceEventType.Warning,
                1932901010,
                "Intrinsic method '{0}' declared in type '{1}' is not marked with the {2} implementation flag. Tiered compilation may replace the patched method implementation with a new compilation tier. Use {3} for the complete set of recommended implementation options.",
                method,
                method.DeclaringType,
                nameof(MethodImplOptions.AggressiveOptimization),
                $"{nameof(Intrinsics)}.{nameof(MethodImplOptions)}");
        }
#endif
    }

    static readonly Lock m_PatchingLock = new();

    static bool IsActive()
    {
        return
            // The m_GiveUpOnPatching flag is checked authoritatively under m_PatchingLock further in the code.
            // Here it's used as a fast-path check only.
            !m_GiveUpOnPatching &&
            ActivationMode switch
            {
                IntrinsicsActivationMode.Auto => CodeSafetyStrategy.UnsafeCodeRecommended,
                IntrinsicsActivationMode.AlwaysOff => false,
                IntrinsicsActivationMode.PreferablyOn => CodeSafetyStrategy.UnsafeCodeAllowed
            };
    }

    static bool m_GiveUpOnPatching;

    static bool IsOSPlatform(string platform)
    {
        OSPlatform osPlatform;
        if (string.Equals(platform, "windows", StringComparison.OrdinalIgnoreCase))
            osPlatform = OSPlatform.Windows;
        else if (string.Equals(platform, "linux", StringComparison.OrdinalIgnoreCase))
            osPlatform = OSPlatform.Linux;
        else if (string.Equals(platform, "macos", StringComparison.OrdinalIgnoreCase))
            osPlatform = OSPlatform.OSX;
        else
            osPlatform = OSPlatform.Create(platform);

        return RuntimeInformation.IsOSPlatform(osPlatform);
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
            var architecture = RuntimeInformation.ProcessArchitecture;
            switch (architecture)
            {
                case Architecture.X86:
                    return new Pal.OS.Windows.AdapterWindowsX86();

                case Architecture.X64:
                    return new Pal.OS.Windows.AdapterWindowsX64();

                case Architecture.Arm64:
                    return new Pal.OS.Windows.AdapterWindowsArm64();

                default:
                    LogUnsupportedArchitecture(architecture, "Windows");
                    break;
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var architecture = RuntimeInformation.ProcessArchitecture;
            switch (architecture)
            {
                case Architecture.Arm:
                    return new Pal.OS.Linux.AdapterLinuxArm32();

                case Architecture.X86:
                    return new Pal.OS.Linux.AdapterLinuxX86();

                case Architecture.X64:
                    return new Pal.OS.Linux.AdapterLinuxX64();

                case Architecture.Arm64:
                    return new Pal.OS.Linux.AdapterLinuxArm64();

                default:
                    LogUnsupportedArchitecture(architecture, "Linux");
                    break;
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var architecture = RuntimeInformation.ProcessArchitecture;
            switch (architecture)
            {
                case Architecture.X64:
                    return new Pal.OS.MacOS.AdapterMacOSX64();

                case Architecture.Arm64:
                    return new Pal.OS.MacOS.AdapterMacOSArm64();

                default:
                    LogUnsupportedArchitecture(architecture, "macOS");
                    break;
            }
        }
        else
        {
            Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901005, "Intrinsic compiler does not support the current host platform '{0}'.", RuntimeInformation.OSDescription);
        }

        return null;
    }

    static void LogUnsupportedArchitecture(Architecture architecture, string platform)
    {
        Log.TraceSource.TraceEvent(
            TraceEventType.Verbose,
            1932901004,
            "Intrinsic compiler does not support {0} architecture for {1} host platform.",
            architecture,
            platform);
    }

    /// <summary>
    /// Gets the <see cref="TraceSource"/> instance used for intrinsic compiler diagnostics.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TraceSource TraceSource => Log.TraceSource;

    /// <summary>
    /// Gets the implementation options that should be set on an intrinsic method using <see cref="MethodImplAttribute"/>.
    /// </summary>
    public const MethodImplOptions MethodImplOptions =
        MethodImplOptions.NoInlining
#if NET
        | MethodImplOptions.AggressiveOptimization // prevent tiered compilation
#endif
        ;
}
