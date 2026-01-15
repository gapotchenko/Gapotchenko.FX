# Gapotchenko.FX.Math.Graphs

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Graphs.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Graphs)

The module provides data structures and primitives for working with abstract graphs of objects.

## `Graph<T>`

`Graph<T>` class provided by `Gapotchenko.FX.Math.Graphs` represents a strongly-typed graph of objects.
The objects stored in a graph are called vertices and each of the related pairs of vertices is called an edge.
A graph can be viewed as a structure containing two sets: the set of vertices and the set of edges.
Vertices define "what" graph contains and edges define "how" those vertices are interconnected.

Let's take a look at the simplest directional graph that contains just two vertices:

``` C#
using Gapotchenko.FX.Math.Graphs;

var g = new Graph<int>
{
    Vertices = { 1, 2 }
};
```

If we could visualize that graph then it would look like this:

![Simple graph with two isolated vertices](../../../../../Documentation/Assets/Math/Graphs/simple-graph-2-0.svg?raw=true)

Now let's add one more vertex `3` plus an edge that goes from vertex `1` to vertex `2`:

``` C#
var g = new Graph<int>
{
    Vertices = { 1, 2, 3 },
    Edges = { (1, 2) }  // <-- an edge has (from, to) notation
};
```

Our new graph looks like this:

![Simple graph with three vertices and one edge](../../../../../Documentation/Assets/Math/Graphs/simple-graph-3-1.svg?raw=true)

The vertices already defined in edges can be omitted for brevity:

``` C#
var h = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

Console.WriteLine(g.GraphEquals(h)); // will print "True"
```

It is worth mentioning that the graph provides its vertices as an `ISet<T>`, so the usual operations on a set apply to the vertices as well:

``` C#
var g = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

g.Vertices.UnionWith([3, 4, 5]);
```

The example above produces the following graph:

![Simple graph with five vertices and one edge](../../../../../Documentation/Assets/Math/Graphs/simple-graph-5-1.svg?raw=true)

The same `ISet<T>` model applies to the graph edges: they are treated as a set too.

### Operations

Now once we have the basics in place, let's take a look at graph operations.
Consider the graph:

``` C#
var g = new Graph<int>
{
    Edges =
    {
        (7, 5), (7, 6),
        (6, 3), (6, 4),
        (5, 2), (5, 4),
        (3, 1),
        (2, 1),
        (1, 0)
    }
};
```

which looks like this:

![Graph with eight vertices and nine edges](../../../../../Documentation/Assets/Math/Graphs/graph-8-9.svg?raw=true)

Let's transpose the graph (i.e. reverse the direction of its edges):

``` C#
var h = g.GetTransposition();
```

Transposed graph `h` renders as:

![Transposed graph with eight vertices and nine edges](../../../../../Documentation/Assets/Math/Graphs/graph-8-9-t.svg?raw=true)

Note that graph `h` is a new instance of `Graph<T>` that was created from graph `g` by reversing its edges.
But what if we want to transpose the graph `g` in place?
Every graph operation has a corresponding in-place variant, so for the transposition it will be:

``` C#
g.Transpose();
```

In this way, a developer can freely choose between immutable, mutable, or combined data models when working on a particular task at hand.

Graph transposition is just one example but there are plenty of other operations available.
They all work in the same manner and follow the same mutable/immutable model:

| Operation | Description | Immutable Function | In-Place Method |
| --- | --- | --- | --- |
| Transposition | Reverses the direction of all edges in the graph. | `GetTransposition` | `Transpose` |
| Transitive reduction | Prunes the transitive relations that have shorter paths. | `GetTransitiveReduction` | `ReduceTransitions` |
| Reflexive reduction | Removes the loops (also called self-loops or buckles). | `GetReflexiveReduction` | `ReduceReflexes` |
| Subgraph | Produces a vertex-induced or edge-induced subgraph. | `GetSubgraph` | `Subgraph` |
| Intersection | Produces a graph containing vertices and edges that are present in both the current and a specified graphs. | `Intersect` | `IntersectWith` |
| Union | Produces a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both. | `Union` | `UnionWith` |
| Exception | Produces a  graph containing vertices and edges that are present in the current graph but not in the specified graph. | `Except` | `ExceptWith` |

