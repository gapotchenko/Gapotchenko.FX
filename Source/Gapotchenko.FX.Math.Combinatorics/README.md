# Gapotchenko.FX.Math.Combinatorics

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Combinatorics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Combinatorics)

The module provides the math operations for combinatorics.

## Permutations

Permutation is a widely used operation that returns all possible permutations of elements in a sequence.

Let's take a look at the sample code:

```csharp
using Gapotchenko.FX.Math.Combinatorics;

var seq = new[] { "A", "B", "C" };

foreach (var i in Permutations.Of(seq))
    Console.WriteLine(string.Join(" ", i));
```

The code produces the following output:

```
A B C
A C B
B A C
B C A
C A B
C B A
```

### Unique Permutations

By default, the generated permutations are positional,
e.g. if the input sequence contains some duplicates, there will be duplicate permutations in the output.

Let's see an example of non-unique permutations:

``` csharp
var seq = new[] { 1, 2, 1 };

foreach (var i in Permutations.Of(seq))
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

The output contains some duplicates, which is expected:

```
1 2 1
1 1 2
2 1 1
2 1 1
1 1 2
1 2 1
```

But if you want to get the *unique permutations*, there exists a straightforward way (note the `Distinct` operation):

``` csharp
var seq = new[] { 1, 2, 1 };

foreach (var i in Permutations.Of(seq).Distinct())
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

which will produce the following output:

```
1 1 2
1 2 1
2 1 1
```

An experienced engineer will definitely spot that while that approach produces the correct result, it is not the most efficient way of achieving it.

It all comes to the number of elements processed by the `Distinct` operation.
The number of resulting permutations is `n!` where `n` is the size of the input sequence.
So if we took care to perform `Distinct` on the input sequence instead, we would achieve considerable savings in number of operations and amount of used memory.
Like so:

``` csharp
foreach (var i in Permutations.Of(seq.Distinct()))
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

(Note that `Distinct` operation is now applied on the input sequence, supposedly making the whole algorithm top-efficient while producing the same results)

This whole way of thinking stands true but `Gapotchenko.FX.Math.Combinatorics` goes ahead of that and provides the out-of-the-box support for such natural idiosyncrasies.

Whatever syntax is preferred: `Permutations.Of(seq.Distinct())` or `Permutations.Of(seq).Distinct()`,
the algorithm complexity stays at bay thanks to the built-in optimizer that chooses the best execution plan for a query automatically.

### Permutations in LINQ

`Permutations.Of(...)` is an explicit form of producing the permutations, but the LINQ shorthand `Permute()` is also available as a part of fluent API:

```csharp
using Gapotchenko.FX.Math.Combinatorics;

foreach (var i in new[] { "A", "B", "C" }.Permute())
    // ...
```

## Cartesian Product

Another widespread combinatorial primitive is Cartesian product:

```csharp
using Gapotchenko.FX.Math.Combinatorics;

var seq1 = new[] { "1", "2" };
var seq2 = new[] { "A", "B", "C" };

foreach (var i in CartesianProduct.Of(seq1, seq2))
    Console.WriteLine(string.Join(" ", i));
```

The output:

```
1 A
2 A
1 B
2 B
1 C
2 C
```

### Cartesian Product in LINQ

`CartesianProduct.Of(...)` is an explicit form of producing the Cartesian product, but the LINQ shorthand `CrossJoin()` is also available as a part of fluent API:

```csharp
using Gapotchenko.FX.Math.Combinatorics;

foreach (var i in new[] { "1", "2" }.CrossJoin(new[] { "A", "B", "C" }))
    // ...
```

Note the naming difference between `CartesianProduct` and `CrossJoin`.
LINQ is historically built around data access,
and data access, in turn, historically uses the term `cross join` for Cartesian product.

## Usage

`Gapotchenko.FX.Math.Combinatorics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Combinatorics):

```
PM> Install-Package Gapotchenko.FX.Math.Combinatorics
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data.Integrity.Checksum](../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - &#x27B4; [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
