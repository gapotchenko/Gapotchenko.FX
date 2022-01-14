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
Vertices define "what" graph contains and edges define "how" those vertices are connected.

Let's take a look at the simplest graph that contains just two vertices:

``` c#
using Gapotchenko.FX.Math.Topology;

var g = new Graph<int>
{
    Vertices = { 1, 2 }
};
```

If we could visualize that graph then it would look like this:

![Simple graph with two isolated vertices](../../Documentation/Assets/Math/Topology/simple-graph-2-0.svg?raw=true)

Now let's add one more vertex `3` plus an edge from vertex `1` to vertex `2`:

``` c#
var g = new Graph<int>
{
    Vertices = { 1, 2, 3 },
    Edges = { (1, 2) }
};
```

Our new graph looks like this:

![Simple graph with three vertices and one edge](../../Documentation/Assets/Math/Topology/simple-graph-3-1.svg?raw=true)

The vertices already defined in edges can be omitted for brevity:

``` c#
var h = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

Console.WriteLine(g.GraphEquals(h)); // will print "True"
```

It is worth mentioning that the graph provides its vertices as an `ISet<T>`, so the usual operations on a set apply:

``` c#
var g = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

g.Vertices.UnionWith(new[] { 3, 4, 5 });
```

The example above produces the following graph:

![Simple graph with five vertices and one edge](../../Documentation/Assets/Math/Topology/simple-graph-5-1.svg?raw=true)


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
