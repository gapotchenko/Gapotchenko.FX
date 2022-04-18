# Gapotchenko.FX.Runtime.CompilerServices.Intrinsics

<!--
<docmeta>
	<complexity>expert</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics)

The module allows to define and compile intrinsic functions.
They can be used in hardware-accelerated implementations of various performance-sensitive algorithms.

## Example

Suppose we are trying to fix the performance bottleneck in the following algorithm:

``` csharp
class BitOperations
{
    // Returns the base 2 logarithm of a specified number.
    public static int Log2_Trivial(uint value)
    {
        int r = 0;
        while ((value >>= 1) != 0)
            ++r;
        return r;
    }
}
```

log<sub>2</sub> seems to be a trivial operation but it often becomes a serious bottleneck in path-finding or cryptographic algorithms.
We can do better here if we switch to a table lookup:

``` csharp
class BitOperations
{
    // "Bit Twiddling Hacks" by Sean Eron Anderson:
    // http://graphics.stanford.edu/~seander/bithacks.html

    static readonly int[] m_Log2DeBruijn32 =
    {
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    };

    public static int Log2_DeBruijn(uint value)
    {
        // Round down to one less than a power of 2.
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        var index = (value * 0x07C4ACDDU) >> 27;
        return m_Log2DeBruijn32[index];
    }
}
```

Here are the execution times of all two implementations (lower is better):

|         Method |     Mean |     Error |    StdDev |
|--------------- |---------:|----------:|----------:|
|   Log2_Trivial | 4.587 ns | 0.0325 ns | 0.0288 ns |
|  Log2_DeBruijn | 1.256 ns | 0.0068 ns | 0.0063 ns |

This is a vast improvement over the previous version but we can do even better.

Meet the Intel 80386, a 32-bit microprocessor introduced in 1985.
It brought the Bit Scan Reverse (BSR) instruction that does exactly the same what we want to achieve by `Log2` using just a small fraction of cycles.

Chances are your machine runs on a descendant of that influential CPU, be it AMD Ryzen or Intel Core.
So how can we use the `BSR` instruction from .NET?

This is why `Gapotchenko.FX.Runtime.CompilerServices.Intrinsics` class exists.
It provides an ability to define intrinsic implementation of a method with `MachineCodeIntrinsicAttribute`. Let's see how:

``` csharp
using Gapotchenko.FX.Runtime.CompilerServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class BitOperations
{
    // Use static constructor to ensure that intrinsic methods are initialized (compiled) before they can be used
    static BitOperations() => Intrinsics.InitializeType(typeof(BitOperations));

    static readonly int[] m_Log2DeBruijn32 =
    {
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    };

    // Define machine code intrinsic for the method
    [MachineCodeIntrinsic(Architecture.X64, 0x0f, 0xbd, 0xc1)]  // BSR EAX, ECX
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Log2_Intrinsic(uint value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        var index = (value * 0x07C4ACDDU) >> 27;
        return m_Log2DeBruijn32[index];
    }
}
```

`Log2_Intrinsic` method defines a custom attribute that provides a machine code for `BSR EAX, ECX` instruction.
Machine code is tied to CPU architecture and this is reflected in the attribute as well.

Please note that besides using `MachineCodeIntrinsicAttribute` to define method intrinsic implementations,
`BitOperations` class **should** use a static constructor to ensure that corresponding methods are initialized (compiled) before they are called.

Here are the execution times of all three implementations (lower is better):

|         Method |     Mean |     Error |    StdDev |
|--------------- |---------:|----------:|----------:|
|   Log2_Trivial | 4.587 ns | 0.0325 ns | 0.0288 ns |
|  Log2_DeBruijn | 1.256 ns | 0.0068 ns | 0.0063 ns |
| Log2_Intrinsic | 1.038 ns | 0.0660 ns | 0.0947 ns |

`Log2_Intrinsic` is a clear winner.

Intrinsic compiler may or may not apply machine code to a method depending on current app host environment.
When intrinsic is not applied, the original method implementation is used, thus providing a graceful, albeit less performant, fallback.

## Usage

`Gapotchenko.FX.Runtime.CompilerServices.Intrinsics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics):

```
PM> Install-Package Gapotchenko.FX.Runtime.CompilerServices.Intrinsics
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data.Integrity.Checksum](../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Data.Linq](../Data/Gapotchenko.FX.Data.Linq) ✱
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Numerics](../Gapotchenko.FX.Numerics) ✱
- [Gapotchenko.FX.Reflection.Loader](../Gapotchenko.FX.Reflection.Loader) ✱
- &#x27B4; [Gapotchenko.FX.Runtime.CompilerServices.Intrinsics](../Gapotchenko.FX.Runtime.CompilerServices.Intrinsics) ✱✱
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Symbol ✱ denotes an advanced module.  
Symbol ✱✱ denotes an expert module.

Or take a look at the [full list of modules](..#available-modules).
