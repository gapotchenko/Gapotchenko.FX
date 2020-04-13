using Gapotchenko.FX.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.AppModel
{
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
        /// Extracts app information from the specified entry type.
        /// </summary>
        /// <param name="entryType">The entry type of an app.</param>
        /// <returns>The app information.</returns>
        public static IAppInformation ForType(Type entryType)
        {
            if (entryType == null)
                throw new ArgumentNullException(nameof(entryType));

            return new AppInformation
            {
                EntryType = entryType,
                EntryAssembly = entryType.Assembly
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AppInformation"/> class.
        /// </summary>
        protected AppInformation()
        {
        }

        /// <inheritdoc/>
        public string Title
        {
            get
            {
                if (m_Title == null)
                    m_Title = GetTitle() ?? string.Empty;
                return Empty.Nullify(m_Title);
            }
            protected set => m_Title = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_Title;

        /// <summary>
        /// Gets app title.
        /// </summary>
        /// <returns>The app title.</returns>
        protected virtual string GetTitle()
        {
            string title;

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
        public string Description
        {
            get
            {
                if (m_Description == null)
                    m_Description = GetDescription() ?? string.Empty;
                return Empty.Nullify(m_Description);
            }
            protected set => m_Description = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_Description;

        /// <summary>
        /// Gets app description.
        /// </summary>
        /// <returns>The app description.</returns>
        protected virtual string GetDescription()
        {
            string description;

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
        /// Gets or sets app entry type.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Type EntryType
        {
            get
            {
                if (m_EntryType == null)
                    m_EntryType = GetEntryType() ?? Empty.Type;
                return Empty.Nullify(m_EntryType);
            }
            set => m_EntryType = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Type m_EntryType;

        /// <summary>
        /// Gets app entry type.
        /// </summary>
        /// <returns>The app entry type.</returns>
        protected virtual Type GetEntryType() => GetType();

        /// <summary>
        /// Gets or sets app entry assembly.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Assembly EntryAssembly
        {
            get
            {
                if (m_EntryAssembly == null)
                    m_EntryAssembly = GetEntryAssembly();
                return m_EntryAssembly;
            }
            set => m_EntryAssembly = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Assembly m_EntryAssembly;

        /// <summary>
        /// Gets app entry assembly.
        /// </summary>
        /// <returns>The app entry assembly.</returns>
        protected virtual Assembly GetEntryAssembly() => EntryType?.Assembly ?? Assembly.GetEntryAssembly();

        FileVersionInfo EntryFileVersionInfo => LazyInitializerEx.EnsureInitialized(ref m_CachedEntryFileVersionInfo, this, GetEntryFileVersionInfo);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        FileVersionInfo m_CachedEntryFileVersionInfo;

        FileVersionInfo GetEntryFileVersionInfo()
        {
            string filePath;

            var type = EntryType;
            if (type != null)
                filePath = type.Module.FullyQualifiedName;
            else
                filePath = ExecutablePath;

            return FileVersionInfo.GetVersionInfo(filePath);
        }

        /// <inheritdoc/>
        public string ProductName
        {
            get
            {
                if (m_ProductName == null)
                    m_ProductName = GetProductName() ?? string.Empty;
                return Empty.Nullify(m_ProductName);
            }
            protected set => m_ProductName = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_ProductName;

        /// <summary>
        /// Gets product name.
        /// </summary>
        /// <returns>The product name.</returns>
        protected virtual string GetProductName()
        {
            string productName;

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
                string ns = entryType.Namespace;
                if (!string.IsNullOrEmpty(ns))
                {
                    int j = ns.LastIndexOf('.');
                    if (j != -1 && j < ns.Length - 1)
                        productName = ns.Substring(j + 1);
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
            get
            {
                if (m_ProductVersion == null)
                    m_ProductVersion = GetProductVersion();
                return m_ProductVersion;
            }
            protected set => m_ProductVersion = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Version m_ProductVersion;

        /// <summary>
        /// Gets product version.
        /// </summary>
        /// <returns>The product version.</returns>
        protected virtual Version GetProductVersion()
        {
            var entryAssembly = EntryAssembly;
            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                if (attribute != null)
                    if (Version.TryParse(attribute.Version, out var version))
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
        public string ProductInformationalVersion
        {
            get
            {
                if (m_ProductInformationalVersion == null)
                    m_ProductInformationalVersion = GetProductInformationalVersion();
                return m_ProductInformationalVersion;
            }
            protected set => m_ProductInformationalVersion = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_ProductInformationalVersion;

        /// <summary>
        /// Gets product informational version.
        /// </summary>
        /// <returns>The product informational version.</returns>
        protected virtual string GetProductInformationalVersion()
        {
            string informationalVersion;

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
        public string CompanyName
        {
            get
            {
                if (m_CompanyName == null)
                    m_CompanyName = GetCompanyName();
                return m_CompanyName;
            }
            protected set => m_CompanyName = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static string m_CompanyName;

        /// <summary>
        /// Gets company name.
        /// </summary>
        /// <returns>The company name.</returns>
        protected virtual string GetCompanyName()
        {
            string companyName;

            var entryAssembly = EntryAssembly;
            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                if (attribute != null)
                {
                    companyName = attribute.Company;
                    if (!string.IsNullOrEmpty(companyName))
                        return companyName;
                }
            }

            companyName = EntryFileVersionInfo.CompanyName;
            if (!string.IsNullOrWhiteSpace(companyName))
                return companyName.Trim();

            var entryType = EntryType;
            if (entryType != null)
            {
                string ns = entryType.Namespace;
                if (!string.IsNullOrEmpty(ns))
                {
                    int j = ns.IndexOf('.');
                    if (j != -1)
                        companyName = ns.Substring(0, j);
                    else
                        companyName = ns;
                    return companyName;
                }
            }

            return ProductName;
        }

        /// <inheritdoc/>
        public string Copyright
        {
            get
            {
                if (m_Copyright == null)
                    m_Copyright = GetCopyright() ?? string.Empty;
                return Empty.Nullify(m_Copyright);
            }
            protected set => m_Copyright = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_Copyright;

        /// <summary>
        /// Gets app copyright information.
        /// </summary>
        /// <returns>App copyright information.</returns>
        protected virtual string GetCopyright()
        {
            string copyright;

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

            return "Copyright © " + CompanyName;
        }

        /// <inheritdoc/>
        public string Trademark
        {
            get
            {
                if (m_Trademark == null)
                    m_Trademark = GetTrademark() ?? string.Empty;
                return Empty.Nullify(m_Trademark);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_Trademark;

        /// <summary>
        /// Gets app trademark information.
        /// </summary>
        /// <returns>App trademark information.</returns>
        protected virtual string GetTrademark()
        {
            string trademark;

            var entryAssembly = EntryAssembly;
            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>();
                if (attribute != null)
                {
                    trademark = attribute.Trademark;
                    if (!string.IsNullOrEmpty(trademark))
                        return trademark;
                }
            }

            trademark = EntryFileVersionInfo.LegalTrademarks;
            if (!string.IsNullOrWhiteSpace(trademark))
                return trademark.Trim();

            return null;
        }

        /// <inheritdoc/>
        public string ExecutablePath
        {
            get
            {
                if (m_ExecutablePath == null)
                    m_ExecutablePath = GetExecutablePath();
                return m_ExecutablePath;
            }
            protected set => m_ExecutablePath = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_ExecutablePath;

        /// <summary>
        /// Gets app executable path.
        /// </summary>
        /// <returns>The app executable path.</returns>
        protected virtual string GetExecutablePath()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Path.GetFullPath(NativeMethods.GetModuleFileName(default));
            }
            else
            {
                var uri = new Uri(entryAssembly.CodeBase);
                if (uri.IsFile)
                {
                    string localPath = uri.LocalPath;

#if NETSTANDARD || NETCOREAPP
                    if (localPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
#if !NETCOREAPP3_0
                        string frameworkDescription = RuntimeInformation.FrameworkDescription;
                        if (frameworkDescription.StartsWith(".NET Core ", StringComparison.OrdinalIgnoreCase) &&
                            Environment.Version.Major >= 3)
#endif
                        {
                            string exeExtension;
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                exeExtension = ".exe";
                            else
                                exeExtension = null;

                            string exePath = Path.ChangeExtension(localPath, exeExtension);
                            if (File.Exists(exePath))
                                localPath = exePath;
                        }
                    }
#endif

                    return localPath + Uri.UnescapeDataString(uri.Fragment);
                }
                else
                {
                    return uri.ToString();
                }
            }

            throw new Exception("Unable to determine app executable file path.");
        }
    }
}
