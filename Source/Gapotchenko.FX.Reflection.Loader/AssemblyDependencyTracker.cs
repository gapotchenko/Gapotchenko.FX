using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader
{
    sealed class AssemblyDependencyTracker
    {
        public AssemblyDependencyTracker(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _TrackedAssemblyNames.Add(assembly.GetName());

            //foreach (var i in assembly.GetReferencedAssemblies())
            //    _TrackedAssemblyNames.Add(i);
        }

        readonly HashSet<AssemblyName> _TrackedAssemblyNames = new HashSet<AssemblyName>();

        public bool IsAssemblyResolutionInhibited(Assembly requestingAssembly)
        {
            if (requestingAssembly != null)
            {
                var name = requestingAssembly.GetName();
                lock (_TrackedAssemblyNames)
                    if (!_TrackedAssemblyNames.Contains(name))
                        return true;
            }
            return false;
        }

        public bool RegisterReferencedAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            lock (_TrackedAssemblyNames)
                return _TrackedAssemblyNames.Add(assemblyName);
        }

        public void UnregisterReferencedAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            lock (_TrackedAssemblyNames)
                _TrackedAssemblyNames.Remove(assemblyName);
        }
    }
}
