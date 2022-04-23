# Gapotchenko.FX.Diagnostics.CommandLine

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Diagnostics.CommandLine.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandLine)

The module provides primitives for command line manipulation.

## CommandLineBuilder

`CommandLineBuilder` class from `Gapotchenko.FX.Diagnostics.CommandLine` module allows to dynamically build a command line on the fly.
It provides the built-in support for characters than need escaping.

Semantically `CommandLineBuilder` is similar to `StringBuilder` class:

``` csharp
using Gapotchenko.FX.Diagnostics;

var clb = new CommandLineBuilder();
clb.AppendParameter("/b");
clb.AppendParameter(@"C:\Temp\Test 1.txt");
clb.AppendParameter(@"C:\Temp\Test 2.txt");

Console.WriteLine(clb.ToString());
```

The code above produces the following output:

```
/b "C:\Temp\Test 1.txt" "C:\Temp\Test 2.txt"
```

Note how some command line parameters were automatically quoted because they contained whitespace characters.

`CommandLineBuilder` supports a fluent interface just like conventional `StringBuilder`,
so the code can be rewritten as:

``` csharp
var clb = new CommandLineBuilder()
    .AppendParameter("/b")
    .AppendParameter(@"C:\Temp\Test 1.txt")
    .AppendParameter(@"C:\Temp\Test 2.txt");

Console.WriteLine(clb.ToString());
```

The resulting command line can be used in various places, most notably for starting a new process:

``` csharp
using System.Diagnostics;

…
Process.Start("copy", clb.ToString());
```

## CommandLine

`CommandLine` static class provides operations for command line manipulation.

### Build

`CommandLine.Build` method allows to quickly build a command line string from a specified list of arguments.
Basically, this is a shortcut to `CommandLineBuilder` class in a handy functional form:

``` csharp
string s = CommandLine.Build("/b", @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt");
```

Such a form is very useful in something like this:

``` csharp
Process.Start(
    "cmd",
    CommandLine.Build(
        "/C", "copy",
        "/b", @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt"));
```

Another cool thing: if you want to exclude some argument from a command line then you can just make it `null`:

``` csharp
string mode = null;

// 'mode' can be assigned to a non-null value if there is a need to specify it.

string s = CommandLine.Build(mode, @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt");
Console.WriteLine(s);
```

The code above produces the following output:

```
"C:\Temp\Test 1.txt" "C:\Temp\Test 2.txt"
```

This is a neat departure from a traditional .NET convention where it always throws `ArgumentNullException`.
Instead, Gapotchenko.FX uses a slightly different philosophy.
It does the best job possible under existing conditions by following common-sense expectations of a user.


### Split

`CommandLine.Split` provides the opposite operation to `CommandLine.Build`.
It allows to split a command line string into a list of arguments using the rules of a host operating system:

``` csharp
using Gapotchenko.FX.Diagnostics;

string s = "/b \"C:\\Temp\\Test 1.txt\" \"C:\\Temp\\Test 2.txt\"";

foreach (string arg in CommandLine.Split(s))
    Console.WriteLine(arg);
```

The code above produces the following output:

```
/b
C:\Temp\Test 1.txt
C:\Temp\Test 2.txt
```

## Usage

`Gapotchenko.FX.Diagnostics.CommandLine` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandLine):

```
PM> Install-Package Gapotchenko.FX.Diagnostics.CommandLine
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Data.Integrity.Checksum](../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Diagnostics](#)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine)
  - [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
  - [Gapotchenko.FX.Diagnostics.WebBrowser](../Gapotchenko.FX.Diagnostics.WebBrowser)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
