# Gapotchenko.FX.Runtime.CompilerServices.Intrinsics

<!--
<docmeta>
	<complexity>expert</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics)

The module allows to define and compile intrinsic functions using machine code.
They can be used in hardware-accelerated implementations of performance-sensitive algorithms.

## Example

Suppose we are trying to fix the performance bottleneck in the following algorithm:

``` C#
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

``` C#
class BitOperations
{
    // "Bit Twiddling Hacks" by Sean Eron Anderson:
    // http://graphics.stanford.edu/~seander/bithacks.html

    public static int Log2_DeBruijn(uint value)
    {
        // Round down to one less than a power of 2.
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        uint index = (value * 0x07c4acddU) >> 27;
        return m_Log2DeBruijn32[index];
    }

    static readonly int[] m_Log2DeBruijn32 =
    [
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    ];
}
```

Here are the execution times of all two implementations (lower is better):

|         Method |     Mean |     Error |    StdDev |
|--------------- |---------:|----------:|----------:|
|   Log2_Trivial | 4.587 ns | 0.0325 ns | 0.0288 ns |
|  Log2_DeBruijn | 1.256 ns | 0.0068 ns | 0.0063 ns |

This is a vast improvement over the previous version but we can do even better.

Meet the Intel 80386, a 32-bit microprocessor introduced in 1985.
It brought the Bit Scan Reverse (BSR) instruction that does exactly the same what we want to achieve by `Log2` using just a small fraction of CPU cycles.

Chances are that your machine runs on a descendant of that influential CPU, be it AMD Ryzen or Intel Core.
So how can we use low-level CPU instructions from high-level .NET?

This is why `Gapotchenko.FX.Runtime.CompilerServices.Intrinsics` class exists.
It allows you to define intrinsic implementations of a method
using machine code tailored to a particular processor architecture.
Let's see how:

``` C#
using Gapotchenko.FX.Runtime.CompilerServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class BitOperations
{
    static BitOperations()
    {
        // Ensure that intrinsic methods are initialized (compiled) before they can be used.
        Intrinsics.InitializeType(typeof(BitOperations));
    }

    // Define machine code intrinsics for the method
    [MachineCodeIntrinsic(
        Architecture.X86,
        // The 0 -> 0 contract is fulfilled by setting the LSB to 1.
        // Log2(1) is 0, and setting the LSB for values > 1 does not change the log2 result.
        0x83, 0xc9, 0x01,  // OR ECX,1
        0x0f, 0xbd, 0xc1,  // BSR EAX,ECX
        AdditionalArchitectures = [Architecture.X64],
        SupportedOSPlatforms = ["windows"])]
    [MachineCodeIntrinsic(
        Architecture.X86,
        0x83, 0xc9, 0x01,        // OR ECX,1
        0xf3, 0x0f, 0xbd, 0xc1,  // LZCNT EAX,ECX
        0x83, 0xf0, 0x1f,        // XOR EAX,31
        AdditionalArchitectures = [Architecture.X64],
        RequiredFeatures = [MachineCodeIntrinsicFeature.Lzcnt],
        SupportedOSPlatforms = ["windows"],
        Priority = 10)]         // LZCNT is faster than BSR on AMD processors
    [MachineCodeIntrinsic(
        Architecture.Arm,
        0x01, 0x21,              // MOVS R1,1
        0x08, 0x43,              // ORRS R0,R1
        0xb0, 0xfa, 0x80, 0xf0,  // CLZ R0,R0
        0x1f, 0x21,              // MOVS R1,31
        0x48, 0x40,              // EORS R0,R1
        SupportedOSPlatforms = ["linux"])]
    [MachineCodeIntrinsic(
        Architecture.Arm64,
        0x00, 0x00, 0x00, 0x32,   // ORR W0,W0,#1
        0x00, 0x10, 0xc0, 0x5a,   // CLZ W0,W0
        0x00, 0x10, 0x00, 0x52,   // EOR W0,W0,#31
        SupportedOSPlatforms = ["windows"])]
    [MethodImpl(Intrinsics.MethodImplOptions)]
    public static int Log2_Intrinsic(uint value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        uint index = (value * 0x07c4acddU) >> 27;
        return m_Log2DeBruijn32[index];
    }

    static readonly int[] m_Log2DeBruijn32 =
    [
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    ];
}
```

`Log2_Intrinsic` method defines a custom attribute that provides a machine code for `BSR EAX, ECX` instruction.
Machine code is tied to CPU architecture and calling conventions of an operating system,
and these requirements are reflected in the attributes as well.

Please note that besides using `MachineCodeIntrinsicAttribute` to define method intrinsic implementations,
`BitOperations` class **should** use a static constructor to ensure that the corresponding methods are initialized (compiled) before they are called.

Here are the execution times of all three implementations benchmarked on a Windows x64 system in Release configuration (lower is better):

|         Method |     Mean |     Error |    StdDev |
|--------------- |---------:|----------:|----------:|
|   Log2_Trivial | 4.587 ns | 0.0325 ns | 0.0288 ns |
|  Log2_DeBruijn | 1.256 ns | 0.0068 ns | 0.0063 ns |
| Log2_Intrinsic | 1.038 ns | 0.0660 ns | 0.0947 ns |

`Log2_Intrinsic` is a clear winner.

For the given example, ARM64 benchmark has similar outcomes.

The intrinsic compiler may or may not apply machine code to a method depending on the current app host environment.
When intrinsic is not applied, the original method implementation is used, thus providing a graceful, albeit less performant, fallback.

## Supported Architectures

The intrinsic compiler supports the following processor architecture and operating system combinations:

| Processor Architecture | Windows | Linux | macOS |
|------------------------|:-------:|:-----:|:-----:|
| x86                    | ✓       | ✓     |       |
| x64                    | ✓       | ✓     | ⧈     |
| ARM (32-bit)           |         | ◇     |       |
| ARM64                  | ✓       | ✓     | ⧈     |

✓ Intrinsic compilation is supported, including Native AOT
<br>
⧈ Intrinsic compilation is supported, except in Native AOT due to restrictions imposed by OS on self-modifiable code
<br>
◇ Processor feature detection is supported, but intrinsic compilation is not available

An individual intrinsic is applied only when it provides machine code for a corresponding architecture and its other requirements
such as processor features and operating system constraints are satisfied.
When a combination or requirement is unsupported, or operating environment does not support intrinsic compilation, the original managed method implementation remains in use.

## Native AOT Compatibility

The methods defined using intrinsic machine code are fully compatible with native ahead-of-time compilation provided by .NET.

When intrinsic initialization is triggered by the first direct call to a method in a Native AOT application,
that call may continue executing the original ahead-of-time compiled implementation even though the method entry point is patched during type initialization.
Subsequent calls execute the intrinsic machine code through the patched entry point.

This behavior does not affect correctness because the original method implementation is the semantic fallback for the intrinsic.
It only means that the managed implementation may be used for the first call; subsequent calls receive the intrinsic performance benefit.

## Diagnostics

The intrinsic compiler reports diagnostics through the
[`System.Diagnostics.TraceSource`](https://learn.microsoft.com/dotnet/api/system.diagnostics.tracesource) named
`Gapotchenko.FX.Runtime.CompilerServices.Intrinsics`.
The source can be accessed directly through `Intrinsics.TraceSource` that has `SourceLevels.Error` diagnostic level set by default.

The diagnostic levels have the following meanings:

- `Verbose` explains why the intrinsic compiler is unavailable for the current environment or architecture.
- `Information` reports successful intrinsic compilation.
- `Warning` reports that an individual intrinsic was discarded and its managed implementation remains in use.
- `Error` reports a hard failure that prevents further intrinsic compilation in the current environment.

For example, the following code writes all intrinsic compiler diagnostics to the console:

``` C#
using Gapotchenko.FX.Runtime.CompilerServices;
using System.Diagnostics;

