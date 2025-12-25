# Gapotchenko.FX.Diagnostics.CommandShell

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Diagnostics.CommandShell.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandShell)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Diagnostics.CommandShell.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandShell)

The module provides command shell operations for locating executable files in the system.

## CommandShell

`CommandShell` static class from `Gapotchenko.FX.Diagnostics.CommandShell` module provides operations for locating files using the `PATH` environment variable according to the rules of a host operating system.

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

- **Windows**: Uses `PATHEXT` environment variable to try multiple file extensions
- **Unix/Linux/macOS**: Searches for files with the exact name specified

This makes it easy to write cross-platform code that locates executables correctly on any operating system.

## Usage

`Gapotchenko.FX.Diagnostics.CommandShell` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Diagnostics.CommandShell):

```
dotnet package add Gapotchenko.FX.Diagnostics.CommandShell
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.CommandShell](.#readme)
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
