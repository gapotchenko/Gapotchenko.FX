using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Gapotchenko.FX.Math.Topology.Tests.Engine
{
    sealed class TopologicalOrderProof
    {
        public int VerticesCount { get; init; }

        public int MaxTopologiesCount { get; init; }

        public bool VerifyMinimalDistance { get; init; }

        public bool CircularDependenciesEnabled { get; init; } = true;

        public Func<IEnumerable<int>, Func<int, int, bool>, IEnumerable<int>>? PredicateSorter { get; init; }

        public Func<Graph<int>, IEnumerable<int>>? GraphSorter { get; init; }

        int _CountOfBits;

        public void Run()
        {
            if (PredicateSorter == null && GraphSorter == null)
                throw new InvalidOperationException("Sorter is not set.");

            _CountOfBits = VerticesCount * VerticesCount;
            if (_CountOfBits > 64)
                throw new InvalidOperationException("Too many bits.");

            ulong maxIterator = (1UL << _CountOfBits) - 1UL;

            var iterators = _EnumerateIterators(maxIterator);

            if (MaxTopologiesCount != 0)
                iterators = iterators.Take(MaxTopologiesCount);

            Console.WriteLine("Count of graph configurations: {0}", iterators.Count());

            _TotalCountOfExperiments = 0;
            _TotalCountOfExperimentsWithViolatedMinimalDistance = 0;

            DebuggableParallel.ForEach(iterators, _CheckIterator);

            if (_TotalCountOfExperimentsWithViolatedMinimalDistance != 0)
            {
                throw new Exception(string.Format(
                    "Minimal distance is not reached in {0:P2} of experiments ({1} from {2}).",
                    ((double)_TotalCountOfExperimentsWithViolatedMinimalDistance / _TotalCountOfExperiments),
                    _TotalCountOfExperimentsWithViolatedMinimalDistance,
                    _TotalCountOfExperiments));
            }
        }

        long _TotalCountOfExperiments;
        long _TotalCountOfExperimentsWithViolatedMinimalDistance;

        bool DF(int x, int y, ulong iterator)
        {
            int selector = x + y * VerticesCount;
            ulong mask = 1UL << selector;
            return (iterator & mask) != 0;
        }

        IEnumerable<ulong> _EnumerateIterators(ulong maxIterator)
        {
            for (ulong iterator = 0; iterator <= maxIterator; ++iterator)
            {
                bool skip = false;

                // Ensure that df(x, x) never returns true for all x ∈ [0; N).
                for (int x = 0; x < VerticesCount; ++x)
                {
                    if (DF(x, x, iterator))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                    continue;

                yield return iterator;
            }
        }

        void _CheckIterator(ulong iterator)
        {
            bool df(int x, int y) => DF(x, y, iterator);

            var source = Enumerable.Range(0, VerticesCount).ToArray();

            var g = LazyEvaluation.Create(() => new Graph<int>(source, (from, to) => df(to, from)));
            if (!CircularDependenciesEnabled)
            {
                if (g.Value.IsCyclic)
                    return;
            }

            if (GraphSorter != null)
            {
                var result = GraphSorter(g.Value);
                CheckResult(source, result, df);
            }
            else
            {
                foreach (var currentSource in source.Permute())
                {
                    Interlocked.Increment(ref _TotalCountOfExperiments);

                    IEnumerable<int> result;

                    if (PredicateSorter != null)
                        result = PredicateSorter(currentSource, df);
                    else
                        throw new InvalidOperationException("Cannot select a sorter.");

                    CheckResult(currentSource, result, df);
                }
            }
        }

        void CheckResult<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            Func<T, T, bool> df)
        {
            source = source.Memoize();
            result = result.Memoize();
            try
            {
                Validate(source, result, df);

                if (VerifyMinimalDistance)
                {
                    if (!MinimalDistanceProof.Verify(source, result, df))
                    {
                        if (Debugger.IsAttached)
                            throw new Exception("Minimal distance not reached.");
                        else
                            Interlocked.Increment(ref _TotalCountOfExperimentsWithViolatedMinimalDistance);
                    }
                }
            }
            catch
            {
                _DumpTopology(source, result, df);
                throw;
            }
        }

        public static bool Verify<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            Func<T, T, bool> df)
        {
            int n = source.Count();

            var _result = result.ToList();
            if (_result.Count != n)
                throw new ArgumentException("result.Length != source.Length");

            var graph = new Graph<T>(source, (from, to) => df(from, to));

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (df(_result[j], _result[i]))
                    {
                        bool aDependsOnB = graph.HasPath(_result[j], _result[i]);
                        bool bDependsOnA = graph.HasPath(_result[i], _result[j]);

                        bool circularDependency = aDependsOnB && bDependsOnA;
                        if (!circularDependency)
                            return false;
                    }
                }
            }

            return true;
        }

        public static void Validate<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            Func<T, T, bool> df)
        {
            if (!Verify(source, result, df))
                throw new Exception("Topological order is violated.");
        }

        static object _ConsoleSync = new object();

        static void _DumpTopology<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            Func<T, T, bool> df)
        {
            var equalityComparer = EqualityComparer<T>.Default;

            lock (_ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine("*** Topology Dump ***");

                Console.Write("Source: ");
                Console.WriteLine(string.Join(", ", source));

                Console.Write("Result: ");
                Console.WriteLine(string.Join(", ", result));

                Console.WriteLine("Dependencies:");
                foreach (var x in source)
                {
                    bool depWritten = false;

                    foreach (var y in source)
                    {
                        if (equalityComparer.Equals(x, y))
                            continue;

                        bool xDependsOnY = df(x, y);

                        if (xDependsOnY)
                        {
                            if (!depWritten)
                            {
                                Console.Write(x);
                                Console.Write(" -> ");
                                depWritten = true;
                            }

                            Console.Write(y);
                            Console.Write(" ");
                        }
                    }

                    if (depWritten)
                        Console.WriteLine();
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> AllOrdersOf<T>(IEnumerable<T> source, Func<T, T, bool> df)
        {
            source = source.Memoize();
            return source
                .Permute()
                .Select(x => x.Memoize())
                .Where(x => Verify(source, x, df));
        }
    }
}
