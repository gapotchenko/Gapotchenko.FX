# Gapotchenko.FX.Math.Geometry

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Geometry.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Geometry)

The module provides primitives and operations for geometry math.

## StringMetrics

`StringMetrics` static class provides a variety of metric functions for measuring the distance between two strings of symbols.

The notion of a string is purely abstract.
So it is not limited to just characters like `System.String`; it can be a string of anything in form of `IEnumerable<T>`.
In this way, `Gapotchenko.FX.Math.Geometry` module tries to achieve the right degree of abstraction desirable for a versatile math toolkit.

### Edit Distance

Edit distance is a string metric reflecting the minimum number of operations required to transform one string into the other.
There are several ways to measure the edit distance.

The table below compares edit distance measurement functions provided by `StringMetrics` class:

| Metric Function                           |      Insertion     |      Deletion      |    Substitution    |    Transposition   |
|:------------------------------------------|:------------------:|:------------------:|:------------------:|:------------------:|
| Levenshtein distance                      | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |                    |
| Longest common subsequence (LCS) distance | :heavy_check_mark: | :heavy_check_mark: |                    |                    |
| Hamming distance                          |                    |                    | :heavy_check_mark: |                    |
| Damerau–Levenshtein distance              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Optimal string alignment (OSA) distance   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Jaro distance                             |                    |                    |                    | :heavy_check_mark: |

### StringMetrics.LevenshteinDistance

`StringMetrics.LevenshteinDistance` method allows to calculate the Levenshtein distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.LevenshteinDistance("ABC", "BAC"));  // distance = 2
```

### StringMetrics.LcsDistance

`StringMetrics.LcsDistance` method allows to calculate the longest common subsequence (LCS) distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.LcsDistance("ABC", "BAC"));  // distance = 2
```

### StringMetrics.HammingDistance

`StringMetrics.HammingDistance` method allows to calculate the Hamming distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.HammingDistance("ABC", "BAC"));  // distance = 2
```

### StringMetrics.DamerauLevenshteinDistance

`StringMetrics.DamerauLevenshteinDistance` method allows to calculate the Damerau–Levenshtein distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.DamerauLevenshteinDistance("ABC", "BAC"));  // distance = 1
```

### StringMetrics.OsaDistance

`StringMetrics.OsaDistance` method allows to calculate the optimal string alignment (OSA) distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0}.", StringMetrics.OsaDistance("ABC", "BAC"));  // distance = 1
```

### StringMetrics.JaroDistance

`StringMetrics.JaroDistance` method allows to calculate the Jaro distance between two strings of symbols.

Although the Jaro distance is often referred to as an edit distance metric, its value does not represent a number of edit operations and varies between 0 and 1 such that 0 is an exact match and 1 equates to no similarities.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

Console.WriteLine("Distance is {0.00}.", StringMetrics.JaroDistance("ABC", "BAC"));  // distance = 0.44
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
