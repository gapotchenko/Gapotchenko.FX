# Gapotchenko.FX.AppModel.Information

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.AppModel.Information.svg)](https://www.nuget.org/packages/Gapotchenko.FX.AppModel.Information)

The module provides functionality for getting the information about the app.

## Information About the Current App

To get the information about the current app, use `AppInformation.Current` property:

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
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
