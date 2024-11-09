# Gapotchenko.FX.Memory

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Memory.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Memory)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Memory.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Memory)

The module provides primitives for memory manipulation.

## MemoryEqualityComparer

`MemoryEqualityComparer` class provided by `Gapotchenko.FX.Memory` module allows you to compare contigious regions of memory for equality.
The regions are represented by the standard `System.ReadOnlyMemory<T>` struct.

Consider the example:

``` C#
using Gapotchenko.FX.Memory;

byte[] arr = [1, 2, 3, 4, 5, 6];

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

- [Gapotchenko.FX](../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../Math/Gapotchenko.FX.Math#readme)
- &#x27B4; [Gapotchenko.FX.Memory](.#readme)
- [Gapotchenko.FX.Security.Cryptography](../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
