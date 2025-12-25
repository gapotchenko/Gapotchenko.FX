# Gapotchenko.FX.Numerics

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Numerics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Numerics)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Numerics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Numerics)

The module provides hardware-accelerated operations for numeric data types.

## BitOperations Class

`BitOperations` class provides bit manipulation operations for unsigned integer values of 32 and 64 bit lengths.
The main consumers of hardware-accelerated bit operations are path-finding and cryptographic algorithms.

`BitOperations` class is a polyfill to the future as it first appeared in .NET Core 3.0.

### Log2

`BitOperations.Log2` method calculates the integer base 2 logarithm of a specified number.
`Log2(0)` returns an undefined value since such operation is undefined.

The behavior corresponds to `BSR` instruction from Intel x86 instruction set.

### PopCount

`BitOperations.PopCount` method calculates the bit population count for a specified value.
The result corresponds to the number of bits set to `1`.

The behavior corresponds to `POPCNT` instruction from Intel x86 instruction set.

### RotateLeft

`BitOperations.RotateLeft` method rotates the specified value left by the specified number of bits.

The behavior corresponds to `ROL` instruction from Intel x86 instruction set.

### RotateRight

`BitOperations.RotateRight` method rotates the specified value right by the specified number of bits.

The behavior corresponds to `ROR` instruction from Intel x86 instruction set.

## BitOperationsEx Class

`BitOperationsEx` class provides an extended set of bit-twidling operations that are not in mainstream .NET yet.

### Reverse

`BitOperationsEx.Reverse` method reverses the order of bits in a specified value and returns a result.
The least significant bit gets swapped with the most significant bit, and so on for all remaining bits of the number.
For example:

``` C#
BitOperationsEx.Reverse((byte)0b10100001) = 0b10000101
```

## Hardware Acceleration

`Gapothenko.FX.Numerics` automatically employs hardware acceleration on conforming CPUs and architectures.
If hardware acceleration is not available for a particular operation then a highly-optimized software fallback implementation is used instead.

## Usage

`Gapotchenko.FX.Numerics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Numerics):

```
dotnet package add Gapotchenko.FX.Numerics
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Data/Archives/Gapotchenko.FX.Data.Archives#readme)
- [Gapotchenko.FX.Diagnostics](../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory#readme)
- &#x27B4; [Gapotchenko.FX.Numerics](.#readme) ✱
- [Gapotchenko.FX.Reflection.Loader](../Reflection/Gapotchenko.FX.Reflection.Loader#readme) ✱
- [Gapotchenko.FX.Runtime.InteropServices](../Runtime/Gapotchenko.FX.Runtime.InteropServices#readme) ✱
- [Gapotchenko.FX.Security.Cryptography](../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../Versioning/Gapotchenko.FX.Versioning#readme)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](../..#readme).
