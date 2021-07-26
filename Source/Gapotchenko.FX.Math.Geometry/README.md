# Gapotchenko.FX.Math.Geometry

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Geometry.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Geometry)

The module provides primitives and operations for geometry math.

## StringMetrics

`StringMetrics` static class provides a variety of metric functions for measuring the distance between two strings of symbols.

The notion of a string is purely abstract.
So it is not limited by the characters like `System.String`; it can be a string of anything.
In this way, `Gapotchenko.FX.Math.Geometry` module tries to achieve the right degree of abstraction desirable for a versatile math framework.

### StringMetrics.LevenshteinDistance

`StringMetrics.LevenshteinDistance` method allows to calculate the Levenshtein distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.LevenshteinDistance("ABC", "BAC"));  // distance = 2
```

## Usage

`Gapotchenko.FX.Math.Geometry` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Geometry):

```
PM> Install-Package Gapotchenko.FX.Math.Geometry
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
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - &#x27B4; [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
