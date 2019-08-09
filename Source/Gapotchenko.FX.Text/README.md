# Gapotchenko.FX.Text

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Text.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Text)

The module provides primitives for string and text manipulation.

## StringEditor

`StringEditor` class provided by `Gapotchenko.FX.Text` module allows to perform an iterative random-access editing of a string.

It is primarily designed to work in conjunction with `Regex` class from `System.Text.RegularExpressions` namespace to efficiently handle an advanced set of tasks
when functionality provided by conventional methods like `Regex.Replace` is just not enough.

[More on `StringEditor` class](StringEditor.md)

## Regular Expression Trampolines

Regex trampolines are special functions that allow to gradually convert the conventional string manipulation code into one that uses regex semantics.

Let's take a look on example:

``` csharp
if (name.Equals("[mscorlib]System.Object", StringComparison.Ordinal))
    Console.WriteLine("The name represents a system object.");
```

The given code is later changed in order to cover the new requirements:

``` csharp
if (name.Equals("[mscorlib]System.Object", StringComparison.Ordinal) ||
    name.Equals("[netstandard]System.Object", StringComparison.Ordinal) ||
    name.Equals("[System.Runtime]System.Object", StringComparison.Ordinal))
{
    Console.WriteLine("The name represents a system object.");
}
```

It does the job but such mechanical changes may put a toll on code maintainability when they accumulate.
You can also spot some amount of code duplication there.

Another approach would be to use `Regex` class which is readily available in .NET.
But that would destroy the expressiveness of string manipulation functions like `Equals`.
And even more than that.
If a string function takes a `StringComparison` parameter then it may become a significant challenge to reliably refactor it to `Regex` implementation.
It would quickly go beyond the trivial. Surely not something you put into words "mechanical refactoring" and "quick change".

That's why `Gapotchenko.FX.Text` module provides a set of so called regex trampoline functions.
They look exactly like `Equals`, `StartsWith`, `EndsWith`, `IndexOf` but work on regex patterns instead of raw strings.
They also end with `Regex` suffix in their names, so `Equals` has `EqualsRegex` companion, `StartsWith` has `StartsWithRegex` and so on.

This is how a regex trampoline can be used for the given sample to meet the new requirements by a one-line change:

``` csharp
using Gapotchenko.FX.Text.RegularExpressions;

if (name.EqualsRegex(@"\[(mscorlib|netstandard|System\.Runtime)]System\.Object", StringComparison.Ordinal))
    Console.WriteLine("The name represents a system object.");
```

An immediate improvement in expressiveness without duplication.

## StringBuilder Polyfills

### StringBuilder.AppendJoin

`AppendJoin` is a method that appeared in later versions of .NET platform.
Gapochenko.FX provides a corresponding polyfill so it can be used in code targeting older versions.

## Usage

`Gapotchenko.FX.Text` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Text):

```
PM> Install-Package Gapotchenko.FX.Text
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.Drawing](../Gapotchenko.FX.Drawing)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- &#x27B4; [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
