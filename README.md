# Gapotchenko.FX

[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.svg)](https://www.nuget.org/packages/Gapotchenko.FX)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.svg)](https://www.nuget.org/packages/Gapotchenko.FX)

.NET polyfill to the future. A versatile RAD (Rapid Application Development) framework for .NET platform.

## Overview

Gapotchenko.FX closes the gaps in original .NET design by providing the missing functionality that should have been mainstream since long ago.
Here is a short list of what Gapotchenko.FX can do:

  - **Use the latest .NET features, even with older target frameworks.**
    Gapotchenko.FX achieves this by providing a set of built-in polyfills
  - **Use formalized primitives to solve a task at hand, without reinventing the boilerplate.**
    For example, `Gapotchenko.FX.Math.Graphs.Graph<T>` allows you to solve a plethora of seemingly hard tasks in a simple and creative way you may never have known about
  - **Benefit from the power of formal mathematics.**
    While some people think that programming is somehow different from the math, actually it is not that different.
    For example, when a software component has an inherent flaw, it shows up way further down the lane, guaranteed.
    Formal math makes this weak thinking impossible, and all the features provided by Gapotchenko.FX rigorously follow that strategy.
    For example, `Gapotchenko.FX.Collections.Generic.AssociativeArray<TKey, TValue>` represents a collection of keys and values covering the full value space of `TKey` type,
    while `System.Collections.Generic.Dictionary<TKey, TValue>` handles only an opinionated subset by failing to contain `null` `TKey` values
  - **Better utilize the hardware and software capabilities of a host environment.**
    Many Gapotchenko.FX primitives are backed up by algorithms leveraging OS and hardware acceleration.
    For example, CRC-32C checksum algorithm provided by `Gapotchenko.FX.Data.Integrity.Checksum.Crc32` module leverages a special CPU instruction that does fast CRC-32C calculation in hardware, whenever possible
    
All in all, you can consider Gapotchenko.FX to play a similar role in .NET ecosystem as Boost plays for C++. The project philosophical aim is to avoid a situation like that:

![.NET Progress ca. 2012 - 2018](Documentation/Assets/dotnet-progress-ca-2012-2018.png?raw=true ".NET Progress ca. 2012 - 2018")

## Modules

Gapotchenko.FX consists of granular modules that form functional hierarchies.

[Get started with Gapotchenko.FX modules >](Source/Modules#readme)

## Profiles

In addition to modules, Gapotchenko.FX also provides profiles.
A profile is a collection of Gapotchenko.FX modules covering a common usage pattern.

[Get started with Gapotchenko.FX profiles >](Source/Profiles#readme)

## Supported Platforms

Gapotchenko.FX is largely platform-independent, but some parts of the implementation require specific integration with a host environment to achieve the best characteristics.

- Supported target frameworks:
  - .NET 5.0+
  - .NET Core 2.0+
  - .NET Standard 2.0+
  - .NET Framework 4.6.1+

- Supported operating systems: 
  - Linux
  - macOS
  - Windows

- Supported CPU architectures: x86, x64, ARM, ARM64

When Gapotchenko.FX runs on an unsupported platform, the platform-dependent functionality is automatically downgraded to the next best thing — a less optimized generic implementation.
On rare occasions, `System.PlatformNotSupported` exceptions may be raised by the functionality that fully depends on a platform to deliver the correct results.
