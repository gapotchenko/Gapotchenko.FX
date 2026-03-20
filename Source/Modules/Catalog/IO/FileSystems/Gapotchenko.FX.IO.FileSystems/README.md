# Gapotchenko.FX.IO.FileSystems

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.IO.FileSystems.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.FileSystems)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.IO.FileSystems.svg)](https://www.nuget.org/packages/Gapotchenko.FX.IO.FileSystems)

The module provides the base infrastructure for mountable file system implementations that integrate with the virtual file system (VFS) abstraction layer.

## Overview

`Gapotchenko.FX.IO.FileSystems` module defines the core interfaces and base classes that enable mountable file system formats to be treated as virtual file systems. This allows file system implementations to work seamlessly with the VFS abstraction layer.

## IFileSystem

`IFileSystem` interface defines the contract for a mountable file system implementation. It extends `IVirtualFileSystem`, which means that any implementation can be used as a file system view:

``` C#
using Gapotchenko.FX.IO.FileSystems;
using Gapotchenko.FX.IO.Vfs;

void ProcessFileSystem(IFileSystem fs)
{
    // Since IFileSystem extends IVirtualFileSystem,
    // we can rely on a unified interface for all file system operations
    if (fs.FileExists("data.txt"))
    {
        string content = fs.ReadAllFileText("data.txt");
        // Process content...
    }
}
```

## Usage

`Gapotchenko.FX.IO.FileSystems` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.IO.FileSystems):

```
dotnet package add Gapotchenko.FX.IO.FileSystems
```

## Related Modules

- [Gapotchenko.FX.IO.Vfs](../../../IO/Gapotchenko.FX.IO.Vfs#readme) - Provides the virtual file system abstraction layer
- [Gapotchenko.FX.IO.FileSystems.MSCfb](../MSCfb/Gapotchenko.FX.IO.FileSystems.MSCfb#readme) - MS-CFB (Compound File Binary) file system implementation

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
  - [Gapotchenko.FX.Data.Archives](../../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
    - [Gapotchenko.FX.Data.Archives.Zip](../../../Data/Archives/Gapotchenko.FX.Data.Archives.Zip#readme)
  - [Gapotchenko.FX.Data.Encoding](../../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum#readme)
- [Gapotchenko.FX.Diagnostics](../../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
  - [Gapotchenko.FX.IO.Vfs](../../Gapotchenko.FX.IO.Vfs#readme)
  - &#x27B4; [Gapotchenko.FX.IO.FileSystems](.#readme)
    - [Gapotchenko.FX.IO.FileSystems.MSCfb](../MSCfb/Gapotchenko.FX.IO.FileSystems.MSCfb#readme)
- [Gapotchenko.FX.Linq](../../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../../..#readme).
