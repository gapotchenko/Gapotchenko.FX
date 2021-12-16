# Gapotchenko.FX.Math.Topology.Serialization

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Topology.Serialization.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Topology.Serialization)

The module provides tools for converting between Gapotchenko.FX graph and DOT graph description language.

## DotSerializer

`DotSerializer` class provided by `Gapotchenko.FX.Math.Topology.Serialization` module allows you to serialize a graph to DOT language string and deserialize it back.

Consider a simple serialization example:

```csharp
var graph = new Graph<string>();
graph.Edges.Add("from", "to");

var dotSerializer = new DotSerializer();
var dot = dotSerializer.Serialize(graph);

Console.WriteLine(dot);
```

And now deserialize some graph:

```csharp
var dot = "digraph { 1 -> 2 }";

var graph = new Graph<int>();

var dotSerializer = new DotSerializer();
dotSerializer.Deserialize(graph, dot);

Console.WriteLine(graph.Edges.Contains(1, 2));
```

By default, for vertices conversion, `DotSerializer` uses `System.Convert` or a registered `System.ComponentModel.TypeConverter` if any. For more control over how a vertex is serialized and deserialized, you can override `SerializeVertex` and `DeserializeVertex` methods of `DotSerializer` class.

## Usage

`Gapotchenko.FX.Math.Topology.Serialization` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Topology.Serialization):

```
PM> Install-Package Gapotchenko.FX.Math.Topology.Serialization
```