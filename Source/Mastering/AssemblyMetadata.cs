using System.Reflection;

#if !DISABLE_ASSEMBLY_METADATA_ATTRIBUTES

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Gapotchenko")]
[assembly: AssemblyProduct("Gapotchenko.FX")]
[assembly: AssemblyCopyright("Copyright © 2019-2020 Gapotchenko and Contributors")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

#if !DISABLE_ASSEMBLY_VERSION_ATTRIBUTE
[assembly: AssemblyVersion("2020.1.8.16010")]
#endif
[assembly: AssemblyFileVersion("2020.1.8.16010")]
[assembly: AssemblyInformationalVersion("2020.1.8 (Beta Release build 2020.1.8.16010)")]

#endif

[assembly: AssemblyTrademark("Gapotchenko.FX")]

namespace Gapotchenko.FX
{
#if ASSEMBLY_METADATA
    static partial class AssemblyMetadata
    {
        public const string ProductName = "Gapotchenko.FX";
        public const string CompanyName = "Gapotchenko";
        public const string InformationalVersion = "2020.1.8 (Beta Release build 2020.1.8.16010)";
        public const string DisplayVersion = "2020.1.8";
        public const string DisplayVersion2F = "2020.1";
    }
#endif
}