var traceSource = Intrinsics.TraceSource;
traceSource.Switch.Level = SourceLevels.Verbose;
traceSource.Listeners.Add(new ConsoleTraceListener());
```

Configure the trace source before calling `Intrinsics.InitializeType` or otherwise triggering initialization of a type
that contains intrinsic methods. This ensures that diagnostics emitted during intrinsic compilation are captured.

On .NET Framework, the trace source can alternatively be configured in the application configuration file:

``` XML
<configuration>
  <system.diagnostics>
    <sources>
      <source name="Gapotchenko.FX.Runtime.CompilerServices.Intrinsics" switchValue="Verbose">
        <listeners>
          <add name="console" type="System.Diagnostics.ConsoleTraceListener" />
        </listeners>
      </source>
    </sources>
  </system.diagnostics>
</configuration>
```

On .NET 7.0+, a similar effect can be achieved by utilizing `System.Diagnostics.TraceSource.Initializing` property.

## Usage

`Gapotchenko.FX.Runtime.CompilerServices.Intrinsics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Runtime.CompilerServices.Intrinsics):

```
dotnet package add Gapotchenko.FX.Runtime.CompilerServices.Intrinsics
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
- &#x27B4; [Gapotchenko.FX.Runtime.CompilerServices.Intrinsics](.#readme) ✱✱
- [Gapotchenko.FX.Runtime.InteropServices](../Gapotchenko.FX.Runtime.InteropServices#readme) ✱
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Symbol ✱ denotes an advanced module.  
Symbol ✱✱ denotes an expert module.

Or take a look at the [full list of modules](../../..#readme).
