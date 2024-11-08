# Gapotchenko.FX.Tuples

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Tuples.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Tuples)

The module provides extended functionality for .NET tuples represented by `System.Tuple` and `System.ValueTuple` types.

## Equality Comparer for Value Tuples

In .NET, value tuples are represented by `System.ValueTuple` type.
They allow to quickly pack several values together without creating dedicated types for that.

Value tuples come with sane defaults, but sometimes you may need custom equality comparers for them.
Let's take a look at example:

``` C#
HashSet<(string, int)> database =
    new()
    {
        // name, age
        ("Alice", 32),
        ("Bob", 40),
        ("John", 14)
    };
```

Let's suppose that we need to search for records in that database by name and age but ignoring the case of letters in the name.
The default value tuple equality comparer is case-sensitive for strings, so the following database query will be unsuccessful:

``` C#
Console.WriteLine("The query result: {0}.", database.Contains(("john", 14)));
// The query result: False.
```

One way to fix that is to manually create a custom `IEqualityComparer<(T1, T2)>` implementation and pass it to the constructor of the `HashSet` class.
Another more simple way to solve the problem is to use `Gapotchenko.FX.Tuples.ValueTupleEqualityComparer` class to quickly create a specialized equality comparer that fits our needs:

``` C#
var equalityComparer = ValueTupleEqualityComparer.Create<string, int>(
    StringComparer.CurrentCultureIgnoreCase,
    null);

HashSet<(string, int)> database =
    new(equalityComparer)
    {
        ("Alice", 32),
        ("Bob", 40),
        ("John", 14)
    };

Console.WriteLine("The query result: {0}.", database.Contains(("john", 14)));
// The query result: True.
```

Now the code works as expected.

## Equality Comparer for Tuples

Tuples are represented by `System.Tuple` types.
The main difference of tuples from value tuples is that `System.Tuple` types are classes while `System.ValueTuple` types are structures.
The remaining part of the concept is almost the same.

`Gapotchenko.FX.Tuples` module allows you to create custom equality comparers for tuples by using `TupleEqualityComparer` class and its `Create` methods.

## Usage

`Gapotchenko.FX.Tuples` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Tuples):

```
PM> Install-Package Gapotchenko.FX.Tuples
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
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading#readme)
- &#x27B4; [Gapotchenko.FX.Tuples](../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
