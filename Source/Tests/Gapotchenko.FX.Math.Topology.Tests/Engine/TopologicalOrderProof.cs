using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Tests.Engine
{
    sealed class TopologicalOrderProof
    {
        public int VerticesCount { get; set; }

        public int MaxGraphConfigurationsCount { get; set; }

        public bool VerifyMinimalDistance { get; set; }

        public Func<IEnumerable<int>, DependencyFunction<int>, IEnumerable<int>> Sorter
        {
            get;
            set;
        }

        int _CountOfBits;

        public void Run()
        {
            if (Sorter == null)
                throw new InvalidOperationException("Sorter is not set.");

            _CountOfBits = VerticesCount * VerticesCount;
            if (_CountOfBits > 64)
                throw new InvalidOperationException("Too many bits.");

            ulong maxIterator = (1UL << _CountOfBits) - 1UL;

            var iterators = _EnumerateIterators(maxIterator);

            if (MaxGraphConfigurationsCount != 0)
                iterators = iterators.Take(MaxGraphConfigurationsCount);

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
            DependencyFunction<int> df = (x, y) => DF(x, y, iterator);

            var source = Enumerable.Range(0, VerticesCount).ToArray();

            foreach (var sourcePermutation in source.Permute())
            {
                var currentSource = sourcePermutation.ToArray();

                Interlocked.Increment(ref _TotalCountOfExperiments);
                try
                {
                    var result = Sorter(currentSource, df).ToArray();
                    try
                    {
                        Validate(currentSource, result, df);

                        if (VerifyMinimalDistance)
                        {
                            if (!MinimalDistanceProof.Verify(currentSource, result, df))
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
                        _DumpTopology(currentSource, result, df);
                        throw;
                    }
                }
                catch (CircularDependencyException)
                {
                    if (!IgnoreCircularDependencyErrors)
                        throw;
                }

            }
        }

        public bool IgnoreCircularDependencyErrors
        {
            get;
            set;
        }

        sealed class DependencyGraph<T>
        {
            private DependencyGraph()
            {
            }

            IEqualityComparer<T> _EqualityComparer;
            IDictionary<T, Node> _Nodes;

            sealed class Node : HashSet<T>
            {
                public Node(IEqualityComparer<T> equalityComparer) :
                    base(equalityComparer)
                {
                }
            }

            public static DependencyGraph<T>? TryCreate(
                IEnumerable<T> source,
                DependencyFunction<T> dependencyFunction,
                IEqualityComparer<T> equalityComparer)
            {
                var list = source as IList<T>;
                if (list == null)
                    list = source.ToArray();

                int n = list.Count;
                if (n < 2)
                    return null;

                var graph = new DependencyGraph<T>();
                graph._EqualityComparer = equalityComparer;
                graph._Nodes = new Dictionary<T, Node>(n, equalityComparer);

                bool hasDependencies = false;

                for (int position = 0; position < n; ++position)
                {
                    var element = list[position];

                    Node node;
                    if (!graph._Nodes.TryGetValue(element, out node))
                    {
                        node = new Node(equalityComparer);
                        graph._Nodes.Add(element, node);
                    }

                    foreach (var anotherElement in list)
                    {
                        if (equalityComparer.Equals(element, anotherElement))
                            continue;

                        if (dependencyFunction(element, anotherElement))
                        {
                            node.Add(anotherElement);
                            hasDependencies = true;
                        }
                    }
                }

                if (!hasDependencies)
                    return null;

                return graph;
            }

            public bool DoesXHaveDirectDependencyOnY(T x, T y)
            {
                Node node;
                if (_Nodes.TryGetValue(x, out node))
                {
                    if (node.Contains(y))
                        return true;
                }
                return false;
            }

            sealed class DependencyTraverser
            {
                public DependencyTraverser(DependencyGraph<T> graph)
                {
                    _Graph = graph;
                    _VisitedNodes = new HashSet<T>(graph._EqualityComparer);
                }

                DependencyGraph<T> _Graph;
                HashSet<T> _VisitedNodes;

                public bool DoesAHaveTransientDependencyOnB(T a, T b)
                {
                    if (!_VisitedNodes.Add(a))
                        return false;

                    Node node;
                    if (_Graph._Nodes.TryGetValue(a, out node))
                    {
                        if (node.Contains(b))
                            return true;

                        foreach (var i in node)
                        {
                            if (DoesAHaveTransientDependencyOnB(i, b))
                                return true;
                        }
                    }

                    return false;
                }
            }

            public bool DoesAHaveTransientDependencyOnB(T a, T b)
            {
                var traverser = new DependencyTraverser(this);
                return traverser.DoesAHaveTransientDependencyOnB(a, b);
            }
        }

        public static bool Verify<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            DependencyFunction<T> df)
        {
            int n = source.Count();

            var _result = result.ToList();
            if (_result.Count != n)
                throw new ArgumentException("result.Length != source.Length");

            var graph = LazyEvaluation.Create(() => DependencyGraph<T>.TryCreate(source, df, EqualityComparer<T>.Default));

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (df(_result[j], _result[i]))
                    {
                        bool circularDependency = false;
                        if (graph.Value != null)
                        {
                            bool aDependsOnB = graph.Value.DoesAHaveTransientDependencyOnB(_result[j], _result[i]);
                            bool bDependsOnA = graph.Value.DoesAHaveTransientDependencyOnB(_result[i], _result[j]);
                            circularDependency = aDependsOnB && bDependsOnA;
                        }

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
            DependencyFunction<T> df)
        {
            if (!Verify(source, result, df))
                throw new Exception("Topological order is violated.");
        }

        static object _ConsoleSync = new object();

        static void _DumpTopology<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            DependencyFunction<T> df)
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

        public static IEnumerable<IEnumerable<T>> AllOrdersOf<T>(IEnumerable<T> source, DependencyFunction<T> df)
        {
            source = source.Memoize();
            return source
                .Permute()
                .Select(x => x.Memoize())
                .Where(x => Verify(source, x, df));
        }
    }
}
