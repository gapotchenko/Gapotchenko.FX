// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics.Pal;

interface IPalAdapter
{
    int GetParentProcessId(Process process);

    string? GetProcessImageFileName(Process process);

    void ReadProcessCommandLineArguments(Process process, out string? commandLine, out IEnumerable<string>? arguments);

    IReadOnlyDictionary<string, string> ReadProcessEnvironmentVariables(Process process);

    Task<bool> TryInterruptProcessAsync(Process process, CancellationToken cancellationToken);

#if !TFF_ENVIRONMENT_PROCESSPATH
    string? GetProcessPath();
#endif
}
