// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.AppModel;

/// <summary>
/// Provides information about the app.
/// </summary>
public class AppInformation : IAppInformation
{
    /// <summary>
    /// Gets information about the current running app.
    /// </summary>
    public static IAppInformation Current => CurrentAppInformation.Instance;

    /// <summary>
    /// Extracts app information associated with the specified type.
    /// </summary>
    /// <param name="type">The type to extract the app information from.</param>
    /// <returns>The app information.</returns>
    public static IAppInformation For(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return new AppInformation
        {
            EntryType = type,
            EntryAssembly = type.Assembly
        };
    }

    /// <summary>
    /// Extracts app information associated with the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to extract the app information from.</param>
    /// <returns>The app information.</returns>
    public static IAppInformation For(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var entryType = assembly.EntryPoint?.ReflectedType;

        return new AppInformation
        {
            EntryType = entryType ?? Empty.Type,
            EntryAssembly = assembly
        };
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AppInformation"/> class.
    /// </summary>
    protected AppInformation()
    {
    }

    /// <inheritdoc/>
    public string? Title
    {
        get => Empty.Nullify(m_Title ??= RetrieveTitle() ?? string.Empty);
        protected set => m_Title = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_Title;

    /// <summary>
    /// Retrieves app title.
    /// </summary>
    /// <returns>The app title.</returns>
    protected virtual string? RetrieveTitle()
    {
        string? title;

        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
            if (attribute != null)
            {
                title = attribute.Title;
                if (!string.IsNullOrEmpty(title))
                    return title;
            }
        }

        title = EntryFileVersionInfo.FileDescription;
        if (!string.IsNullOrWhiteSpace(title))
            return title.Trim();

        return null;
    }

    /// <inheritdoc/>
    public string? Description
    {
        get => Empty.Nullify(m_Description ??= RetrieveDescription() ?? string.Empty);
        protected set => m_Description = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_Description;

    /// <summary>
    /// Retrieves app description.
    /// </summary>
    /// <returns>The app description.</returns>
    protected virtual string? RetrieveDescription()
    {
        string? description;

        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (attribute != null)
            {
                description = attribute.Description;
                if (!string.IsNullOrEmpty(description))
                    return description;
            }
        }

        description = EntryFileVersionInfo.Comments;
        if (!string.IsNullOrWhiteSpace(description))
            return description.Trim();

        return null;
    }

    /// <summary>
    /// Gets or configures app entry type.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Type? EntryType
    {
        get => Empty.Nullify(m_EntryType ??= RetrieveEntryType() ?? Empty.Type);
        init => m_EntryType = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile Type? m_EntryType;

    /// <summary>
    /// Retrieves app entry type.
    /// </summary>
    /// <returns>The app entry type.</returns>
    protected virtual Type? RetrieveEntryType() => GetType();

    /// <summary>
    /// Gets or configures app entry assembly.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Assembly? EntryAssembly
    {
        get => m_EntryAssembly ??= RetrieveEntryAssembly();
        init => m_EntryAssembly = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile Assembly? m_EntryAssembly;

    /// <summary>
    /// Retrieves app entry assembly.
    /// </summary>
    /// <returns>The app entry assembly.</returns>
    protected virtual Assembly? RetrieveEntryAssembly() => EntryType?.Assembly ?? Assembly.GetEntryAssembly();

    FileVersionInfo EntryFileVersionInfo =>
        LazyInitializerEx.EnsureInitialized(
            ref m_CachedEntryFileVersionInfo,
            this,
            RetrieveEntryFileVersionInfo);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    FileVersionInfo? m_CachedEntryFileVersionInfo;

    FileVersionInfo RetrieveEntryFileVersionInfo()
    {
        var type = EntryType;
        string filePath = type != null ? type.Module.FullyQualifiedName : ExecutablePath;

        return FileVersionInfo.GetVersionInfo(filePath);
    }

    /// <inheritdoc/>
    public string? ProductName
    {
        get => Empty.Nullify(m_ProductName ??= RetrieveProductName() ?? string.Empty);
        protected set => m_ProductName = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_ProductName;

    /// <summary>
    /// Retrieves product name.
    /// </summary>
    /// <returns>The product name.</returns>
    protected virtual string? RetrieveProductName()
    {
        string? productName;

        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyProductAttribute>();
            if (attribute != null)
            {
                productName = attribute.Product;
                if (!string.IsNullOrEmpty(productName))
                    return productName;
            }
        }

        productName = EntryFileVersionInfo.ProductName;
        if (!string.IsNullOrWhiteSpace(productName))
            return productName.Trim();

        var entryType = EntryType;
        if (entryType != null)
        {
            string? ns = entryType.Namespace;
            if (!string.IsNullOrEmpty(ns))
            {
                int j = ns.LastIndexOf('.');
                if (j != -1 && j < ns.Length - 1)
                    productName = ns[(j + 1)..];
                else
                    productName = ns;
            }
            else
            {
                productName = entryType.Name;
            }

            return productName;
        }

        return null;
    }

    /// <inheritdoc/>
    public Version ProductVersion
    {
        get => m_ProductVersion ??= RetrieveProductVersion();
        protected set => m_ProductVersion = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile Version? m_ProductVersion;

    /// <summary>
    /// Retrieves product version.
    /// </summary>
    /// <returns>The product version.</returns>
    protected virtual Version RetrieveProductVersion()
    {
        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attribute != null && Version.TryParse(attribute.Version, out var version))
                return version;
        }

        var entryFileVersionInfo = EntryFileVersionInfo;
        return new Version(
            entryFileVersionInfo.ProductMajorPart,
            entryFileVersionInfo.ProductMinorPart,
            entryFileVersionInfo.ProductBuildPart,
            entryFileVersionInfo.ProductPrivatePart);
    }

    /// <inheritdoc/>
    public string InformationalVersion
    {
        get => m_InformationalVersion ??= RetrieveInformationalVersion();
        protected set => m_InformationalVersion = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_InformationalVersion;

    /// <summary>
    /// Retrieves product informational version.
    /// </summary>
    /// <returns>The product informational version.</returns>
    protected virtual string RetrieveInformationalVersion()
    {
        string? informationalVersion;

        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
            {
                informationalVersion = attribute.InformationalVersion;
                if (!string.IsNullOrEmpty(informationalVersion))
                    return informationalVersion;
            }
        }

        informationalVersion = EntryFileVersionInfo.ProductVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
            return informationalVersion.Trim();

        return ProductVersion.ToString();
    }

    /// <inheritdoc/>
    public string? CompanyName
    {
        get => Empty.Nullify(m_CompanyName ??= RetrieveCompanyName() ?? string.Empty);
        protected set => m_CompanyName = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_CompanyName;

    /// <summary>
    /// Retrieves company name.
    /// </summary>
    /// <returns>The company name.</returns>
    protected virtual string? RetrieveCompanyName()
    {
        return
            Empty.Nullify(EntryAssembly
                ?.GetCustomAttribute<AssemblyCompanyAttribute>()
                ?.Company) ??
            Empty.NullifyWhiteSpace(EntryFileVersionInfo.CompanyName)?.Trim() ??
            TryGetCompanyName(EntryType);

        static string? TryGetCompanyName(Type? type)
        {
            if (type == null)
                return null;

            string? ns = type.Namespace;
            if (string.IsNullOrEmpty(ns))
                return null;

            int j = ns.IndexOf('.');
            if (j != -1)
                return ns[..j];
            else
                return ns;
        }
    }

    /// <inheritdoc/>
    public string? Copyright
    {
        get => Empty.Nullify(m_Copyright ??= RetrieveCopyright() ?? string.Empty);
        protected set => m_Copyright = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_Copyright;

    /// <summary>
    /// Retrieves app copyright information.
    /// </summary>
    /// <returns>App copyright information.</returns>
    protected virtual string? RetrieveCopyright()
    {
        string? copyright;

        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            var attribute = entryAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (attribute != null)
            {
                copyright = attribute.Copyright;
                if (!string.IsNullOrEmpty(copyright))
                    return copyright;
            }
        }

        copyright = EntryFileVersionInfo.LegalCopyright;
        if (!string.IsNullOrWhiteSpace(copyright))
            return copyright.Trim();

        string? companyName = CompanyName;
        if (companyName != null)
            return "Copyright © " + companyName;

        return null;
    }

    /// <inheritdoc/>
    public string? Trademark
    {
        get => Empty.Nullify(m_Trademark ??= RetrieveTrademark() ?? string.Empty);
        protected set => m_Trademark = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_Trademark;

    /// <summary>
    /// Retrieves app trademark information.
    /// </summary>
    /// <returns>App trademark information.</returns>
    protected virtual string? RetrieveTrademark() =>
        Empty.Nullify(EntryAssembly
            ?.GetCustomAttribute<AssemblyTrademarkAttribute>()
            ?.Trademark) ??
        Empty.NullifyWhiteSpace(EntryFileVersionInfo.LegalTrademarks)?.Trim();

    /// <inheritdoc/>
    public string ExecutablePath
    {
        get => m_ExecutablePath ??= RetrieveExecutablePath();
        protected set => m_ExecutablePath = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    volatile string? m_ExecutablePath;

    /// <summary>
    /// Retrieves app executable path.
    /// </summary>
    /// <returns>The app executable path.</returns>
    protected virtual string RetrieveExecutablePath()
    {
        var entryAssembly = EntryAssembly;
        if (entryAssembly != null)
        {
            if (entryAssembly.EntryPoint == null)
                entryAssembly = null;
        }

        bool useProcess = false;
        if (entryAssembly == null)
        {
            entryAssembly = Assembly.GetEntryAssembly();
            useProcess = true;
        }

        if (entryAssembly != null)
        {
#if NET5_0_OR_GREATER
            string location = entryAssembly.Location;
            if (location.Length != 0)
                return GetLocalExecutablePath(location);
#else
            string? codeBase = entryAssembly.CodeBase;
            if (codeBase != null)
            {
                var uri = new Uri(codeBase);
                if (uri.IsFile)
                    return GetLocalExecutablePath(uri.LocalPath) + Uri.UnescapeDataString(uri.Fragment);
                else
                    return uri.ToString();
            }
#endif
        }

        if (useProcess || entryAssembly == Assembly.GetEntryAssembly())
        {
            if (GetProcessPath() is not null and var processPath)
                return processPath;
        }

        throw new AppModelException("Unable to determine executable file path of the app.");
    }

    static string GetLocalExecutablePath(string localPath)
    {
#if NETSTANDARD || NETCOREAPP || NET
        if (localPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
#if !NETCOREAPP3_0_OR_GREATER
            string frameworkDescription = RuntimeInformation.FrameworkDescription;
            if (frameworkDescription.StartsWith(".NET Core ", StringComparison.OrdinalIgnoreCase) && Environment.Version.Major >= 3 ||
                frameworkDescription.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) && Environment.Version.Major >= 5)
#endif
            {
                string? exeExtension = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : null;
                string exePath = Path.ChangeExtension(localPath, exeExtension);

                if (File.Exists(exePath))
                    localPath = exePath;
            }
        }
#endif

        return localPath;
    }

    static string? GetProcessPath()
    {
#if NET6_0_OR_GREATER
        return Environment.ProcessPath;
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Path.GetFullPath(NativeMethods.GetModuleFileName(default));
        else
            return Process.GetCurrentProcess().MainModule?.FileName;
#endif
    }
}
