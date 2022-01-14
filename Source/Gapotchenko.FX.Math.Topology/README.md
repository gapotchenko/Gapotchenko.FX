# Gapotchenko.FX.Math.Topology

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Topology.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Topology)

The module provides data structures and primitives for working with abstract topologies.

In its classical meaning, the science of topology is concerned with the properties of geometric objects.
However, such a definition may be considered as too narrow by some.
Math has no boundaries,
so topology should not be viewed as limited to just one unique flavor of objects (geometric, physical); it can really be anything.

## Graph&lt;T&gt;

`Graph<T>` provided by `Gapotchenko.FX.Math.Topology` represents a strongly-typed directional graph of objects.
The objects correspond to mathematical abstractions called graph vertices and each of the related pairs of vertices is called an edge.
A graph can be viewed as a structure that contains two sets: set of vertices and set of edges.
Vertices define "what" and edges define "how" it is connected.

Let's take a look at the simplest graph that contains just two vertices:

``` c#
using Gapotchenko.FX.Math.Topology;

var g = new Graph<int>
{
    Vertices = { 1, 2 }
};
```

If we could visualize that graph then it would look like this:

<svg width="134pt" height="44pt"
 viewBox="0.00 0.00 134.00 44.00" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
<g id="graph0" class="graph" transform="scale(1 1) rotate(0) translate(4 40)">
<title>G</title>
<polygon fill="#ffffff" stroke="transparent" points="-4,4 -4,-40 130,-40 130,4 -4,4"/>
<!-- 1 -->
<g id="node1" class="node">
<title>1</title>
<ellipse fill="none" stroke="#000000" cx="27" cy="-18" rx="27" ry="18"/>
<text text-anchor="middle" x="27" y="-13.8" font-family="Times,serif" font-size="14.00" fill="#000000">1</text>
</g>
<!-- 2 -->
<g id="node2" class="node">
<title>2</title>
<ellipse fill="none" stroke="#000000" cx="99" cy="-18" rx="27" ry="18"/>
<text text-anchor="middle" x="99" y="-13.8" font-family="Times,serif" font-size="14.00" fill="#000000">2</text>
</g>
</g>
</svg>


## Usage

`Gapotchenko.FX.Math.Topology` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Topology):

```
PM> Install-Package Gapotchenko.FX.Math.Topology
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - &#x27B4; [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
