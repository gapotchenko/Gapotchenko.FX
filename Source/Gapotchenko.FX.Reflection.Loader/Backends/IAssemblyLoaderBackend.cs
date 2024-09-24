// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

interface IAssemblyLoaderBackend : IDisposable
{
    string? ResolveAssemblyPath(AssemblyName assemblyName);

    string? ResolveUnmanagedDllPath(string unmanagedDllName);
}
