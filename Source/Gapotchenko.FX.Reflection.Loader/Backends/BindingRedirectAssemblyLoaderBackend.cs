using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gapotchenko.FX.Reflection.Loader.Backends
{
    sealed class BindingRedirectAssemblyLoaderBackend : IAssemblyLoaderBackend
    {
        private BindingRedirectAssemblyLoaderBackend(
            Dictionary<string, BindingRedirect> bindingRedirects,
            AssemblyDependencyTracker assemblyDependencyTracker)
        {
            _BindingRedirects = bindingRedirects;
            _AssemblyDependencyTracker = assemblyDependencyTracker;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

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

        readonly Dictionary<string, BindingRedirect> _BindingRedirects;
        readonly AssemblyDependencyTracker _AssemblyDependencyTracker;

        public static bool TryCreate(string assemblyFilePath, AssemblyDependencyTracker assemblyDependencyTracker, out IAssemblyLoaderBackend backend)
        {
            backend = null;

            string configFilePath = assemblyFilePath + ".config";
            if (!File.Exists(configFilePath))
                return false;

            var bindingRedirects = _LoadBindingRedirects(configFilePath);
            if (bindingRedirects != null)
                backend = new BindingRedirectAssemblyLoaderBackend(bindingRedirects, assemblyDependencyTracker);

            return true;
        }

        static Dictionary<string, BindingRedirect> _LoadBindingRedirects(string configFilePath)
        {
            var xdoc = XDocument.Load(configFilePath);

            var xRuntime = xdoc.Element("configuration")?.Element("runtime");
            if (xRuntime == null)
                return null;

            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
            var xDependentAssemblies = xRuntime.Elements(ns + "assemblyBinding").SelectMany(x => x.Elements(ns + "dependentAssembly")).ToArray();

            Dictionary<string, BindingRedirect> bindingRedirects = null;

            foreach (var xDependentAssembly in xDependentAssemblies)
            {
                var xAsmID = xDependentAssembly.Element(ns + "assemblyIdentity");
                if (xAsmID == null)
                    continue;

                var xBindingRedirect = xDependentAssembly.Element(ns + "bindingRedirect");
                if (xBindingRedirect == null)
                    continue;

                string asmIDName = xAsmID.Attribute("name")?.Value;
                string asmIDPublicKeyToken = xAsmID.Attribute("publicKeyToken")?.Value;
                string asmIDCulture = xAsmID.Attribute("culture")?.Value;

                if (asmIDName == null || asmIDPublicKeyToken == null || asmIDCulture == null)
                    continue;

                string asmName = $"{asmIDName}, Culture={asmIDCulture}, PublicKeyToken={asmIDPublicKeyToken}";

                string oldVersion = xBindingRedirect.Attribute("oldVersion")?.Value;
                string newVersion = xBindingRedirect.Attribute("newVersion")?.Value;

                if (oldVersion == null || newVersion == null)
                    continue;

                var parts = oldVersion.Split(new[] { '-' }, 2, StringSplitOptions.None).Select(x => x.Trim()).ToList();
                if (parts.Count == 0)
                    continue;

                if (bindingRedirects == null)
                    bindingRedirects = new Dictionary<string, BindingRedirect>(StringComparer.OrdinalIgnoreCase);

                bindingRedirects[asmName] = new BindingRedirect(
                    Version.Parse(parts[0]),
                    Version.Parse(parts.Last()),
                    Version.Parse(newVersion));
            }

            return bindingRedirects;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_AssemblyDependencyTracker.IsAssemblyResolutionInhibited(args.RequestingAssembly))
                return null;

            var assemblyName = new AssemblyName(args.Name);
            var assemblyVersion = assemblyName.Version;
            assemblyName.Version = null;

            if (_BindingRedirects.TryGetValue(assemblyName.ToString(), out var bindingRedirect) &&
                bindingRedirect.Matches(assemblyVersion))
            {
                assemblyName.Version = bindingRedirect.To;

                bool assemblyRegistered = _AssemblyDependencyTracker.RegisterReferencedAssembly(assemblyName);

                Assembly assembly;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    if (assemblyRegistered)
                        _AssemblyDependencyTracker.UnregisterReferencedAssembly(assemblyName);
                    throw;
                }

                if (assembly == null && assemblyRegistered)
                    _AssemblyDependencyTracker.UnregisterReferencedAssembly(assemblyName);

                return assembly;
            }

            return null;
        }
    }
}
