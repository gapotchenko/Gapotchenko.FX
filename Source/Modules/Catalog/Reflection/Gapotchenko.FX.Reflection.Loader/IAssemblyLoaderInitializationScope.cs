// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

#if ASSEMBLY_LOADER_INITIALIZER

namespace Gapotchenko.FX.Reflection.Loader;

/// <summary>
/// Represents a logical scope of the assembly loader initialization ceremony.
/// </summary>
interface IAssemblyLoaderInitializationScope : IDisposable
{
    bool IsLazy { get; }
    void Enqueue(Action action);
    void Flush();
}

#endif
