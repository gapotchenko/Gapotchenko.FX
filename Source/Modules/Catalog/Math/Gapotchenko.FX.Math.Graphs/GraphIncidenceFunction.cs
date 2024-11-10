// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Assigns a pair of graph vertices to an edge.
/// </summary>
/// <typeparam name="TVertex">The type of vertices in the graph.</typeparam>
/// <param name="from">The source vertex of the edge.</param>
/// <param name="to">The destination vertex of the edge.</param>
/// <returns>
/// <see langword="true"/> if there is an edge between the <paramref name="from"/> and the <paramref name="to"/> vertices;
/// otherwise, <see langword="false"/>.
/// </returns>
public delegate bool GraphIncidenceFunction<in TVertex>(TVertex from, TVertex to);
