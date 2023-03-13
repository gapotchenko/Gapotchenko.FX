using Gapotchenko.FX.Diagnostics.Pal;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Provides extended operations for <see cref="Process"/> class.
/// </summary>
public static partial class ProcessExtensions
{
    /// <summary>
    /// Reads environment variables from the environment block of a process.
    /// </summary>
    /// <param name="process">The process. It can be any process running on a local machine.</param>
    /// <returns>The environment variables.</returns>
    public static StringDictionary ReadEnvironmentVariables(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        // This is a hacky way to create a StringDictionary with the correct characteristics.
        // Needs a future review.
        // One solution is to change the public contract to IReadOnlyDictionary<string, string>.
        var psi = new ProcessStartInfo();
        var env = psi.EnvironmentVariables;
        env.Clear();

        foreach (var i in PalServices.Adapter.ReadProcessEnvironmentVariables(process))
            env[i.Key] = i.Value;

        return env;
    }

    /// <summary>
    /// Reads a set of command-line arguments of the process.
    /// </summary>
    /// <param name="process">The process. It can be any process running on a local machine.</param>
    /// <returns>The set of command-line arguments of the process.</returns>
    public static string ReadArguments(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        PalServices.Adapter.ReadProcessCommandLineArguments(process, out var commandLine, out var arguments);

        if (commandLine != null)
            return commandLine;
        else if (arguments != null)
            return CommandLine.Build(arguments);
        else
            throw new InvalidOperationException();
    }

    /// <summary>
    /// Reads a sequence of command-line arguments of the process.
    /// </summary>
    /// <param name="process">The process. It can be any process running on a local machine.</param>
    /// <returns>The sequence of command-line arguments of the process.</returns>
    public static IEnumerable<string> ReadArgumentList(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        PalServices.Adapter.ReadProcessCommandLineArguments(process, out var commandLine, out var arguments);

        if (arguments != null)
            return arguments;
        else if (commandLine != null)
            return CommandLine.Split(commandLine);
        else
            throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets the parent process.
    /// </summary>
    /// <param name="process">The process to get the parent for.</param>
    /// <returns>The parent process or <see langword="null"/> if it is no longer running or there is no parent.</returns>
    public static Process? GetParent(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        try
        {
            int parentPID = PalServices.Adapter.GetParentProcessId(process);
            try
            {
                var parentProcess = Process.GetProcessById(parentPID);
                if (!ProcessHelper.IsValidParentProcess(parentProcess, process))
                    return null;
                return parentProcess;
            }
            catch (ArgumentException)
            {
                // "Process with an Id of NNN is not running."
                return null;
            }
        }
        catch (Exception e) when (!e.IsControlFlowException())
        {
            throw new Exception("Unable to get parent process.", e);
        }
    }

    /// <summary>
    /// Enumerates observable parent processes.
    /// </summary>
    /// <remarks>
    /// The closest parents are returned first.
    /// </remarks>
    /// <param name="process">The process to get the parents for.</param>
    /// <returns>The sequence of observable parent processes.</returns>
    public static IEnumerable<Process> EnumerateParents(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        for (; ; )
        {
            var parent = process.GetParent();
            if (parent == null)
                break;

            yield return parent;

            process = parent;
        }
    }

    /// <summary>
    /// Gets the file name of a process image.
    /// </summary>
    /// <remarks>
    /// Usually, the value returned by this method equals to the value returned by <see cref="ProcessModule.FileName"/> property of the main process module.
    /// The difference becomes apparent when the current process cannot access the module information due to security restrictions imposed by the host environment.
    /// While <see cref="ProcessModule.FileName"/> may not work in that situation, this method always works.
    /// </remarks>
    /// <param name="process">The process to get image file name for.</param>
    /// <returns>The file name of a process image or <see langword="null"/> if there is no associated file.</returns>
    public static string? GetImageFileName(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));

        ProcessModule? mainModule = null;
        try
        {
            mainModule = process.MainModule;
        }
        catch (InvalidOperationException)
        {
        }
        catch (Win32Exception)
        {
        }

        string? fileName = mainModule?.FileName;
        if (fileName != null)
            return fileName;

        var adapter = PalServices.AdapterOrDefault;
        if (adapter != null)
            return adapter.GetProcessImageFileName(process);

        return null;
    }
}
