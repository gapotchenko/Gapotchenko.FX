# Gapotchenko.FX.Memory

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Memory.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Memory)

The module provides primitives for memory manipulation.

## MemoryEqualityComparer

`MemoryEqualityComparer` class provided by `Gapotchenko.FX.Memory` module allows you to compare contigious regions of memory for equality.
The regions are represented by the standard `System.ReadOnlyMemory<T>` struct.

Consider the example:

```csharp
using Gapotchenko.FX.Memory;

byte[] arr = new byte[] { 1, 2, 3, 4, 5, 6 };

var map = new Dictionary<ReadOnlyMemory<byte>, string>(MemoryEqualityComparer<byte>.Default);

map[arr.AsMemory().Slice(0, 3)] = "A";
map[arr.AsMemory().Slice(3, 3)] = "B";

Console.WriteLine(map[new byte[] { 1, 2, 3 }]);  // prints A
Console.WriteLine(map[new byte[] { 4, 5, 6 }]);  // prints B
```

## Usage

`Gapotchenko.FX.Memory` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Memory):

```
PM> Install-Package Gapotchenko.FX.Memory
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- &#x27B4; [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
