# Gapotchenko.FX.Versioning

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Versioning.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Versioning)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Versioning.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Versioning)

The module provides functionality for working with versions.

## VersionConverter

`VersionConverter` class provides a type converter for `System.Version` that enables conversion to and from string representations and integration with various .NET frameworks and UI components.

#### Using `VersionConverter`

The `VersionConverter` can be used with `TypeDescriptor` to convert between `Version` and string:

``` C#
using Gapotchenko.FX.Versioning;
using System.ComponentModel;

var converter = TypeDescriptor.GetConverter(typeof(Version));
var version = (Version)converter.ConvertFrom("1.2.3.4");
Console.WriteLine(version); // "1.2.3.4"
```

#### Registering `VersionConverter`

To ensure that `VersionConverter` is used as the default converter for `Version` type, you can register it:

``` C#
using Gapotchenko.FX.Versioning;

VersionConverter.Register();
```

After registration, the converter will be automatically used by frameworks that rely on `TypeDescriptor` for type conversion, such as Windows Forms, WPF, and ASP.NET Core.
`Register` method can be safely called multiple times; the actual registration occurs only once.

## Version Deconstruction

The module provides extension methods that enable deconstruction of `Version` objects, allowing you to easily extract version components using pattern matching syntax.

### Deconstructing to Major and Minor

``` C#
using Gapotchenko.FX.Versioning;
using System;

var version = new Version(1, 2, 3, 4);
var (major, minor) = version;

Console.WriteLine($"Major: {major}, Minor: {minor}"); // "Major: 1, Minor: 2"
```

### Deconstructing to Major, Minor, and Build

``` C#
using Gapotchenko.FX.Versioning;
using System;

var version = new Version(1, 2, 3, 4);
var (major, minor, build) = version;

Console.WriteLine($"Major: {major}, Minor: {minor}, Build: {build}"); // "Major: 1, Minor: 2, Build: 3"
```

### Deconstructing to All Components

``` C#
using Gapotchenko.FX.Versioning;
using System;

var version = new Version(1, 2, 3, 4);
var (major, minor, build, revision) = version;

Console.WriteLine($"Version: {major}.{minor}.{build}.{revision}"); // "Version: 1.2.3.4"
```

Deconstruction is particularly useful when working with pattern matching:

``` C#
using Gapotchenko.FX.Versioning;
using System;

Version GetVersion() => new Version(2, 0, 0, 0);

var (major, minor, _, _) = GetVersion();

if (major >= 2)
{
    Console.WriteLine("Version 2.0 or later");
}
```

## Usage

`Gapotchenko.FX.Versioning` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Versioning):

```
dotnet package add Gapotchenko.FX.Versioning
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
- &#x27B4; [Gapotchenko.FX.Versioning](.#readme)
  - [Gapotchenko.FX.Versioning.Semantic](../Gapotchenko.FX.Versioning.Semantic#readme)

Or look at the [full list of modules](../../..#readme).
