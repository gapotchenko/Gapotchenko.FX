# Gapotchenko.FX.Math

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Math.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math)

The module provides extended math primitives.

## MathEx

`MathEx` is a class provided by `Gapotchenko.FX.Math`.
It offers extended mathematical functions,
and serves as an addendum to a conventional `System.Math` class.

### Swap

The swap operation is a widely demanded primitive:

``` C#
Swap<T>(ref T val1, ref T val2)
```

Its implementation is trivial, but was found highly desired during real-life coding sessions.
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

An immediate improvement in readability.

Note that some .NET languages have a built-in support for swap.
For example, a modern C# code swaps `val1` and `val2` by using tuples:

``` C#
(val1, val2) = (val2, val1);
```

However, if the code works with arrays, a more efficient and concise approach is to still use `MathEx.Swap`:


``` C#
// Swapping with tuples is less efficient for arrays:
(arr[val1], arr[val2]) = (arr[val2], arr[val1]);

// Swapping with MathEx.Swap is more efficient for arrays:
MathEx.Swap(ref arr[val1], ref arr[val2]);
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
var desiredProgress = new DateTime(2019, 1, 1);

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

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- &#x27B4; [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
