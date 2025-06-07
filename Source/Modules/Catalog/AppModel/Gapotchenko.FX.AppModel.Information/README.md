# Gapotchenko.FX.AppModel.Information

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.AppModel.Information.svg)](https://www.nuget.org/packages/Gapotchenko.FX.AppModel.Information)

The module provides functionality for getting information about the app.

## Information About the Current App

To get information about the current app, use `AppInformation.Current` property:

``` C#
using Gapotchenko.FX.AppModel.Information;
using System;

var info = AppInformation.Current;

Console.WriteLine("Product: {0}", info.ProductName);
Console.WriteLine("Version: {0}", info.ProductVersion);
Console.WriteLine("Company: {0}", info.CompanyName);
Console.WriteLine("Copyright: {0}", info.Copyright);
```

This can be useful for purposes like showing an about box in GUI or a copyright banner in console.

## Information About Another Part of the App

Sometimes a program consists of several parts, each of which has its own associated product information.

To get information about a specific part of the program other than the main app, use `AppInformation.For(Type)` method:

``` C#
using Gapotchenko.FX.AppModel.Information;
using System;

var info = AppInformation.For(typeof(object));

Console.WriteLine("Product: {0}", info.ProductName);
Console.WriteLine("Version: {0}", info.ProductVersion);
Console.WriteLine("Company: {0}", info.CompanyName);
Console.WriteLine("Copyright: {0}", info.Copyright);
```

Note that the example above gets information for `System.Object` type which belongs to .NET BCL (Base Class Library).
The retrieved information about that part looks like so:

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
Note that it is preferable to use `AppInformation.For(Type)` method because it is slightly more precise than `AppInformation.For(Assembly)`.

## Usage

`Gapotchenko.FX.AppModel.Information` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.AppModel.Information):

```
dotnet package add Gapotchenko.FX.AppModel.Information
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- &#x27B4; [Gapotchenko.FX.AppModel.Information](.#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
