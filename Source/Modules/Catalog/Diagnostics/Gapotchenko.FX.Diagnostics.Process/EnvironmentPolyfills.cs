// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Diagnostics.Pal;
using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Provides polyfill extension members for <see cref="Environment"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnvironmentPolyfills
{
    extension(Environment)
    {
        /// <summary>
        /// Gets the unique identifier for the current process.
        /// </summary>
        /// <value>
        /// A number that represents the unique identifier for the current process.
        /// </value>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </remarks>
#if TFF_ENVIRONMENT_PROCESSID
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static int ProcessId
        {
            get
            {
#if TFF_ENVIRONMENT_PROCESSID
                return Environment.ProcessId;
#else
                int processId = m_CachedProcessId;
                if (processId == -1)
                {
                    m_CachedProcessId = processId = GetProcessIdCore();
                    Debug.Assert(processId != -1);
                }
                return processId;
#endif
            }
        }

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

#if !TFF_ENVIRONMENT_PROCESSID

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static int m_CachedProcessId = -1;

    static int GetProcessIdCore()
    {
        return
            PalServices.AdapterOrDefault?.GetCurrentProcessId() ??
            GetProcessIdFallback();
    }

    static int GetProcessIdFallback()
    {
        return Process.GetCurrentProcess().Id;
    }

#endif

#if !TFF_ENVIRONMENT_PROCESSPATH

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static string? m_CachedProcessPath;

    static string? GetProcessPathCore()
    {
        return
            PalServices.AdapterOrDefault?.GetProcessPath() ??
            GetProcessPathFallback();
    }

    static string? GetProcessPathFallback()
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
