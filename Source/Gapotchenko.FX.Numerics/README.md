﻿# Gapotchenko.FX.Numerics

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Numerics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Numerics)

The module provides hardware-accelerated operations for numeric data types.

## BitOperations

`BitOperations` class provides bit manipulation operations for unsigned integer values of 32 and 64 bit lengths.

The main consumers of hardware-accelerated bit operations are path-finding and cryptographic algorithms.

`BitOperations` class is a polyfill to the future as it first appeared in .NET Core 3.0.


### Log2

Calculates the integer base 2 logarithm of a specified number.
By convention, `Log2(0)` returns `0` since such operation is undefined.

The behavior corresponds to `BSR` instruction from Intel x86 instruction set.

### PopCount

Calculates the bit population count for a specified value.
The result corresponds to the number of bits set to `1`.

The behavior corresponds to `POPCNT` instruction from Intel x86 instruction set.

### RotateLeft

Rotates the specified value left by the specified number of bits.

The behavior corresponds to `ROL` instruction from Intel x86 instruction set.

### RotateRight

Rotates the specified value right by the specified number of bits.

The behavior corresponds to `ROR` instruction from Intel x86 instruction set.

## BitOperationsEx

`BitOperationsEx` provides an extended set of bit-twidling operations that are not in .NET mainstream yet.

### Reverse

Reverses the order of bits in a specified value.
The least significant bit gets swapped with the most significant bit, and so on for all remaining bits of the number.
For example:

```c#
BitOperationsEx.Reverse((byte)0b10100001) = 0b10000101
```

## Hardware Acceleration

`Gapothenko.FX.Numerics` automatically employs hardware acceleration on conforming CPUs and architectures.
If hardware acceleration is not available for a particular operation then a highly-optimized software fallback implementation is used instead.

## Usage

`Gapotchenko.FX.Numerics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Numerics):

```
PM> Install-Package Gapotchenko.FX.Numerics
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- &#x27B4; [Gapotchenko.FX.Numerics](../Gapotchenko.FX.Numerics) ✱
- [Gapotchenko.FX.Reflection.Loader](../Gapotchenko.FX.Reflection.Loader) ✱
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](..#available-modules).
