using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math.Topology
{
	partial class Graph<T>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int m_Version;

		void IncrementVersion() => ++m_Version;

		readonly struct ModificationGuard
		{
			public ModificationGuard(Graph<T> graph)
			{
				m_Graph = graph;
				m_Version = graph.m_Version;
			}

			readonly Graph<T> m_Graph;
			readonly int m_Version;

			[DoesNotReturn]
			public static void Throw() =>
				throw new InvalidOperationException("Graph was modified; enumeration operation may not execute.");

			public void Checkpoint()
			{
				if (m_Graph.m_Version != m_Version)
					Throw();
			}

			[return: NotNullIfNotNull("source")]
			public IEnumerable<T>? Protect(IEnumerable<T>? source) => source == null ? null : ProtectCore(source);

			IEnumerable<T> ProtectCore(IEnumerable<T> source)
			{
				foreach (var i in source)
				{
					Checkpoint();
					yield return i;
				}
			}
		}
	}
}
