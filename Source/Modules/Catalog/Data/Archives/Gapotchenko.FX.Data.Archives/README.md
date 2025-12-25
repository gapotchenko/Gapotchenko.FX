# Gapotchenko.FX.Data.Archives

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Archives.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Archives)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Data.Archives.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Archives)

The module provides the base infrastructure for data archives that integrate with the virtual file system (VFS) abstraction layer.

## Overview

`Gapotchenko.FX.Data.Archives` module defines the core interfaces and base classes that enable archive formats to be treated as virtual file systems. This allows archive implementations to work seamlessly with the VFS abstraction layer.

## IDataArchive

`IDataArchive` interface defines the contract for a data archive. It extends `IVirtualFileSystem`, which means any data archive can be used as a file system view:

``` C#
using Gapotchenko.FX.Data.Archives;
using Gapotchenko.FX.IO.Vfs;

void ProcessArchive(IDataArchive archive)
{
    // Since IDataArchive extends IVirtualFileSystem,
    // we can use it as a file system view
    if (archive.FileExists("data.txt"))
    {
        string content = archive.ReadAllText("data.txt");
        // Process content...
    }
}
```

## Usage

`Gapotchenko.FX.Data.Archives` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Archives):

```
dotnet package add Gapotchenko.FX.Data.Archives
```

## Related Modules

- [Gapotchenko.FX.IO.Vfs](../../IO/Gapotchenko.FX.IO.Vfs#readme) - Provides the virtual file system abstraction layer
- [Gapotchenko.FX.Data.Archives.Zip](../Gapotchenko.FX.Data.Archives.Zip#readme) - Provides ZIP archive implementation

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](#)
  - &#x27B4; [Gapotchenko.FX.Data.Archives](.#readme)
    - [Gapotchenko.FX.Data.Archives.Zip](../Gapotchenko.FX.Data.Archives.Zip#readme)
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
