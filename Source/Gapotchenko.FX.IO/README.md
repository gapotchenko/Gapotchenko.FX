# Gapotchenko.FX.IO

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.IO.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO)

The module provides highly demanded input/output functionality that is missing in conventional .NET platform.

## FileSystem

`FileSystem` is a class provided by `Gapotchenko.FX.IO`.
If offers extended I/O functions for file system,
and serves as an important addendum to a conventional `System.IO` namespace.

### IsCaseSensitive

The `FileSystem.IsCaseSensitive` property shows whether the current host operating system uses case sensitive file names.

For example, Windows operating system uses case-**in**sensitive file names,
so `FileSystem.IsCaseSensitive` returns `false`.
The same goes to macOS.

However, Linux and other Unix flavors use case-sensitive file names by default.
Whenever an app creates two files named `Test.txt` and `test.txt`,
those files are distinct and can coexist at the same folder.
`FileSystem.IsCaseSensitive` returns `true` on such operating systems.

### PathComparer

`FileSystem.PathComparer` property returns a string comparer for file names.

Please take a look at the code below:

``` csharp
using Gapotchenko.FX.IO;

var files = new HashSet<string>(FileSystem.PathComparer);

files.Add("Test.txt");
files.Add("test.txt");

Console.WriteLine("Count of files: {0}", files.Count);
```

The given set would contain one entry on Windows, and two entries on Linux.

### PathComparison

`FileSystem.PathComparison` property returns a `StringComparison` value that signifies a file name comparison mode used by the host OS.

It can be used in string comparison operations:

``` csharp
using Gapotchenko.FX.IO;

void ProcessFile(string filePath)
{
    if (!filePath.EndsWith(".txt", FileSystem.PathComparison))
        throw new Exception("Only text files can be processed.");
    …
}

```

### PathsAreEquivalent(string a, string b)

Determines whether the given paths are equivalent. If they point to the same file system entry then the method returns `true`; otherwise, `false`.

The problem this method solves is caused by the fact that a file path can be specified in multiple forms:
- `Test.txt` (relative path)
- `C:\Temp\Test.txt` (absolute path)

Let's take a look at code:

``` csharp
using Gapotchenko.FX.IO;

Directory.SetCurrentDirectory(@"C:\Temp");

string fileA = "Test.txt";
string fileB = @"C:\Temp\Test.txt";

Console.WriteLine("String equality: {0}", string.Equals(fileA, fileB));
Console.WriteLine("Path equivalence: {0}", FileSystem.PathsAreEquivalent(fileA, fileB));
```

It produces the following results:

```
String equality: False
Path equivalence: True
```

Note that file equivalence check is positive despite the different forms of a file path.

### PathStartsWith(string path, string value)

Determines whether the beginning of the path matches the specified value in terms of file system equivalence.

Say we have a folder name `Contoso\Reports\2012\Final`.
How do we know that it starts with `Contoso\Reports`?

A straightforward solution would be to use `String.StartsWith` function, like so:

``` csharp
bool IsContosoReportsFolder(string path) => path.StartsWith(@"Contoso\Reports");
```

It kind of works, until we try to pass something like `Contoso\ReportsBackup`.
The problem is `ReportsBackup` is a very different folder than `Reports`, but the provided function returns `true`.

We can cheat here, and try to use an updated function that adds a trailing slash:

``` csharp
bool IsContosoReportsFolder(string path) => path.StartsWith(@"Contoso\Reports\");
```

The problem is gone.
Until we ask for `IsContosoReportsFolder("Contoso\Reports")` value.
It is `false` now despite the fact that `Contoso\Reports` is literally a Contoso reports folder we are so eagerly looking for.

The correct solution is to use `FileSystem.PathStartsWith` method provided by `Gapotchenko.FX.IO` module:

``` csharp
using Gapotchenko.FX.IO;

bool IsContosoReportsFolder(string path) => FileSystem.PathStartsWith(path, @"Contoso\Reports");
```

It will now give the correct results for all inputs, even when they use alternative directory separators:

``` csharp
IsContosoReportsFolder(@"Contoso\ReportsBackup") => false
IsContosoReportsFolder(@"Contoso\Reports\2012\Final") => true
IsContosoReportsFolder(@"Contoso\Reports") => true
IsContosoReportsFolder(@"Contoso/Reports/2019/Progress") => true
```

### WaitForFileWriteAccess(path)

`FileSystem.WaitForFileWriteAccess` method is a subtle but important primitive. It waits for a write access to the specified file.

Why would anyone want such a method?
It turns out that a modern OS is a noisy environment that can put your app under a sledgehammer.

For example, if an app changes a file, it immediately grabs attention of various OS services.
Anti-virus tools, search engines, file synchronization applications all can lock the files for short random time spans.

If a user of your app is unlucky or just uses an app frequently enough then he would occasionally get "File access denied" errors.

To minimize a possibility of such a congestion, you should call `FileSystem.WaitForFileWriteAccess` method before changing a file:

``` csharp
using Gapotchenko.FX.IO;

string fileName = "Results.txt";
FileSystem.WaitForFileWriteAccess(fileName);
File.WriteAllText(fileName, "A user can now use the app without occasional 'File access denied' errors.");
```

What it does is polls the file until write access is available.
If the access is not there for 10 seconds, the method falls through.

[More on this topic (Raymond Chen, "The Old New Thing" blog)](https://devblogs.microsoft.com/oldnewthing/?p=6663)

## BitReader/BitWriter

`BitReader` and `BitWriter` classes provided by `Gapotchenko.FX.IO` extend the functionality of conventional `BinaryReader` and `BinaryWriter` by inheriting from them.

The conventional `BinaryReader`/`BinaryWriter` combo only supports little-endian byte order.
However, big-endian byte order is equally widespread.

This is how a big-endian binary reader can be created:

``` csharp
using Gapotchenko.FX;
using Gapotchenko.FX.IO;

var br = new BitReader(BigEndianBitConverter.Instance);
```

Thanks to the fact that `BitReader` is inherited from `BinaryReader` class, it is almost a drop-in replacement.
The same goes to `BitWriter`.

## Usage

`Gapotchenko.FX.IO` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.IO):

```
PM> Install-Package Gapotchenko.FX.IO
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- &#x27B4; [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
