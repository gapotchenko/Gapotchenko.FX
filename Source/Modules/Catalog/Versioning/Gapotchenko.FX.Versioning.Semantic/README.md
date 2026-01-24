# Gapotchenko.FX.Versioning.Semantic

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Versioning.Semantic.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Versioning.Semantic)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Versioning.Semantic.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Versioning.Semantic)

The module provides semantic versioning functionality that follows the [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html) specification.

## SemanticVersion

`SemanticVersion` record from `Gapotchenko.FX.Versioning.Semantic` module represents a semantic version with support for major, minor, patch, prerelease, and build metadata components.

### Creating a Semantic Version

You can create a `SemanticVersion` in several ways:

``` C#
using Gapotchenko.FX.Versioning;

// Using constructor with major, minor, and patch
var version1 = new SemanticVersion(1, 2, 3);

// With prerelease and build metadata
var version2 = new SemanticVersion(1, 2, 3, "alpha.1", "20240101");

// From a version string
var version3 = new SemanticVersion("1.2.3-alpha.1+20240101");

// Using Parse method
var version4 = SemanticVersion.Parse("2.0.0-beta.2");
```

### Parsing

The `Parse` method converts a string representation of a semantic version to a `SemanticVersion` object:

``` C#
using Gapotchenko.FX.Versioning;

var version = SemanticVersion.Parse("1.2.3-alpha.1+20240101");
Console.WriteLine($"Major: {version.Major}, Minor: {version.Minor}, Patch: {version.Patch}");
Console.WriteLine($"Prerelease: {version.Prerelease}, Build: {version.Build}");
```

For scenarios where the input might be invalid, use `TryParse`:

``` C#
if (SemanticVersion.TryParse("1.2.3", out var version))
    Console.WriteLine($"Parsed version: {version}");
else
    Console.WriteLine("Invalid version string");
```

### Version Components

A `SemanticVersion` exposes the following properties:

- `Major`: The major version number
- `Minor`: The minor version number
- `Patch`: The patch version number
- `Prerelease`: The prerelease identifier (e.g., "alpha.1", "beta.2", "rc.1")
- `Build`: The build metadata (e.g., "20240101", "abc123")

``` C#
using Gapotchenko.FX.Versioning;

var version = new SemanticVersion(1, 2, 3, "alpha.1", "20240101");

Console.WriteLine(version.Major);      // 1
Console.WriteLine(version.Minor);      // 2
Console.WriteLine(version.Patch);      // 3
Console.WriteLine(version.Prerelease); // "alpha.1"
Console.WriteLine(version.Build);      // "20240101"
```

### Comparison

`SemanticVersion` implements `IComparable<SemanticVersion>` and supports standard comparison operators:

``` C#
using Gapotchenko.FX.Versioning;

var v1 = new SemanticVersion(1, 2, 3);
var v2 = new SemanticVersion(1, 2, 4);
var v3 = new SemanticVersion(1, 2, 3, "alpha.1");

Console.WriteLine(v1 < v2);   // True
Console.WriteLine(v1 > v3);   // True (release version is greater than prerelease)
Console.WriteLine(v1 == v2);  // False
```

Prerelease versions are considered less than release versions:

``` C#
var release = new SemanticVersion(1, 0, 0);
var prerelease = new SemanticVersion(1, 0, 0, "alpha.1");

Console.WriteLine(release > prerelease); // True
```

### Conversion to `System.Version`

`SemanticVersion` can be implicitly converted to `System.Version`:

``` C#
using Gapotchenko.FX.Versioning;
using System;

var semanticVersion = new SemanticVersion(1, 2, 3, "alpha.1", "20240101");
Version version = semanticVersion; // Implicit conversion

Console.WriteLine(version); // "1.2.3"
```

Note that the conversion is lossy because `System.Version` does not support prerelease or build metadata.

### Conversion from `System.Version`

You can create a `SemanticVersion` from a `System.Version`:

``` C#
using Gapotchenko.FX.Versioning;
using System;

var version = new Version(1, 2, 3);
var semanticVersion = new SemanticVersion(version);

Console.WriteLine(semanticVersion); // "1.2.3"
```

### Deconstruction

`SemanticVersion` supports deconstruction for easy extraction of version components:

``` C#
using Gapotchenko.FX.Versioning;

var version = new SemanticVersion(1, 2, 3);

// Deconstruct to major and minor
var (major, minor) = version;

// Deconstruct to major, minor, and patch
var (major2, minor2, patch) = version;
```

### String Formatting

`SemanticVersion` provides a `ToString()` method that returns the standard semantic version string format:

``` C#
using Gapotchenko.FX.Versioning;

var version = new SemanticVersion(1, 2, 3, "alpha.1", "20240101");
Console.WriteLine(version.ToString()); // "1.2.3-alpha.1+20240101"
```

### Type Converter

`SemanticVersion` includes a `TypeConverter` that enables integration with various .NET components:

``` C#
using Gapotchenko.FX.Versioning;
using System.ComponentModel;

var converter = TypeDescriptor.GetConverter(typeof(SemanticVersion));
var version = (SemanticVersion)converter.ConvertFrom("1.2.3");
```

## Usage

`Gapotchenko.FX.Versioning.Semantic` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Versioning.Semantic):

```
dotnet package add Gapotchenko.FX.Versioning.Semantic
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../Gapotchenko.FX.Versioning#readme)
  - &#x27B4; [Gapotchenko.FX.Versioning.Semantic](.#readme)

Or look at the [full list of modules](../../..#readme).
