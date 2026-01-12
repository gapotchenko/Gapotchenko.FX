# Gapotchenko.FX.Linq.Async

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Linq.Async.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Linq.Async)

The module provides asynchronous LINQ extensions and utilities for working with `IAsyncEnumerable<T>` sequences, including bridging between synchronous and asynchronous enumeration models.

## `AsyncEnumerableBridge`

`AsyncEnumerableBridge` static class provides methods to bridge synchronous and asynchronous enumeration models together, allowing seamless conversion between `IEnumerable<T>` and `IAsyncEnumerable<T>`.

### Converting Asynchronous Enumerable to Synchronous

You can synchronously enumerate values from an asynchronous source:

``` C#
using Gapotchenko.FX.Linq;

IAsyncEnumerable<string> asyncSource = GetAsyncStrings();

// Synchronously enumerate from async source
foreach (string item in AsyncEnumerableBridge.Enumerate(asyncSource))
    Console.WriteLine(item);

```

### Converting Synchronous Enumerable to Asynchoronous

You can asynchronously enumerate values from a synchronous source:

``` C#
using Gapotchenko.FX.Linq;

IEnumerable<string> syncSource = GetStrings();

// Asynchronously enumerate from sync source
await foreach (string item in AsyncEnumerableBridge.EnumerateAsync(syncSource))
    Console.WriteLine(item);

// With cancellation support
var cancellationToken = ...;
await foreach (string item in AsyncEnumerableBridge.EnumerateAsync(syncSource, cancellationToken))
    Console.WriteLine(item);
```

When cancellation is requested, the thread executing the synchronous enumeration is terminated.

## ScalarOrDefaultAsync

The `ScalarOrDefaultAsync()` extension method provides safe scalar extraction from asynchronous sequences. Unlike `SingleOrDefaultAsync()` from conventional .NET, it does not throw an exception when a sequence contains multiple elements.

The method follows the same safe semantics as `ScalarOrDefault()` from `Gapotchenko.FX.Linq`:

- Returns the element if the sequence contains exactly one element
- Returns the default value if the sequence is empty
- Returns the default value if the sequence contains multiple elements

This allows you to safely determine whether a given asynchronous query converges to a scalar result without exception handling.

## Usage

`Gapotchenko.FX.Linq.Async` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Linq.Async):

```
dotnet package add Gapotchenko.FX.Linq.Async
```

## Related Modules

- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq#readme) - Provides synchronous LINQ extensions and primitives
- [Gapotchenko.FX.Threading](../../Threading/Gapotchenko.FX.Threading#readme) - Provides threading primitives, including bridging between synchronous and asynchronous execution models

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq#readme)
  - &#x27B4; [Gapotchenko.FX.Linq.Async](.#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Threading/Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../..#readme).
