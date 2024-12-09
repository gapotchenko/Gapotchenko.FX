// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Reflection.Loader;

/// <summary>
/// Represents a logical scope of assembly loader initialization.
/// </summary>
interface IAssemblyLoaderInitializationScope : IDisposable
{
    bool IsLazy { get; }
    void Enqueue(Action action);
    void Flush();
}
