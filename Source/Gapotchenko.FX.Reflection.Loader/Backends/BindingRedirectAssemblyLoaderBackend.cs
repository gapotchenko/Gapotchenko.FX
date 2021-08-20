using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    sealed class BindingRedirectAssemblyLoaderBackend : IAssemblyLoaderBackend
    {
        private BindingRedirectAssemblyLoaderBackend(
            AssemblyAutoLoader assemblyAutoLoader,
            Dictionary<string, BindingRedirect> bindingRedirects,
            AssemblyLoadPal assemblyLoadPal,
            AssemblyDependencyTracker assemblyDependencyTracker)
        {
            m_AssemblyAutoLoader = assemblyAutoLoader;
            m_BindingRedirects = bindingRedirects;
            m_AssemblyLoadPal = assemblyLoadPal;
            m_AssemblyDependencyTracker = assemblyDependencyTracker;

            m_AssemblyLoadPal.Resolving += AssemblyResolver_Resolving;
        }

        public void Dispose()
        {
            m_AssemblyLoadPal.Resolving -= AssemblyResolver_Resolving;
        }

        readonly AssemblyAutoLoader m_AssemblyAutoLoader;

        struct BindingRedirect
        {
            public BindingRedirect(Version fromA, Version fromB, Version to)
            {
                FromA = fromA;
                FromB = fromB;
                To = to;
            }

            public Version FromA { get; }
            public Version FromB { get; }
            public Version To { get; }

#if DEBUG
            public override string ToString() => $"{FromA}-{FromB} -> {To}";
#endif

            public bool Matches(Version version) => version >= FromA && version <= FromB;
        }

        readonly AssemblyLoadPal m_AssemblyLoadPal;
        readonly Dictionary<string, BindingRedirect> m_BindingRedirects;
        readonly AssemblyDependencyTracker m_AssemblyDependencyTracker;

        public static bool TryCreate(
            string assemblyFilePath,
            AssemblyAutoLoader assemblyAutoLoader,
            AssemblyLoadPal assemblyLoadPal,
            AssemblyDependencyTracker assemblyDependencyTracker,
            out IAssemblyLoaderBackend? backend)
        {
            backend = null;

            string configFilePath = assemblyFilePath + ".config";
            if (!File.Exists(configFilePath))
                return false;

            var bindingRedirects = _LoadBindingRedirects(configFilePath);
            if (bindingRedirects != null)
                backend = new BindingRedirectAssemblyLoaderBackend(assemblyAutoLoader, bindingRedirects, assemblyLoadPal, assemblyDependencyTracker);

            return true;
        }

        static Dictionary<string, BindingRedirect>? _LoadBindingRedirects(string configFilePath)
        {
            var xdoc = XDocument.Load(configFilePath);

            var xRuntime = xdoc.Element("configuration")?.Element("runtime");
            if (xRuntime == null)
                return null;

            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
            var xDependentAssemblies = xRuntime.Elements(ns + "assemblyBinding").SelectMany(x => x.Elements(ns + "dependentAssembly")).ToArray();

            Dictionary<string, BindingRedirect>? bindingRedirects = null;

            foreach (var xDependentAssembly in xDependentAssemblies)
            {
                var xAsmID = xDependentAssembly.Element(ns + "assemblyIdentity");
                if (xAsmID == null)
                    continue;

                var xBindingRedirect = xDependentAssembly.Element(ns + "bindingRedirect");
                if (xBindingRedirect == null)
                    continue;

                string? asmIDName = xAsmID.Attribute("name")?.Value;
                string? asmIDPublicKeyToken = xAsmID.Attribute("publicKeyToken")?.Value;
                string? asmIDCulture = xAsmID.Attribute("culture")?.Value;

                if (asmIDName == null || asmIDPublicKeyToken == null || asmIDCulture == null)
                    continue;

                string asmName = $"{asmIDName}, Culture={asmIDCulture}, PublicKeyToken={asmIDPublicKeyToken}";

                string? oldVersion = xBindingRedirect.Attribute("oldVersion")?.Value;
                string? newVersion = xBindingRedirect.Attribute("newVersion")?.Value;

                if (oldVersion == null || newVersion == null)
                    continue;

                var parts = oldVersion.Split(new[] { '-' }, 2, StringSplitOptions.None).Select(x => x.Trim()).ToList();
                if (parts.Count == 0)
                    continue;

                bindingRedirects ??= new(StringComparer.OrdinalIgnoreCase);

                bindingRedirects[asmName] = new BindingRedirect(
                    Version.Parse(parts[0]),
                    Version.Parse(parts.Last()),
                    Version.Parse(newVersion));
            }

            return bindingRedirects;
        }

        AssemblyName? TryRedirectAssembly(AssemblyName assemblyName)
        {
            var assemblyVersion = assemblyName.Version;
            if (assemblyVersion == null)
                return null;

            assemblyName = (AssemblyName)assemblyName.Clone();

            assemblyName.Version = null;

            if (m_BindingRedirects.TryGetValue(assemblyName.ToString(), out var bindingRedirect) &&
                bindingRedirect.Matches(assemblyVersion) &&
                assemblyVersion != bindingRedirect.To)
            {
                assemblyName.Version = bindingRedirect.To;
                return assemblyName;
            }

            return null;
        }

        Assembly? AssemblyResolver_Resolving(AssemblyLoadPal sender, AssemblyLoadPal.ResolvingEventArgs args)
        {
            if (m_AssemblyDependencyTracker.IsAssemblyResolutionInhibited(args.RequestingAssembly))
                return null;

            var assemblyName = TryRedirectAssembly(args.Name);
            if (assemblyName == null)
                return null;

            bool assemblyRegistered = m_AssemblyDependencyTracker.RegisterReferencedAssembly(assemblyName);

            Assembly? assembly = null;
            try
            {
                assembly = m_AssemblyLoadPal.Load(assemblyName);
            }
            finally
            {
                if (assembly == null && assemblyRegistered)
                    m_AssemblyDependencyTracker.UnregisterReferencedAssembly(assemblyName);
            }

            return assembly;
        }

        public string? ResolveAssemblyPath(AssemblyName assemblyName)
        {
            var redirectedAssemblyName = TryRedirectAssembly(assemblyName);
            if (redirectedAssemblyName == null)
                return null;

            return m_AssemblyAutoLoader.ResolveAssemblyPath(redirectedAssemblyName);
        }

        public string? ResolveUnmanagedDllPath(string unmanagedDllName) => null;
    }
}
