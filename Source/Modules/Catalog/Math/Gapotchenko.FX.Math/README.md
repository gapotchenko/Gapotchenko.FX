# Gapotchenko.FX.Math

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)

The module provides extended math primitives.

## MathEx

`MathEx` is a class provided by `Gapotchenko.FX.Math`.
It offers extended mathematical functions,
and serves as an addendum to a conventional `System.Math` class.
Some functions provided by `MathEx` class are described below.

### Swap

The swap operation is a widely demanded primitive:

``` C#
Swap<T>(ref T val1, ref T val2)
```

Despite a trivial implementation, swap operation was found highly desirable during real-life coding sessions.
`Swap` primitive allows to keep a mind of developer more focused on important things,
instead of writing tedious code like:

``` C#
T temp = val1;
val1 = val2;
val2 = temp;
```

For comparison, please take a look at a concise version of the same:

``` C#
MathEx.Swap(ref val1, ref val2);
```

The expression above gives an immediate improvement in readability.

Note that some .NET languages have a built-in support for swap operation.
For example, a modern C# code can swap `val1` and `val2` values using tuples, which is a highly recommended approach:

``` C#
(val1, val2) = (val2, val1);
```

### Min/Max for Three Values

The conventional `Math` class provides ubiquitous `Min`/`Max` primitives for _two_ values.

However, such a limitation on number of values was proven counter-productive on more than several occasions.
`MathEx` fixes that by providing `Min`/`Max` operations for _three_ values:

``` C#
using Gapotchenko.FX.Math;

Console.WriteLine(MathEx.Max(1, 2, 3));
```

### Min/Max for Any Comparable Type

Ever found yourself trying to find the maximum `System.DateTime` value? Or `System.Version`?

`MathEx` provides `Min`/`Max` operations for _any_ comparable type:

``` C#
using Gapotchenko.FX.Math;

var currentProgress = new DateTime(2012, 1, 1);
var desiredProgress = new DateTime(2025, 1, 1);

var fxProgress = MathEx.Max(currentProgress, desiredProgress);
Console.WriteLine(fxProgress);
```

## Usage

`Gapotchenko.FX.Math` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math):

```
PM> Install-Package Gapotchenko.FX.Math
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

Or look at the [full list of modules](../../../..#readme).
