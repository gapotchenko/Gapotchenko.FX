# Gapotchenko.FX.Text

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Text.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Text)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Text.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Text)

The module provides primitives for string and text manipulation.

## StringEditor

`StringEditor` class provided by `Gapotchenko.FX.Text` module allows to perform an iterative random-access editing of a string.

It is primarily designed to work in conjunction with `Regex` class from `System.Text.RegularExpressions` namespace to efficiently handle an advanced set of tasks
when functionality provided by conventional methods like `Regex.Replace` is just not enough.

[More on `StringEditor` class](StringEditor.md)

## Regular Expression Trampolines

Regex trampolines are special functions that allow to gradually convert the conventional string manipulation code into one that uses regex semantics.

Let's take a look on example:

``` C#
if (name.Equals("[mscorlib]System.Object", StringComparison.Ordinal))
    Console.WriteLine("The name represents a system object.");
```

The given code is later changed in order to cover the new requirements:

``` C#
if (name.Equals("[mscorlib]System.Object", StringComparison.Ordinal) ||
    name.Equals("[netstandard]System.Object", StringComparison.Ordinal) ||
    name.Equals("[System.Runtime]System.Object", StringComparison.Ordinal))
{
    Console.WriteLine("The name represents a system object.");
}
```

It does the job but such mechanical changes may put a toll on code maintainability when they accumulate.
You can also spot some amount of code duplication here.

Another approach would be to use `Regex` class which is readily available in .NET.
But that might destroy the expressiveness of string manipulation functions like `Equals`.
If a string function takes a `StringComparison` parameter then it may become a significant challenge to reliably refactor it to `Regex` implementation.


That's why `Gapotchenko.FX.Text` module provides a set of so called regex trampoline functions.
They look exactly like `Equals`, `StartsWith`, `EndsWith`, `IndexOf` but work with regex patterns instead of raw strings.
They also end with `Regex` suffix in their names, so `Equals` becomes `EqualsRegex`, `StartsWith` correspondingly becomes `StartsWithRegex`, and so on.

And this is how a regex trampoline can be used for the given sample in order to meet the new requirements by a single line change:

``` C#
using Gapotchenko.FX.Text.RegularExpressions;

if (name.EqualsRegex(@"\[(mscorlib|netstandard|System\.Runtime)]System\.Object", StringComparison.Ordinal))
    Console.WriteLine("The name represents a system object.");
```

An immediate improvement in expressiveness without duplication.

## StringBuilder Polyfills

### AppendJoin

`StringBuilder.AppendJoin` is a method that appeared in later versions of .NET platform.
Gapochenko.FX provides a corresponding polyfill that can be used in code targeting older .NET versions.

The benefit of this method is that it combines `string.Join` and `StringBuilder.Append` operations in one method,
while using the underlying efficiency of the `StringBuilder`.

## Usage

`Gapotchenko.FX.Text` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Text):

```
PM> Install-Package Gapotchenko.FX.Text
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
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Runtime.InteropServices](../Gapotchenko.FX.Runtime.InteropServices)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- &#x27B4; [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
