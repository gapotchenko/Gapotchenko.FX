using Gapotchenko.FX.Data.Dot.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    sealed class IntermediateGraphBuilder : DotSyntaxWalker
    {
        public IntermediateGraphBuilder(bool directedGraph)
        {
            _directedGraph = directedGraph;
        }

        readonly bool _directedGraph;
        readonly Dictionary<string, DotDocumentVertex> _vertices = new();
        readonly Stack<List<DotDocumentVertex>> _edgeElementsStack = new();
        readonly Graph<DotDocumentVertex> _graph = new();

        public IReadOnlyGraph<DotDocumentVertex> Graph => _graph;

        public override void VisitDotEdgeSyntax(DotEdgeSyntax node)
        {
            var elements = node.Elements;
            if (elements is not null)
            {
                var edgeElements = new List<DotDocumentVertex>();
                _edgeElementsStack.Push(edgeElements);

                List<DotDocumentVertex>? previousList = null;

                foreach (var element in elements)
                {
                    Visit(element);

                    if (previousList is not null)
                    {
                        foreach (var from in previousList)
                        {
                            foreach (var to in edgeElements)
                            {
                                AddEdge(from, to);
                            }
                        }
                    }

                    previousList = edgeElements.ToList();
                    edgeElements.Clear();
                }

                _edgeElementsStack.Pop();
            }
        }

        public override void VisitDotVertexIdentifierSyntax(DotVertexIdentifierSyntax node)
        {
            var vertex = GetVertex(node.Identifier.Value);
            if (_edgeElementsStack.Count != 0)
            {
                _edgeElementsStack.Peek().Add(vertex);
            }

            base.VisitDotVertexIdentifierSyntax(node);
        }

        Dictionary<string, string> _vertexAttributes = new();
        bool _acceptVertexAttributes = false;

        public override void VisitDotVertexSyntax(DotVertexSyntax node)
        {
            _vertexAttributes.Clear();

            _acceptVertexAttributes = true;
            base.VisitDotVertexSyntax(node);
            _acceptVertexAttributes = false;

            var vertex = GetVertex(node.Identifier.Identifier.Value);
            foreach (var kv in _vertexAttributes)
            {
                vertex.Attributes[kv.Key] = kv.Value;
            }
        }

        public override void VisitDotAttributeSyntax(DotAttributeSyntax node)
        {
            if (_acceptVertexAttributes)
            {
                var key = node.LHS.Value;
                var value = node.RHS.Value;
                _vertexAttributes[key] = value;
            }

            base.VisitDotAttributeSyntax(node);
        }

        DotDocumentVertex GetVertex(string id)
        {
            if (!_vertices.TryGetValue(id, out var vertex))
            {
                vertex = new DotDocumentVertex(id, _vertices.Count);
                _vertices.Add(id, vertex);
                _graph.Vertices.Add(vertex);
            }

            return vertex;
        }

        void AddEdge(DotDocumentVertex from, DotDocumentVertex to)
        {
            _graph.Edges.Add(from, to);

            if (!_directedGraph)
            {
                _graph.Edges.Add(to, from);
            }
        }
    }
}
