// Portions © Microsoft and .NET Foundation

using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Reflection.Loader.Backends;

readonly struct LibraryNameVariation
{
    public LibraryNameVariation(string prefix, string suffix)
    {
        Prefix = prefix;
        Suffix = suffix;
    }

    public readonly string Prefix;
    public readonly string Suffix;

    public static IEnumerable<LibraryNameVariation> Enumerate(string libName, bool isRelativePath, bool forOSLoader = false)
    {
#if NETCOREAPP
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Enumerate_Windows(libName, isRelativePath, forOSLoader);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
#if NETCOREAPP3_0_OR_GREATER
            || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
#endif
            )
        {
            return Enumerate_Unix(libName, isRelativePath, forOSLoader);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Enumerate_OSX(libName, isRelativePath, forOSLoader);
        }
        else
        {
            return new[] { new LibraryNameVariation(string.Empty, string.Empty) };
        }
#else
        return Enumerate_Windows(libName, isRelativePath, forOSLoader);
#endif
    }

    static IEnumerable<LibraryNameVariation> Enumerate_Windows(string libName, bool isRelativePath, bool forOSLoader)
    {
        // This is a copy of logic from DetermineLibNameVariations function defined at dllimport.cpp file of CoreCLR.

        const string LibraryNameSuffix = ".dll";

        yield return new LibraryNameVariation(string.Empty, string.Empty);

        // Follow LoadLibrary rules if forOSLoader is true.
        if (isRelativePath &&
            (!forOSLoader || libName.Contains('.') && !libName.EndsWith('.')) &&
            !libName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
            !libName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            yield return new LibraryNameVariation(string.Empty, LibraryNameSuffix);
        }
    }

#if NETCOREAPP
    static IEnumerable<LibraryNameVariation> Enumerate_Unix(string libName, bool isRelativePath, bool forOSLoader, string libraryNameSuffix = ".so")
    {
        // This is a copy of logic from DetermineLibNameVariations function defined at dllimport.cpp file of CoreCLR.

        const string LibraryNamePrefix = "lib";

        if (!isRelativePath)
        {
            yield return new LibraryNameVariation(string.Empty, string.Empty);
        }
        else
        {
            bool containsSuffix = false;
            int indexOfSuffix = libName.IndexOf(libraryNameSuffix, StringComparison.OrdinalIgnoreCase);
            if (indexOfSuffix >= 0)
            {
                indexOfSuffix += libraryNameSuffix.Length;
                containsSuffix = indexOfSuffix == libName.Length || libName[indexOfSuffix] == '.';
            }

            bool containsDelim = libName.Contains(Path.DirectorySeparatorChar);

            if (containsSuffix)
            {
                yield return new(string.Empty, string.Empty);
                if (!containsDelim)
                {
                    yield return new(LibraryNamePrefix, string.Empty);
                }
                yield return new(string.Empty, libraryNameSuffix);
                if (!containsDelim)
                {
                    yield return new(LibraryNamePrefix, libraryNameSuffix);
                }
            }
            else
            {
                yield return new(string.Empty, libraryNameSuffix);
                if (!containsDelim)
                {
                    yield return new(LibraryNamePrefix, libraryNameSuffix);
                }
                yield return new(string.Empty, string.Empty);
                if (!containsDelim)
                {
                    yield return new(LibraryNamePrefix, string.Empty);
                }
            }
        }
    }

    static IEnumerable<LibraryNameVariation> Enumerate_OSX(string libName, bool isRelativePath, bool forOSLoader) =>
        Enumerate_Unix(libName, isRelativePath, forOSLoader, ".dylib");
#endif
}
