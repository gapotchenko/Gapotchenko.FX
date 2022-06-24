using System;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

interface IAssemblyLoaderBackend : IDisposable
{
    string? ResolveAssemblyPath(AssemblyName assemblyName);

    string? ResolveUnmanagedDllPath(string unmanagedDllName);
}
