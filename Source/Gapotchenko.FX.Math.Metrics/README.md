# Gapotchenko.FX.Math.Metrics

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Metrics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Metrics)

The module provides math metrics algorithms.

## StringMetrics

`StringMetrics` static class provides a variety of metric functions for measuring the distance and similarity between two strings of symbols.

The notion of a string is purely abstract.
So it is not limited to just characters like `System.String`; it can be a string of anything in form of `IEnumerable<T>`.
In this way, `Gapotchenko.FX.Math.Metrics` module achieves the right degree of abstraction desirable for a versatile math toolkit.

### Edit Distance

Edit distance is a string metric reflecting the minimum number of operations required to transform one string into the other.
There are several ways to measure the edit distance.

The table below compares edit distance measurement functions provided by `StringMetrics` class:

| Metric Function                           | Insertion | Deletion | Substitution | Transposition |
|:------------------------------------------|:---------:|:--------:|:------------:|:-------------:|
| Levenshtein distance                      | &check;   | &check;  | &check;      |               |
| Longest common subsequence (LCS) distance | &check;   | &check;  |              |               |
| Hamming distance                          |           |          | &check;      |               |
| Damerau–Levenshtein distance              | &check;   | &check;  | &check;      | &check;       |
| Optimal string alignment (OSA) distance   | &check;   | &check;  | &check;      | &check;       |
| Jaro distance                             |           |          |              | &check;       |

### Levenshtein Distance

`StringMetrics.LevenshteinDistance` method allows to calculate the Levenshtein distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

int distance = StringMetrics.LevenshteinDistance("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

### Longest Common Subsequence (LCS) Distance

`StringMetrics.LcsDistance` method allows to calculate the longest common subsequence (LCS) distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

int distance = StringMetrics.LcsDistance("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

### Hamming Distance

`StringMetrics.HammingDistance` method allows to calculate the Hamming distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

int distance = StringMetrics.HammingDistance("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

Please note that the Hamming distance can only be calculated between two string of an equal length.

### Damerau-Levenshtein Distance

`StringMetrics.DamerauLevenshteinDistance` method allows to calculate the Damerau–Levenshtein distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

int distance = StringMetrics.DamerauLevenshteinDistance("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 1
```

### Optimal String Alignment (OSA) Distance

`StringMetrics.OsaDistance` method allows to calculate the optimal string alignment (OSA) distance between two strings of symbols.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

int distance = StringMetrics.OsaDistance("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 1
```

### Jaro Distance

`StringMetrics.JaroDistance` method allows to calculate the Jaro distance between two strings of symbols.

Although the Jaro distance is often referred to as an edit distance metric, its value does not represent a number of edit operations
and varies between 0.0 and 1.0 such that 0.0 represents an exact match and 1.0 equates to no similarities.

Consider the example:

```csharp
using Gapotchenko.FX.Math.Geometry;

double distance = StringMetrics.JaroDistance("ABC", "BAC");
Console.WriteLine("Distance is {0.00}.", distance);  // distance = 0.44
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
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - &#x27B4; [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