### Topological Sorting

Topological sort of a graph is a linear ordering of its vertices such that
for every directed edge `u` → `v`,
`u` comes before `v` in the ordering.

A classic example of topological sorting is to schedule a sequence of jobs (or tasks) according to their dependencies.
The jobs are represented by vertices, and there is an edge from `x` to `y` if job `x` must be completed before job `y` can be started.
Performing topological sort on such a graph would give us an order in which to perform the jobs.

Let's take a look at example graph:

![Graph with eight vertices and nine edges](../../../../../Documentation/Assets/Math/Graphs/graph-8-9.svg?raw=true)

Let's assume that vertices represent the jobs, and edges define the dependencies between them.
In this way, job `0` depends on job `1` and thus cannot be started unless job `1` is finished.
In turn, job `1` cannot be started unless jobs `2` and `3` are finished. And so on.
In what order should the jobs be executed?

To answer that question, let's use `OrderTopologically` method:

``` C#
using Gapotchenko.FX.Math.Graphs;

// Define a graph according to the diagram.
var g = new Graph<int>
{
    Edges =
    {
        (7, 5), (7, 6),
        (6, 3), (6, 4),
        (5, 2), (5, 4),
        (3, 1),
        (2, 1),
        (1, 0)
    }
};

// Sort the graph topologically.
var ordering = g.OrderTopologically();

// Print the results.
Console.WriteLine(string.Join(", ", ordering));
```

The resulting sequence of jobs is:

```
7, 5, 6, 2, 3, 4, 1, 0
```

`OrderTopologically` method can only work on directed acyclic graphs.
If the graph contains a cycle then `GraphCircularReferenceException` is raised.

### Stable Topological Sort of a Graph

Graph is a data structure similar to a set: it does not guarantee to preserve the order in which the elements were added.
As a result, topological sorting may return different orderings for otherwise equal graphs.

To overcome that limitation, it may be beneficial to use the topological sorting with a subsequent ordering by some other criteria.
Such approach makes the topological sorting stable.
It can be achieved by leveraging the standard `IOrderedEnumerable<T>` LINQ semantics of the operation, like so:

``` C#
g.OrderTopologically().ThenBy(…)
```

### Stable Topological Sort of a Sequence

Sorting a sequence of elements in topological order is another play on the topological sorting idea.

Say we have a sequence of elements `[A, B, C, D, E, F]`. Some elements depend on others:

- A depends on B
- B depends on D

Objective: sort the sequence so that its elements are ordered according to their dependencies.
The resulting sequence should have a minimal edit distance to the original one.
In other words, sequence should be topologically sorted while preserving the original order of elements whenever it is possible.

`Gapotchenko.FX.Math.Graphs` module provides an extension method for `IEnumerable<T>` that allows to achieve that:

``` C#
using Gapotchenko.FX.Math.Graphs;

string seq = "ABCDEF";

// Dependency function.
static bool df(char a, char b) =>
    (a + " depends on " + b) switch
    {
        "A depends on B" or
        "B depends on D" => true,
        _ => false
    };

var ordering = seq.OrderTopologicallyBy(x => x, df);
Console.WriteLine(string.Join(", ", ordering));  // <- prints "D, B, A, C, E, F"
```

Unlike its graph sibling, `IEnumerable<T>.OrderTopologicallyBy` extension method tolerates circular dependencies by ignoring them.
They are resolved according to the original order of elements in the sequence.

`OrderTopologicallyBy` method allows a subsequent sorting by following the standard `IOrderedEnumerable<T>` LINQ convention:

``` C#
seq.OrderTopologicallyBy(…).ThenBy(…)
```

## Usage

`Gapotchenko.FX.Math.Graphs` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Graphs):

```
dotnet package add Gapotchenko.FX.Math.Graphs
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
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math#readme)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics#readme)
  - &#x27B4; [Gapotchenko.FX.Math.Graphs](.#readme)
  - [Gapotchenko.FX.Math.Intervals](../Gapotchenko.FX.Math.Intervals#readme)
  - [Gapotchenko.FX.Math.Metrics](../Gapotchenko.FX.Math.Metrics#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../..#readme).
