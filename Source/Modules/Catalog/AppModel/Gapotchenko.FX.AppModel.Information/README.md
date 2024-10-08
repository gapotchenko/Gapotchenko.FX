# Gapotchenko.FX.AppModel.Information

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.AppModel.Information.svg)](https://www.nuget.org/packages/Gapotchenko.FX.AppModel.Information)

The module provides functionality for getting information about the app.

## Getting Information About the Current App

To get information about the current app, use `AppInformation.Current` property:

```csharp
using Gapotchenko.FX.AppModel.Information;
using System;

var info = AppInformation.Current;

Console.WriteLine("Product: {0}", info.ProductName);
Console.WriteLine("Version: {0}", info.ProductVersion);
Console.WriteLine("Company: {0}", info.CompanyName);
Console.WriteLine("Copyright: {0}", info.Copyright);
```

This can be useful for purposes like showing an about box in GUI or a copyright banner in console.

## Getting Information About Another Part of the App

Sometimes a program consists of several parts, each of which has its own associated product information.

To get information about a specific part of the program other than the main app, use `AppInformation.For(Type)` method:

```csharp
using Gapotchenko.FX.AppModel.Information;
using System;

var info = AppInformation.For(typeof(object));

Console.WriteLine("Product: {0}", info.ProductName);
Console.WriteLine("Version: {0}", info.ProductVersion);
Console.WriteLine("Company: {0}", info.CompanyName);
Console.WriteLine("Copyright: {0}", info.Copyright);
```

Note that the example above gets information for `System.Object` type which belongs to .NET BCL (**B**ase **C**lass **L**ibrary).
The following information is retrieved:

```
Product: Microsoft® .NET
Version: 8.0.824.36612
Company: Microsoft Corporation
Copyright: © Microsoft Corporation. All rights reserved.
```

You can use this functionality to retrieve information about any other part of the program.
It can be a plugin, a library, and so on.

There also exists `AppInformation.For(Assembly)` overload of the `For` method.
That method overload retrieves information about a specific assembly.
It is useful for situations when you have no specific `System.Type` at hand to retrieve the information for, but only a `System.Assembly`.

It is prefereable to use `AppInformation.For(Type)` method because it is slightly more precise than `AppInformation.For(Assembly)`.

## Usage

`Gapotchenko.FX.AppModel.Information` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.AppModel.Information):

```
PM> Install-Package Gapotchenko.FX.AppModel.Information
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- &#x27B4; [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
