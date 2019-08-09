# Gapotchenko.FX.Drawing

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Drawing.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Drawing)

`Gapotchenko.FX.Drawing` is a complementary module to conventional `System.Drawing` library provided by .NET platform.

The module provides extended primitives covering the advanced tasks related to drawing.

## ThemeColors

`Gapotchenko.FX.Drawing.ThemeColors` class provides access to theme colors of a host operating system.

The difference between `ThemeColors` and `SystemColors` is that theme colors are **the real colors** displayed on screen,
while `SystemColors` provided by the stock `System.Drawing` module are at most just a compatible approximation stuck in the constraints of the past.

## ColorMetrics

`Gapotchenko.FX.Drawing.ColorMetrics` class provides color metrics functions.

### Similarity

`double ColorMetrics.Similarity(Color a, Color b)` function calculates perceptual similarity of two colors.

The returned value lays between `0.0` and `1.0` inclusively.
When specified colors are equal, the returned similarity factor is `1.0`.
When specified colors are completely different (like black and white), the returned similarity factor is `0.0`.

Specifics of a human visual perception are taken into account in accordance to the following standards and approaches:
ISO-9241-3, ANSI-HFES-100-1988, CIELUV.

The cool thing about this function is that it allows to choose the "best" color among several alternatives.

## Usage

`Gapotchenko.FX.Drawing` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Drawing):

```
PM> Install-Package Gapotchenko.FX.Drawing
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Data.Linq](../Gapotchenko.FX.Data.Linq) ✱
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- &#x27B4; [Gapotchenko.FX.Drawing](../Gapotchenko.FX.Drawing) ✱
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Numerics](../Gapotchenko.FX.Numerics) ✱
- [Gapotchenko.FX.Reflection.Loader](../Gapotchenko.FX.Reflection.Loader) ✱
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](..#available-modules).
