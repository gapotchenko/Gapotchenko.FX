# Gapotchenko.FX.Data.Archives.Zip

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Archives.Zip.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Archives.Zip)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Data.Archives.Zip.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Archives.Zip)

The module provides ZIP archive functionality that integrates with the virtual file system (VFS) abstraction layer, allowing you to treat ZIP archives as file systems.

## ZipArchive

`ZipArchive` class from `Gapotchenko.FX.Data.Archives.Zip` module represents a ZIP archive and implements `IFileSystemView`, which means you can use it just like a regular file system.

### Creating a ZIP Archive

You can create a new ZIP archive:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using Gapotchenko.FX.IO.Vfs;

// Create a new writable ZIP archive
using var zip = ZipArchive.Storage.CreateFile("archive.zip");

// Now you can use it as a file system
zip.WriteAllText("readme.txt", "Hello from ZIP archive!");
```

### Reading from a ZIP Archive

Reading from a ZIP archive is straightforward:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using Gapotchenko.FX.IO.Vfs;

// Open an existing ZIP archive for reading
using var zip = ZipArchive.Storage.ReadFile("archive.zip");

// Check if a file exists
if (zip.FileExists("readme.txt"))
{
    // Read file content
    string content = zip.ReadAllText("readme.txt");
    Console.WriteLine(content);
}
```

### Working with Files

Since `ZipArchive` implements `IFileSystemView`, you can use all the typical file operations:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using Gapotchenko.FX.IO.Vfs;

using var zip = ZipArchive.Storage.OpenFile("archive.zip", FileMode.OpenOrCreate);

// Write text to a file
zip.WriteAllText("data.txt", "Some content");

// Read all lines
string[] lines = zip.ReadAllLines("data.txt");

// Copy a file
zip.CopyFile("data.txt", "backup.txt", overwrite: true);

// Delete a file
zip.DeleteFile("old.txt");
```

### Enumerating Files

You can enumerate files and directories in a ZIP archive:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using System.IO;

using var zip = ZipArchive.Storage.ReadFile("archive.zip");

// Enumerate all files
foreach (string file in zip.EnumerateFiles("/"))
    Console.WriteLine(file);

// Enumerate files with a pattern
foreach (string file in zip.EnumerateFiles("/", "*.txt"))
    Console.WriteLine(file);

// Enumerate directories
foreach (string dir in zip.EnumerateDirectories("/"))
    Console.WriteLine(dir);
```

### Working with Directories

Directory operations are also supported:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using System.IO;

using var zip = ZipArchive.Storage.CreateFile("archive.zip");

// Create a directory structure
zip.CreateDirectory("folder1/subfolder");

// Write files to directories
zip.WriteAllText("folder1/file.txt", "Content");

// Check if directory exists
if (zip.DirectoryExists("folder1"))
{
    // Enumerate files in directory
    foreach (string file in zip.EnumerateFiles("folder1"))
        Console.WriteLine(file);
}
```

### Stream Management

You can control whether the underlying stream should remain open after disposing the archive:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using System.IO;

var stream = new FileStream("archive.zip", FileMode.Create);

// Leave the stream open after disposing the archive
using (var zip = new ZipArchive(stream, writable: true, leaveOpen: true))
    zip.WriteAllText("file.txt", "Content");

// Archive is disposed, but stream remains open
// ... stream can be used further
```

### Capabilities

`ZipArchive` exposes capabilities that indicate what operations are supported:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using System.IO;

using var stream = new FileStream("archive.zip", FileMode.Open);
using var zip = new ZipArchive(stream, writable: false);

Console.WriteLine($"Can read: {zip.CanRead}");
Console.WriteLine($"Can write: {zip.CanWrite}");
Console.WriteLine($"Supports last write time: {zip.SupportsLastWriteTime}");
```

### Integration with Virtual File System

Since `ZipArchive` implements `IFileSystemView`, it can be used anywhere a file system view is expected. This allows you to write code that works with both regular file systems and ZIP archives:

``` C#
using Gapotchenko.FX.Data.Archives.Zip;
using Gapotchenko.FX.IO.Vfs;

// Works with local file system
ProcessFiles(FileSystemView.Local);

// Also works with ZIP archive
using (var zip = ZipArchive.Storage.ReadFile("archive.zip"))
{
    ProcessFiles(zip);

    // Works with a nested archive as well
    using (var nestedZip = ZipArchive.Storage.ReadFile(new VfsLocation(zip, "nested.zip"))
        ProcessFiles(nestedZip);
}

void ProcessFiles(IReadOnlyFileSystemView vfs)
{
    if (vfs.FileExists("data.txt"))
    {
        string content = vfs.ReadAllText("data.txt");
        // Process content...
    }
}
```

## Usage

`Gapotchenko.FX.Data.Archives.Zip` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Archives.Zip):

```
dotnet package add Gapotchenko.FX.Data.Archives.Zip
```

## Related Modules

- [Gapotchenko.FX.IO.Vfs](../../IO/Gapotchenko.FX.IO.Vfs#readme) - Provides the virtual file system abstraction layer
- [Gapotchenko.FX.Data.Archives](../Gapotchenko.FX.Data.Archives#readme) - Provides base functionality for data archives

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Gapotchenko.FX.Data.Archives#readme)
  - [Gapotchenko.FX.Data.Archives](../Gapotchenko.FX.Data.Archives#readme)
    - &#x27B4; [Gapotchenko.FX.Data.Archives.Zip](.#readme)
  - [Gapotchenko.FX.Data.Encoding](../../Encoding/Gapotchenko.FX.Data.Encoding#readme)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum#readme)
- [Gapotchenko.FX.Diagnostics](../../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../../..#readme).
