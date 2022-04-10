# Gapotchenko.FX.Linq

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Linq.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Linq)

The module provides primitives for functional processing of data sequences.

## Memoize

Memoization is a pillar of functional data processing.
You already met it quite often albeit in somewhat masked forms.

`Gapotchenko.FX.Linq` module provides the `Memoize()` extension method for `IEnumerable<T>` types.
You can use it like this:

``` csharp
using Gapotchenko.FX.Linq;

var files = Directory.EnumerateFiles(".", "*.txt").Select(Path.GetFileName).Memoize();

Console.WriteLine("A text file with an upper case letter is present: {0}", files.Any(x => x.Any(char.IsUpper)));
Console.WriteLine("A text file with a name longer than 12 letters is present: {0}", files.Any(x => x.Length > 12));
```

`Memoize()` caches the already retrieved elements of a sequence, and does it lazily.

.NET developers often use `ToList()` and `ToArray()` methods for the very same purpose.
But those methods are eager, as they retrieve _all_ elements of a sequence in one shot.
This often leads to suboptimal performance of an otherwise sound functional algorithm.

`Memoize()` solves that. This is the method you are going to use the most for LINQ caching.

## ScalarOrDefault

The second most popular primitive provided by `Gapotchenko.FX.Linq` module is `ScalarOrDefault()` method.
It is similar to `SingleOrDefault()` from conventional .NET but with one big difference: it does not throw an exception when sequence contains multiple elements.

### *Single*OrDefault() Semantics

``` csharp
using System.Linq;

new string[0].SingleOrDefault(); // returns null
new[] { "A" }.SingleOrDefault(); // returns "A"
new[] { "A", "B" }.SingleOrDefault(); // throws an exception 😞
```

### *Scalar*OrDefault() Semantics

``` csharp
using Gapotchenko.FX.Linq;

new string[0].ScalarOrDefault(); // returns null
new[] { "A" }.ScalarOrDefault(); // returns "A"
new[] { "A", "B" }.ScalarOrDefault(); // returns null 👍
```

In practice, `ScalarOrDefault()` semantics is a big win as it allows to _safely_ determine whether a given query converges to a scalar result.

## DistinctBy

Returns distinct elements from a sequence by using the default equality comparer on the keys extracted by a specified selector function.

The method is similar to `Distinct()` method provided by the stock `System.Linq` namespace, but allows to specify a selector function in order to differentiate the elements by a specific criteria.

Let's take a look at example:

``` csharp
using Gapotchenko.FX.Linq;

var source = new[]
{
    new { FirstName = "Alex", LastName = "Cooper" },
    new { FirstName = "John", LastName = "Walker" },
    new { FirstName = "Alex", LastName = "The Great" },
    new { FirstName = "Jeremy", LastName = "Doer" }
};

var query = source.DistinctBy(x => x.FirstName);

foreach (var i in query)
    Console.WriteLine(i);
```

The code produces the following output:

```
{ FirstName = Alex, LastName = Cooper }
{ FirstName = John, LastName = Walker }
{ FirstName = Jeremy, LastName = Doer }
```

## MinBy/MaxBy

Returns a minimum/maximum value in a sequence according to a specified key selector function.

Let's take a look at example:

``` csharp
using Gapotchenko.FX.Linq;

var source = new[]
{
    new { FirstName = "Alex", LastName = "Cooper", Age = 45 },
    new { FirstName = "John", LastName = "Walker", Age = 17 },
    new { FirstName = "Alex", LastName = "The Great", Age = 1500 },
    new { FirstName = "Jeremy", LastName = "Doer", Age = 29 }
};

Console.WriteLine("The oldest person: {0}", source.MaxBy(x => x.Age));
Console.WriteLine("The youngest person: {0}", source.MinBy(x => x.Age));
```

The code produces the following output:

```
The oldest person: { FirstName = Alex, LastName = The Great, Age = 1500 }
The youngest person: { FirstName = John, LastName = Walker, Age = 17 }
```

## MinOrDefault/MaxOrDefault

These methods augment the semantics of conventional `Min` and `Max` methods, allowing to return the default value when the input sequence is empty.
Conventional `Min` and `Max` methods just throw an exception in that case.

## Usage

`Gapotchenko.FX.Linq` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Linq):

```
PM> Install-Package Gapotchenko.FX.Linq
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
- &#x27B4; [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
