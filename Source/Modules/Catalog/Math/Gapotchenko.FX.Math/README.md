# Gapotchenko.FX.Math

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)

The module provides extended math primitives.

## Extended `System.Math` Functionality

`System.Math` is a static class in the .NET Base Class Library (BCL) that provides commonly used mathematical operations. While comprehensive, its API leaves some practical gaps.

The `Gapotchenko.FX.Math` module complements `System.Math` by extending its functionality to address those limitations.

### Min/Max for Three Values

The conventional `System.Math` class provides ubiquitous `Min`/`Max` primitives for _two_ values.

However, such a limitation on number of values was proven counter-productive on more than several occasions.
`Gapotchenko.FX.Math` fixes that by providing `Min`/`Max` operations for _three_ values:

``` C#
using Gapotchenko.FX.Math;
using System;

Console.WriteLine(Math.Max(1, 2, 3));
```

### Min/Max for Any Comparable Type

Ever found yourself trying to find the maximum `System.DateTime` value? Or `System.Version`?

`Gapotchenko.FX.Math` module provides `Min`/`Max` operations for _any_ comparable type:

``` C#
using Gapotchenko.FX.Math;
using System;

var currentProgress = new DateTime(2012, 1, 1);
var desiredProgress = new DateTime(2026, 1, 1);

var fxProgress = Math.Max(currentProgress, desiredProgress);
Console.WriteLine(fxProgress);
```

## `Gapotchenko.FX.Math.MathEx`

`MathEx` is a class provided by `Gapotchenko.FX.Math` module that offers extended mathematical functions.
It is designed as a natural addendum to the standard `System.Math` class.

Some of the functionality provided by `MathEx` is outlined below.

### Factorial

To get a factorial of a number, you can utilize the `MathEx.Factorial` function:

``` C#
using Gapotchenko.FX.Math;

Console.WriteLine(MathEx.Factorial(5));
```


## Usage

`Gapotchenko.FX.Math` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math):

```
dotnet package add Gapotchenko.FX.Math
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- &#x27B4; [Gapotchenko.FX.Math](.#readme)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics#readme)
  - [Gapotchenko.FX.Math.Graphs](../Gapotchenko.FX.Math.Graphs#readme)
  - [Gapotchenko.FX.Math.Intervals](../Gapotchenko.FX.Math.Intervals#readme)
  - [Gapotchenko.FX.Math.Metrics](../Gapotchenko.FX.Math.Metrics#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
