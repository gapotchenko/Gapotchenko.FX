# Gapotchenko.FX.IO.FileSystems.MSCfb

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.IO.FileSystems.MSCfb.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.FileSystems.MSCfb)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.IO.FileSystems.MSCfb.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.FileSystems.MSCfb)

This module implements the MS-CFB (Compound File Binary Format) file system and integrates it with the virtual file system (VFS) abstraction layer, allowing compound files to be accessed and manipulated using a unified file system interface.

## Overview

[MS-CFB](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-cfb/) is a structured storage format that embeds a complete file system within a single file.
It underpins a variety of widespread file formats, such as Microsoft Office binary documents, `.msi` files, and many others.

`Gapotchenko.FX.IO.FileSystems.MSCfb` exposes a compound file as `MSCfbFileSystem`, which implements `IMSCfbFileSystem` and the usual VFS surface (`IVirtualFileSystem` / `IFileSystemView`), including streams, directories, enumeration, and timestamps.

Paths in `MSCfbFileSystem` are case-insensitive, in accordance with the format specification.

Conceptually, the MS-CFB format can be viewed as a compact, portable, and most importantly standardized file system encapsulated within a single file, similar in spirit to FAT12 or FAT16.

## `MSCfbFileSystem`

`MSCfbFileSystem` class is the primary entry point for working with a compound file.
It can be instantiated directly over an existing `System.IO.Stream`,
or accessed via `MSCfbFileSystem.Storage` property to open or create a compound file on disk (or any other backing store exposed through `VfsLocation` or `VfsReadOnlyLocation`).

### Opening from a stream

For a compound file you already have as a stream (for example from a network or an in-memory buffer):

``` C#
using Gapotchenko.FX.IO.FileSystems.MSCfb;
using Gapotchenko.FX.IO.Vfs;

void ReadFromStream(Stream stream)
{
    using var vfs = new MSCfbFileSystem(stream);

    if (vfs.FileExists("README.txt"))
    {
        string text = vfs.ReadAllFileText("README.txt");
        // ...
    }
}
```

Use the constructor overloads when you need a writable file system, leave-open behavior, or `MSCfbFileSystemOptions`. There is also `MSCfbFileSystem.CreateAsync` for asynchronous construction.

### Opening or creating a file via storage

`MSCfbFileSystem.Storage` property implements `IFileSystemStorage<IMSCfbFileSystem, MSCfbFileSystemOptions>` interface and follows the same patterns as other mountable VFS types: read-only open, create/overwrite, and full `OpenFile`/`WriteFile`/`ReadFile` semantics via extension methods provided by`Gapotchenko.FX.IO.Vfs` module:

``` C#
using Gapotchenko.FX.IO.FileSystems.MSCfb;
using Gapotchenko.FX.IO.Vfs;

// Open an existing compound file for reading (from a local path)
using var cfb = MSCfbFileSystem.Storage.ReadFile("document.cfb");

// Create a new compound file (or truncate an existing one)
using var newCfb = MSCfbFileSystem.Storage.CreateFile("new.cfb");
newCfb.WriteAllFileText("README.txt", "Hello from MS-CFB");
```

`VfsReadOnlyLocation` and `VfsLocation` accept implicit conversions from `string` (paths on the host file system) or you can specify a custom `IFileSystemView` to open storage from a non-local backend.

## Usage

`Gapotchenko.FX.IO.FileSystems.MSCfb` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.IO.FileSystems.MSCfb):

```
dotnet package add Gapotchenko.FX.IO.FileSystems.MSCfb
```

## Related Modules

- [Gapotchenko.FX.IO.Vfs](../../../Gapotchenko.FX.IO.Vfs#readme) - Provides the virtual file system abstraction layer
- [Gapotchenko.FX.IO.FileSystems](../../Gapotchenko.FX.IO.FileSystems#readme) - Provides base functionality for file system implementations

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
  - [Gapotchenko.FX.Data.Archives](../../../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
    - [Gapotchenko.FX.Data.Archives.Zip](../../../../Data/Archives/Gapotchenko.FX.Data.Archives.Zip#readme)
  - [Gapotchenko.FX.Data.Encoding](../../../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../../../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum#readme)
- [Gapotchenko.FX.Diagnostics](../../../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../../Gapotchenko.FX.IO#readme)
  - [Gapotchenko.FX.IO.Vfs](../../../Gapotchenko.FX.IO.Vfs#readme)
  - [Gapotchenko.FX.IO.FileSystems](../../Gapotchenko.FX.IO.FileSystems#readme)
  - &#x27B4; [Gapotchenko.FX.IO.FileSystems.MSCfb](.#readme)
- [Gapotchenko.FX.Linq](../../../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../../../../..#readme).
