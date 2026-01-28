// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET6_0_OR_GREATER
#define TFF_ENVIRONMENT_PROCESSPATH
#endif

using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Provides polyfill extension methods for <see cref="Environment"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnvironmentPolyfills
{
    extension(Environment)
    {
        /// <summary>
        /// Returns the path of the executable that started the currently executing process.
        /// Returns <see langword="null"/> when the path is not available.
        /// </summary>
        /// <returns>The path of the executable that started the currently executing process.</returns>
        /// <remarks>
        /// <para>
        /// If the executable is renamed or deleted before this property is first accessed, the return value is undefined and depends on the operating system.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </para>
        /// </remarks>
#if TFF_ENVIRONMENT_PROCESSPATH
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static string? ProcessPath =>
#if TFF_ENVIRONMENT_PROCESSPATH
            Environment.ProcessPath;
#else
            Empty.Nullify(m_CachedProcessPath ??= (GetProcessPathCore() ?? string.Empty));
#endif
    }

#if !TFF_ENVIRONMENT_PROCESSPATH

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static string? m_CachedProcessPath;

    static string? GetProcessPathCore()
    {
        var currentProcess = Process.GetCurrentProcess();

        ProcessModule? mainModule;
        try
        {
            mainModule = currentProcess.MainModule;
        }
        catch (Exception e) when (e is PlatformNotSupportedException or NotSupportedException or InvalidOperationException or Win32Exception)
        {
            mainModule = null;
        }

        return mainModule?.FileName;
    }

#endif
}
