# Gapotchenko.FX.Runtime.InteropServices

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Runtime.InteropServices.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Runtime.InteropServices)

`Gapotchenko.FX.Runtime.InteropServices` is a complementary module to `System.Runtime.InteropServices` which is provided as a part of .NET.
The module provides polyfills for functionality defined in `System.Runtime.InteropServices` BCL module.

## Polyfills for `SafeBuffer`

`SafeBuffer` class provided by `System.Runtime.InteropServices` module got a new `ReadSpan(ulong byteOffset, Span<byte> buffer)` method since .NET 6.0.
`Gapotchenko.FX.Runtime.InteropServices` module makes this method available for all other supported target frameworks.

## Usage

`Gapotchenko.FX.Runtime.InteropServices` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Runtime.InteropServices):

```
dotnet package add Gapotchenko.FX.Runtime.InteropServices
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Numerics](../../Gapotchenko.FX.Numerics#readme) ✱
- [Gapotchenko.FX.Reflection.Loader](../../Reflection/Gapotchenko.FX.Reflection.Loader#readme) ✱
- &#x27B4; [Gapotchenko.FX.Runtime.InteropServices](.#readme) ✱
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](../../..#readme).
