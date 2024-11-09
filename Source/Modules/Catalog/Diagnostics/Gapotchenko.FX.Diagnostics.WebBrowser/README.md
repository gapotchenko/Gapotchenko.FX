# Gapotchenko.FX.Diagnostics.WebBrowser

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Diagnostics.WebBrowser.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.WebBrowser)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Diagnostics.WebBrowser.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.WebBrowser)

It turns out that launching a web browser is a black voodoo art that should be grasped on every particular version of an operating system.

.NET developers used to do this trick back in the day:

``` C#
using System.Diagnostics;

Process.Start("https://example.com/");
```

Looks easy enough? It is, but there is a catch (or lots of them):

- Sometimes web browser is started while the `Process.Start(…)` method throws an exception on some machines
- Sometimes it starts not the default web browser but Internet Explorer or Edge.
Go figure.
Spotted on more than several occasions on different machines at random points of time
- It does nothing on some machines
- It fails on .NET Core

## The Solution

`Gapotchenko.FX.Diagnostics.WebBrowser` module provides `WebBrowser` class with a single `Launch` method.

A pure joy to use that comes without aforementioned drawbacks of the `Process.Start(…)` method:

``` C#
using Gapotchenko.FX.Diagnostics;

WebBrowser.Launch("https://example.com/");
```

## Usage

`Gapotchenko.FX.Diagnostics.WebBrowser` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Diagnostics.WebBrowser):

```
PM> Install-Package Gapotchenko.FX.Diagnostics.WebBrowser
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process#readme)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.WebBrowser](.#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../../..#readme).
