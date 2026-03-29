# Gapotchenko.FX.Diagnostics.CommandLine

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Diagnostics.CommandLine.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandLine)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Diagnostics.CommandLine.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandLine)

The module provides primitives for command line manipulation.

## CommandLineBuilder

`CommandLineBuilder` class allows you to dynamically build a command line.
It provides the built-in support for characters than need escaping.

Semantically `CommandLineBuilder` is similar to `StringBuilder` class:

``` C#
using Gapotchenko.FX.Diagnostics;

var clb = new CommandLineBuilder();
clb.AppendParameter("/b");
clb.AppendFileName(@"C:\Temp\Test 1.txt");
clb.AppendFileName(@"C:\Temp\Test 2.txt");

Console.WriteLine(clb.ToString());
```

The code above produces the following output:

```
/b "C:\Temp\Test 1.txt" "C:\Temp\Test 2.txt"
```

Note how some command-line parameters were automatically quoted because they contained whitespace characters.

`CommandLineBuilder` supports a fluent interface, just like conventional `StringBuilder`.
So the code can be rewritten as:

``` C#
var clb = new CommandLineBuilder()
    .AppendParameter("/b")
    .AppendFileName(@"C:\Temp\Test 1.txt")
    .AppendFileName(@"C:\Temp\Test 2.txt");

Console.WriteLine(clb.ToString());
```

The resulting command line can be used in various places, most notably for starting a new process:

``` C#
using System.Diagnostics;

Process.Start("copy", clb.ToString());
```

## CommandLine

`CommandLine` static class provides operations for command line manipulation.

### Build

`CommandLine.Build` method allows you to quickly build a command-line string from a specified list of arguments.
Basically, this is a shortcut to `CommandLineBuilder` class in a handy functional form:

``` C#
string commandLine = CommandLine.Build("/b", @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt");
```

Such a form is very useful in something like this:

``` C#
Process.Start(
    "cmd",
    CommandLine.Build(
        "/C", "copy",
        "/b", @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt"));
```

Another cool thing: if you want to exclude some arguments from the command line then you can just make them `null`:

``` C#
// 'mode' will have a non-null value if there is a need to specify it.
string? mode = binary ? "/b" : null;

string commandLine = CommandLine.Build(mode, @"C:\Temp\Test 1.txt", @"C:\Temp\Test 2.txt");
Console.WriteLine(commandLine);
```

The code above produces the following outputs depending on the value of `binary` flag:

```
"C:\Temp\Test 1.txt" "C:\Temp\Test 2.txt"
/b "C:\Temp\Test 1.txt" "C:\Temp\Test 2.txt"
```

This is a neat departure from a traditional .NET convention where it always throws `ArgumentNullException`.
Instead, Gapotchenko.FX uses a slightly different philosophy.
It does the best job possible under existing conditions by following common-sense expectations.

### Split

`CommandLine.Split` provides the inverse operation to `CommandLine.Build`.
It allows you to split a command-line string into a list of arguments using the rules of a host operating system:

``` C#
using Gapotchenko.FX.Diagnostics;

string commandLine = "/b \"C:\\Temp\\Test 1.txt\" \"C:\\Temp\\Test 2.txt\"";

foreach (string arg in CommandLine.Split(commandLine))
    Console.WriteLine(arg);
```

The code above produces the following output:

```
/b
C:\Temp\Test 1.txt
C:\Temp\Test 2.txt
```

## CommandShell

`CommandShell` static class from `Gapotchenko.FX.Diagnostics.CommandLine` module provides operations for locating files using the `PATH` environment variable according to the rules of a host operating system.

### Where

The `Where` method enumerates the paths of a file with the specified name using the `PATH` environment variable. It is similar to the Windows `where` command or Unix `which` command, but works cross-platform.

The basic usage is straightforward:

``` C#
using Gapotchenko.FX.Diagnostics;

foreach (string path in CommandShell.Where("notepad"))
    Console.WriteLine(path);
```

On Windows, this might produce output like:

```
C:\Windows\System32\notepad.exe
C:\Windows\notepad.exe
```

The method respects the operating system's file discovery rules:
- On Windows, it uses the `PATHEXT` environment variable to try different file extensions (`.exe`, `.cmd`, `.bat`, etc.)
- On Unix-like systems, it searches for files with the exact name specified
- It handles case sensitivity according to the host operating system

#### `Where` with Probing Paths

You can also specify custom probing paths that will be checked before the `PATH` environment variable:

``` C#
using Gapotchenko.FX.Diagnostics;

// Check custom directories first, then PATH
string[] customPaths = [@"C:\MyTools", @"C:\MyDir"];
foreach (string path in CommandShell.Where("myapp", customPaths))
    Console.WriteLine(path);
```

This is useful when you want to prioritize certain directories or check additional locations beyond the standard `PATH`.

#### Handling Paths with Directory Information

If the file name contains directory information, `Where` method will only search in that specific directory:

``` C#
using Gapotchenko.FX.Diagnostics;

// Only searches in "C:\MyDir" directory
foreach (string path in CommandShell.Where(@"C:\MyDir\myapp"))
    Console.WriteLine(path);
```

#### Cross-Platform Behavior

The `Where` method automatically adapts to the host operating system:

- **On Windows**: uses `PATHEXT` environment variable to try multiple file extensions
- **On Unix/Linux/macOS**: searches for files with the exact name specified

This makes it easy to write cross-platform code that locates executable files correctly on any operating system.

## Usage

`Gapotchenko.FX.Diagnostics.CommandLine` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandLine):

```
dotnet package add Gapotchenko.FX.Diagnostics.CommandLine
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](#)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.CommandLine](.#readme)
  - [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process#readme)
  - [Gapotchenko.FX.Diagnostics.WebBrowser](../Gapotchenko.FX.Diagnostics.WebBrowser#readme)
- [Gapotchenko.FX.IO](../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../..#readme).
