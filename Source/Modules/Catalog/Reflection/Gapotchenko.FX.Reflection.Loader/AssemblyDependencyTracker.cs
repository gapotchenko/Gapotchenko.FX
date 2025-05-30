﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Reflection.Loader.Util;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader;

sealed class AssemblyDependencyTracker
{
    public AssemblyDependencyTracker(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        m_TrackedAssemblyNames.Add(assembly.GetName());

        //foreach (var i in assembly.GetReferencedAssemblies())
        //    m_TrackedAssemblyNames.Add(i);
    }

    public bool IsAssemblyResolutionInhibited(Assembly? requestingAssembly)
    {
        if (requestingAssembly == null)
        {
            return false;
        }
        else
        {
            var name = requestingAssembly.GetName();
            lock (m_TrackedAssemblyNames)
                return !m_TrackedAssemblyNames.Contains(name);
        }
    }

    public bool RegisterReferencedAssembly(AssemblyName assemblyName)
    {
        lock (m_TrackedAssemblyNames)
            return m_TrackedAssemblyNames.Add(assemblyName);
    }

    public void UnregisterReferencedAssembly(AssemblyName assemblyName)
    {
        lock (m_TrackedAssemblyNames)
            m_TrackedAssemblyNames.Remove(assemblyName);
    }

    readonly HashSet<AssemblyName> m_TrackedAssemblyNames = new(AssemblyNameEqualityComparer.Instance);
}
