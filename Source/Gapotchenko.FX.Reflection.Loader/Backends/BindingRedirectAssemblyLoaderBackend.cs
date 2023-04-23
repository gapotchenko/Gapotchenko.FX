using System.Reflection;
using System.Xml.Linq;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

sealed class BindingRedirectAssemblyLoaderBackend : IAssemblyLoaderBackend
{
    BindingRedirectAssemblyLoaderBackend(
        bool isAttached,
        AssemblyAutoLoader assemblyAutoLoader,
        Dictionary<string, BindingRedirect> bindingRedirects,
        AssemblyLoadPal assemblyLoadPal,
        AssemblyDependencyTracker assemblyDependencyTracker)
    {
        m_AssemblyAutoLoader = assemblyAutoLoader;
        m_BindingRedirects = bindingRedirects;
        m_AssemblyLoadPal = assemblyLoadPal;
        m_AssemblyDependencyTracker = assemblyDependencyTracker;

        if (isAttached)
            m_AssemblyLoadPal.Resolving += AssemblyResolver_Resolving;
    }

    public void Dispose()
    {
        m_AssemblyLoadPal.Resolving -= AssemblyResolver_Resolving;
    }

    readonly AssemblyAutoLoader m_AssemblyAutoLoader;

    readonly struct BindingRedirect
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
        out IAssemblyLoaderBackend? backend,
        out IEnumerable<string>? probingPaths)
    {
        backend = null;

        string configFilePath = assemblyFilePath + ".config";
        if (!File.Exists(configFilePath))
        {
            probingPaths = null;
            return false;
        }

        var bindingRedirects = LoadBindingRedirects(configFilePath, out probingPaths);
        if (bindingRedirects != null)
        {
            backend = new BindingRedirectAssemblyLoaderBackend(
                assemblyAutoLoader.IsAttached,
                assemblyAutoLoader,
                bindingRedirects,
                assemblyLoadPal,
                assemblyDependencyTracker);
        }

        return true;
    }

    static Dictionary<string, BindingRedirect>? LoadBindingRedirects(
        string configFilePath,
        out IEnumerable<string>? probingPaths)
    {
        var xDoc = XDocument.Load(configFilePath);

        var xRuntime = xDoc.Element("configuration")?.Element("runtime");
        if (xRuntime == null)
        {
            probingPaths = null;
            return null;
        }

        List<string>? probingPathList = null;
        Dictionary<string, BindingRedirect>? bindingRedirects = null;

        XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
        foreach (var xAssemblyBinding in xRuntime.Elements(ns + "assemblyBinding"))
        {
            foreach (var xProbing in xAssemblyBinding.Elements(ns + "probing"))
            {
                string? privatePath = xProbing.Attribute("privatePath")?.Value;
                if (!string.IsNullOrEmpty(privatePath))
                {
                    var paths = privatePath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (paths.Length != 0)
                        (probingPathList ??= new()).AddRange(paths);
                }
            }

            foreach (var xDependentAssembly in xAssemblyBinding.Elements(ns + "dependentAssembly"))
            {
                var xAsmId = xDependentAssembly.Element(ns + "assemblyIdentity");
                if (xAsmId == null)
                    continue;

                var xBindingRedirect = xDependentAssembly.Element(ns + "bindingRedirect");
                if (xBindingRedirect == null)
                    continue;

                string? asmIdName = xAsmId.Attribute("name")?.Value;
                string? asmIdPublicKeyToken = xAsmId.Attribute("publicKeyToken")?.Value;
                string? asmIdCulture = xAsmId.Attribute("culture")?.Value;

                if (asmIdName == null || asmIdPublicKeyToken == null || asmIdCulture == null)
                    continue;

                string asmName = $"{asmIdName}, Culture={asmIdCulture}, PublicKeyToken={asmIdPublicKeyToken}";

                string? oldVersion = xBindingRedirect.Attribute("oldVersion")?.Value;
                string? newVersion = xBindingRedirect.Attribute("newVersion")?.Value;

                if (oldVersion == null || newVersion == null)
                    continue;

                IReadOnlyList<string> parts = oldVersion
#if NETCOREAPP3_1_OR_GREATER
                    .Split('-', 2, StringSplitOptions.TrimEntries);
#else
                    .Split(new[] { '-' }, 2, StringSplitOptions.None)
                    .Select(x => x.Trim())
                    .ToList();
#endif

                int partCount = parts.Count;
                if (partCount == 0)
                    continue;

                bindingRedirects ??= new(StringComparer.OrdinalIgnoreCase);

                bindingRedirects[asmName] = new BindingRedirect(
                    Version.Parse(parts[0]),
                    Version.Parse(parts[partCount - 1]),
                    Version.Parse(newVersion));
            }
        }

        probingPaths = probingPathList;
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
