# Gapotchenko.FX.Math.Metrics

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Metrics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Metrics)

The module provides math metrics algorithms.

## String Metrics

`Gapotchenko.FX.Math.Metrics.StringMetrics` class provides a variety of algorithms for measuring the distance and similarity between two strings of symbols.

The notion of a string is purely abstract.
So it is not limited to just characters like `System.String`; it can be a string of anything in form of `IEnumerable<T>`.
In this way, `Gapotchenko.FX.Math.Metrics` module achieves the right degree of abstraction desirable for a versatile math toolkit.

### Edit Distance

Edit distance is a string metric reflecting the minimum number of operations required to transform one string into the other.
There are several ways to measure the edit distance.

The table below compares edit distance measurement functions provided by `StringMetrics` class:

| Metric Function                           | Insertion | Deletion | Substitution | Transposition | Gapotchenko.FX Implementation                 |
|:------------------------------------------|:---------:|:--------:|:------------:|:-------------:|:---------------------------------------------:|
| Levenshtein distance                      | &check;   | &check;  | &check;      |               | `StringMetrics.Distance.Levenshtein`          |
| Longest common subsequence (LCS) distance | &check;   | &check;  |              |               | `StringMetrics.Distance.Lcs`                  |
| Hamming distance                          |           |          | &check;      |               | `StringMetrics.Distance.Hamming`              |
| Damerau–Levenshtein distance              | &check;   | &check;  | &check;      | &check;       | `StringMetrics.Distance.DamerauLevenshtein`   |
| Optimal string alignment (OSA) distance   | &check;   | &check;  | &check;      | &check;       | `StringMetrics.Distance.Osa`                  |
| Jaro similarity                           |           |          |              | &check;       | `StringMetrics.Similarity.Jaro`               |

### Levenshtein Distance

`StringMetrics.Distance.Levenshtein` algorithm allows you to calculate the Levenshtein distance between two strings of symbols.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

int distance = StringMetrics.Distance.Levenshtein.Calculate("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

### Longest Common Subsequence (LCS) Distance

`StringMetrics.Distance.Lcs` algorithm allows you to calculate the longest common subsequence (LCS) distance between two strings of symbols.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

int distance = StringMetrics.Distance.Lcs.Calculate("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

### Hamming Distance

`StringMetrics.Distance.Hamming` algorithm allows you to calculate the Hamming distance between two strings of symbols.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

int distance = StringMetrics.Distance.Hamming.Calculate("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 2
```

Please note that the Hamming distance can only be calculated between two string of an equal length.

### Damerau-Levenshtein Distance

`StringMetrics.Distance.DamerauLevenshtein` algorithm allows you to calculate the Damerau–Levenshtein distance between two strings of symbols.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

int distance = StringMetrics.Distance.DamerauLevenshtein.Calculate("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 1
```

### Optimal String Alignment (OSA) Distance

`StringMetrics.Distance.Osa` algorithm allows you to calculate the optimal string alignment (OSA) distance between two strings of symbols.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

int distance = StringMetrics.Distance.Osa.Calculate("ABC", "BAC");
Console.WriteLine("Distance is {0}.", distance);  // distance = 1
```

### Jaro Similarity

`StringMetrics.Similarity.Jaro` algorithm allows you to calculate the Jaro distance between two strings of symbols.

Although the Jaro similarity is often referred to as an edit distance metric, its value does not represent a number of edit operations
and varies between 0.0 and 1.0 such that 1.0 represents an exact match and 0.0 equates to no similarities.

Consider the example:

``` C#
using Gapotchenko.FX.Math.Metrics;

double similarity = StringMetrics.Similarity.Jaro.Calculate("ABC", "BAC");
Console.WriteLine("Similarity is {0.00}.", similarity);  // similarity = 0.36
```

## Usage

`Gapotchenko.FX.Math.Metrics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Metrics):

```
PM> Install-Package Gapotchenko.FX.Math.Metrics
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
