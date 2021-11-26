using Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    partial class DotParser
    {
        bool _strict;
        Graph<DotDocumentVertex> _graph = new();
        DotSubgraph? _currentSubgraph;
        bool _directed;
        string? _graphId;
        List<DotSubgraph>? _subgraphsRoot;
        Stack<DotSubgraph?>? _subgraphsStack;
        Dictionary<string, string?> _properties = new();
        Dictionary<string, DotDocumentVertex> _vertices = new();

        internal class Cell<T>
        {
            public Cell<T>? left, right;
            public T? value;
            public Cell()
            {
            }
            public Cell(Cell<T>? l, T? v, Cell<T>? r)
            {
                left = l;
                right = r;
                value = v;
            }
            public T[] ToArray()
            {
                List<T> values = new List<T>();
                Walk(values);
                return values.ToArray();
            }
            private void Walk(List<T> values)
            {
                if (left != null)
                {
                    left.Walk(values);
                }
                if (value != null)
                {
                    values.Add(value);
                }
                if (right != null)
                {
                    right.Walk(values);
                }

            }
        }

        void CreateNewCurrentSubgraph(string? subgraphId)
        {
            var sg = new DotSubgraph(subgraphId);
            if (_currentSubgraph == null)
            {
                (_subgraphsRoot ??= new()).Add(sg);
            }
            else
            {
                _currentSubgraph.Subgraphs.Add(sg);
            }

            (_subgraphsStack ??= new()).Push(_currentSubgraph);

            _currentSubgraph = sg;
        }

        void PopCurrentSubgraph()
        {
            if (_subgraphsStack is null || _subgraphsStack.Count is 0)
                throw new InvalidOperationException("Cannot pop subgraph.");

            _currentSubgraph = _subgraphsStack.Pop();
        }

        Cell<T> MkSingleton<T>(T s)
        {
            return new Cell<T>(null, s, null);
        }

        Cell<T> Append<T>(Cell<T> x, Cell<T> y)
        {
            return new Cell<T>(x, default(T), y);
        }

        void MkEqStmt(string? attributeName, string? attributeValue)
        {
            if (attributeName is not null)
                _properties[attributeName] = attributeValue;
        }

        Cell<DotDocumentVertex> MkVertexStmt(DotDocumentVertex vertex, List<KeyValuePair<string, string>>? attributes)
        {
            if (attributes is not null)
            {
                foreach (var kv in attributes)
                {
                    vertex.Attributes[kv.Key] = kv.Value;
                }
            }

            return MkSingleton(vertex);
        }

        DotDocumentVertex MkVertex(string identifier)
        {
            if (!_vertices.TryGetValue(identifier, out var vertex))
            {
                vertex = new(identifier, _vertices.Count);
                _vertices.Add(identifier, vertex);
                _graph.Vertices.Add(vertex);
            }

            _currentSubgraph?.AddVertex(vertex);

            return vertex;
        }

        void MkGraphAttrStmt(List<KeyValuePair<string, string>>? attrs)
        {
            if (attrs is not null)
            {
                foreach (var kv in attrs)
                {
                    _properties[kv.Key] = kv.Value;
                }
            }
        }

        void MkVertexAttrStmt(List<KeyValuePair<string, string>>? al) { }
        void MkEdgeAttrStmt(List<KeyValuePair<string, string>>? al) { }

        void MkEdgeStmt(DotDocumentVertex src, DotDocumentVertex dst, List<KeyValuePair<string, string>>? attrs)
        {
            _graph.Edges.Add(src, dst);

            if (!_directed)
            {
                _graph.Edges.Add(dst, src);
            }

            // Edge properties are not supported.
            // AttributeValuePair.AddEdgeAttrs(attrs, edge);
        }

        void MkEdgeStmt(Cell<DotDocumentVertex> src, Cell<DotDocumentVertex> dst, List<KeyValuePair<string, string>>? attrs)
        {
            foreach (var s in src.ToArray())
            {
                foreach (var d in dst.ToArray())
                {
                    MkEdgeStmt(s, d, attrs);
                }
            }
        }

        Cell<DotDocumentVertex> MkEdgeStmt(Cell<DotDocumentVertex> src, Cell<Cell<DotDocumentVertex>> edges, List<KeyValuePair<string, string>>? attrs)
        {
            Cell<DotDocumentVertex> result = src;
            foreach (var dst in edges.ToArray())
            {
                MkEdgeStmt(src, dst, attrs);
                src = dst;
                result = new(result, null, dst);
            }
            return result;
        }

        KeyValuePair<string, string> MkAvPair(string src, string dst) =>
            new KeyValuePair<string, string>(src, dst);

        public DotParser(AbstractScanner<DotValueType, LexLocation> scanner) :
            base(scanner)
        { }

        public static Graph<DotDocumentVertex> Parse(TextReader reader)
        {
            var scanner = new DotScanner(reader);

            var parser = new DotParser(scanner);

            parser.Scanner = scanner;

            Graph<DotDocumentVertex>? graph = null;
            Exception? parseException = null;
            try
            {
                if (parser.Parse())
                {
                    graph = parser._graph;
                }
            }
            catch (Exception e)
            {
                parseException = e;
                parser.Scanner.yyerror(e.Message);
            }

            if (graph is null)
            {
                throw new ArgumentException(
                    $"Cannot parse DOT document. {scanner.Message} at {scanner.Line}:{scanner.Col}.",
                    nameof(reader),
                    parseException);
            }

            return graph;
        }

        protected override State[] States => states;
        protected override Rule[] Rules => rules;
        protected override string[] NonTerms => nonTerms;
    }
}
