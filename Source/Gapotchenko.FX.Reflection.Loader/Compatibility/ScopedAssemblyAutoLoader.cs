using Gapotchenko.FX.Reflection.Loader;
using Gapotchenko.FX.Reflection.Loader.Backends;
using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gapotchenko.FX.Reflection
{
    /// <summary>
    /// Provides services with a controlled lifespan for automatic assembly resolution and dynamic loading based on specified probing paths, binding redirects and common sense heuristics.
    /// </summary>
    [Obsolete("Use AssemblyAutoLoader instead.")]
    public sealed class ScopedAssemblyAutoLoader : AssemblyAutoLoader
    {
    }
}
