# Gapotchenko.FX.IO.Vfs

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.IO.Vfs.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.Vfs)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.IO.Vfs.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.Vfs)

The module provides a virtual file system (VFS) abstraction layer that enables uniform access to different file storage backends, including local file systems, archives, and custom storage formats.

## FileSystemView

`FileSystemView` static class from `Gapotchenko.FX.IO.Vfs` module provides access to virtual file system views and utilities for working with them.

### Local File System

The most common use case is working with the local file system through the virtual file system interface:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Check if a file exists
if (vfs.FileExists("data.txt"))
{
    // Read a file
    using var stream = vfs.ReadFile("data.txt");
    // ... work with stream
}

// Enumerate files
foreach (string file in vfs.EnumerateFiles("C:\\MyFolder", "*.txt"))
    Console.WriteLine(file);
```

This is very similar to working with local files using `System.IO` namespace.
`IFileSystemView` interface was specifically designed to be a drop-in replacement.

### Working with Files

The module provides provides a unified interface for file operations:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Read all text from a file
string content = vfs.ReadAllText("data.txt");

// Write text to a file
vfs.WriteAllText("output.txt", "Hello, World!");

// Read all lines
string[] lines = vfs.ReadAllLines("data.txt");

// Copy a file
vfs.CopyFile("source.txt", "destination.txt", overwrite: true);

// Delete a file
vfs.DeleteFile("old.txt");
```

### Working with Directories

Directory operations are also supported:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Check if directory exists
if (vfs.DirectoryExists("MyFolder"))
{
    // Enumerate directories
    foreach (string dir in vfs.EnumerateDirectories("MyFolder"))
    {
        Console.WriteLine(dir);
    }
}

// Create a directory
vfs.CreateDirectory("NewFolder");

// Delete a directory
vfs.DeleteDirectory("OldFolder", recursive: true);
```

### Asynchronous Operations

The module provides comprehensive support for asynchronous file and directory operations, allowing you to perform I/O operations without blocking threads:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Asynchronously read all text from a file
string content = await vfs.ReadAllFileTextAsync("data.txt");

// Asynchronously write text to a file
await vfs.WriteAllFileTextAsync("output.txt", "Hello, World!");

// Asynchronously read all bytes
byte[] data = await vfs.ReadAllFileBytesAsync("binary.dat");

// Asynchronously open a file for reading
using var stream = await vfs.ReadFileAsync("data.txt");

// Asynchronously open a file with specific mode and access
await using var writeStream = await vfs.OpenFileAsync(
    "output.txt", 
    FileMode.Create, 
    FileAccess.Write, 
    FileShare.None);

// Asynchronously copy a file
await vfs.CopyFileAsync("source.txt", "destination.txt". overwrite: true);

// Asynchronously move a file
await vfs.MoveFileAsync("old.txt", "new.txt", overwrite: false);

// Asynchronously delete a file
await vfs.DeleteFileAsync("old.txt");

// Asynchronously create a directory
await vfs.CreateDirectoryAsync("NewFolder");

// Asynchronously delete a directory
await vfs.DeleteDirectoryAsync("OldFolder", recursive: true);

// Asynchronously read file lines
await foreach (string line in vfs.ReadFileLinesAsync("data.txt"))
{
    Console.WriteLine(line);
}
```

All asynchronous methods support cancellation tokens for cooperative cancellation:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;
var cancellationToken = ...;

// Asynchronously read with cancellation support
string content = await vfs.ReadAllFileTextAsync("data.txt", cancellationToken);

// Asynchronously write with cancellation support
await vfs.WriteAllFileTextAsync("output.txt", "Hello, World!", cancellationToken);
```

### Capabilities

Virtual file system views expose capabilities that indicate what operations are supported:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

Console.WriteLine($"Can read: {vfs.CanRead}");
Console.WriteLine($"Can write: {vfs.CanWrite}");
Console.WriteLine($"Supports creation time: {vfs.SupportsCreationTime}");
Console.WriteLine($"Supports last write time: {vfs.SupportsLastWriteTime}");
Console.WriteLine($"Supports attributes: {vfs.SupportsAttributes}");
```

### Enforcing Capabilities

You can create a view with enforced capabilities:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Create a read-only view
var readOnlyView = FileSystemView.WithCapabilities(vfs, canRead: true, canWrite: false);
```

### Path Operations

The virtual file system provides path manipulation methods that work consistently across different storage backends:

``` C#
using Gapotchenko.FX.IO.Vfs;

var vfs = FileSystemView.Local;

// Combine paths
string fullPath = vfs.CombinePaths("folder1", "folder2", "file.txt");

// Get directory name
string? dirName = vfs.GetDirectoryName("C:\\Folder\\File.txt");

// Get file name
string? fileName = vfs.GetFileName("C:\\Folder\\File.txt");

// Get full path
string full = vfs.GetFullPath("relative/path/file.txt");

// Check if path is rooted
bool isRooted = vfs.IsPathRooted("C:\\Folder");
```

## Virtual File Systems

The module is designed to support custom virtual file system implementations. You can implement `IFileSystemView` or `IReadOnlyFileSystemView` to create your own file system backend, such as:

- Archive-based file systems (ZIP, TAR, etc.)
- Network file systems
- In-memory file systems
- Encrypted file systems
- Versioned file systems

### Implementing a Custom File System

To implement a custom virtual file system, you can inherit from `VirtualFileSystemKit` which provides a base implementation:

``` C#
using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;

public class MyCustomFileSystem : VirtualFileSystemKit
{
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override bool FileExists(string? path)
    {
        // Implement your logic
        return false;
    }

    public override Stream ReadFile(string path)
    {
        // Implement your logic
        throw new NotImplementedException();
    }

    // Implement other required methods...
}
```

## Usage

`Gapotchenko.FX.IO.Vfs` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.IO.Vfs):

```
dotnet package add Gapotchenko.FX.IO.Vfs
```

## Related Modules

- [Gapotchenko.FX.Data.Archives](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme) - Provides base functionality for data archives
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO#readme) - Provides extended I/O functionality for the local file system

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO#readme)
  - &#x27B4; [Gapotchenko.FX.IO.Vfs](.#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../..#readme).
