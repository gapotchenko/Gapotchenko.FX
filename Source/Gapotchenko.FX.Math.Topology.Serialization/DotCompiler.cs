using Gapotchenko.FX.Data.Dot.Dom;
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    sealed class DotCompiler<TVertex>
    {
        readonly Func<TVertex, IDotVertex> _vertexSerializer;

        public DotCompiler(Func<TVertex, IDotVertex> serializeVertex)
        {
            _vertexSerializer = serializeVertex;
        }

        sealed class IdVertex
        {
            public IdVertex(int index, IDotVertex vertex, TVertex originalVertex, string stringId, string? label)
            {
                Index = index;
                Vertex = vertex;
                OriginalVertex = originalVertex;
                StringId = stringId;
                Label = label;
            }

            public readonly int Index;
            public readonly IDotVertex Vertex;
            public readonly TVertex OriginalVertex;
            public readonly string StringId;
            public readonly string? Label;
        }

        sealed class DotDocumentModel
        {
            public List<IReadOnlyList<IReadOnlyList<string>>> Edges { get; } = new();
            public List<(string vertex, IReadOnlyList<(string attributeName, string attributeValue)> attributes)> Vertices { get; } = new();
        }

        /// <summary>
        /// Converts given graph into the dot language.
        /// </summary>
        public DotDocument Serialize(IReadOnlyGraph<TVertex> graph)
        {
            HashSet<string> stringIdSet = new();

            var idGraph = new Graph<IdVertex>(
                graph.Vertices.Select((v, index) => CreateIdVertex(v, index, stringIdSet)),
                (from, to) => graph.Edges.Contains(new GraphEdge<TVertex>(from.OriginalVertex, to.OriginalVertex)));

            var model = CreateDocumentModel(idGraph);

            return Serialize(model);
        }

        DotDocumentModel CreateDocumentModel(IReadOnlyGraph<IdVertex> idGraph)
        {
            var model = new DotDocumentModel();

            var transposedIdGraph = idGraph.GetTransposition();
            HashSet<IdVertex> isolatedVertices = new();

            foreach (var v in idGraph.Vertices)
            {
                var adjacentTo = idGraph.VerticesAdjacentTo(v);
                if (adjacentTo.Any())
                {
                    var from = new[] { v.StringId };

                    var to = adjacentTo
                        .Select(w => w.StringId)
                        .ToList();

                    model.Edges.Add(new IReadOnlyList<string>[] { from, to });
                }
                else if (!transposedIdGraph.VerticesAdjacentTo(v).Any())
                {
                    isolatedVertices.Add(v);
                }
            }

            foreach (var v in isolatedVertices)
            {
                AddVertex(model, v);
            }

            foreach (var v in idGraph.Vertices)
            {
                if (HasAttributes(v) &&
                    !isolatedVertices.Contains(v))
                {
                    AddVertex(model, v);
                }
            }

            return model;
        }

        static bool HasAttributes(IdVertex v)
        {
            return v.Vertex.Attributes.Any() ||
                v.Label != null;
        }

        static void AddVertex(DotDocumentModel model, IdVertex v)
        {
            var attributes = v.Vertex.Attributes;
            var label = v.Label;

            IReadOnlyList<(string, string)> attrs;
            if (attributes is null && label is null)
            {
                attrs = Empty<(string, string)>.Array;
            }
            else
            {
                var list = new List<(string, string)>();
                if (attributes is not null)
                {
                    foreach (var kv in attributes)
                    {
                        list.Add((kv.Key, kv.Value));
                    }
                }

                if (label is not null)
                {
                    list.Add(("label", label));
                }

                attrs = list;
            }

            model.Vertices.Add((v.StringId, attrs));
        }

        IdVertex CreateIdVertex(TVertex v, int index, HashSet<string> stringIdSet)
        {
            var mappedVertex = _vertexSerializer(v);

            var hasLabelAttribute = mappedVertex.Attributes?.ContainsKey("label") == true;

            var stringId = mappedVertex.Identifier;

            string? label = null;
            if (!stringIdSet.Add(stringId))
            {
                if (!hasLabelAttribute)
                {
                    label = stringId;
                }

                stringId = $"v{index}";
                int duplicateCounter = 0;
                while (!stringIdSet.Add(stringId))
                {
                    stringId = $"v{index}_{++duplicateCounter}";
                }
            }

            return new IdVertex(index, mappedVertex, v, stringId, label);
        }

        static DotDocument Serialize(DotDocumentModel model)
        {
            return new DotDocument(DotSyntaxFactory.Graph(
                strict: default,
                kind: DotSyntaxFactory.Token(DotToken.Digraph),
                identifier: default,
                statements: CreateStatements(model)));
        }

        static DotStatementListSyntax CreateStatements(DotDocumentModel model)
        {
            List<DotStatementSyntax> statements = new();

            foreach (var edge in model.Edges)
            {
                statements.Add(CreateEdgeStatement(edge));
            }

            foreach (var vertex in model.Vertices)
            {
                statements.Add(CreateVertexStatement(vertex.vertex, vertex.attributes));
            }

            return DotSyntaxFactory.StatementList(
                DotSyntaxFactory.List(statements));
        }

        static DotVertexSyntax CreateVertexStatement(string vertex, IEnumerable<(string attributeName, string attributeValue)>? attributes)
        {
            return DotSyntaxFactory.VertexStatement(
                identifier: CreateIdentifier(vertex),
                attributes: CreateAttributesList(attributes));
        }

        static DotVertexIdentifierSyntax CreateIdentifier(string identifier)
        {
            return DotSyntaxFactory.Identifier(identifier);
        }

        static DotSyntaxList<DotAttributeListSyntax>? CreateAttributesList(IEnumerable<(string attributeName, string attributeValue)>? attributes)
        {
            if (attributes?.Any() != true)
                return null;

            var attributesList = attributes.Select(attr => DotSyntaxFactory.Attribute(
                name: attr.attributeName,
                value: attr.attributeValue));

            return DotSyntaxFactory.List(new[]
            {
                DotSyntaxFactory.AttributeList(
                    DotSyntaxFactory.List(attributesList))
            });
        }

        static DotEdgeSyntax CreateEdgeStatement(IEnumerable<IEnumerable<string>> edge)
        {
            return DotSyntaxFactory.Edge(
                elements: DotSyntaxFactory.SeparatedList(
                    edge.Select(CreateEdgeElement),
                    DotSyntaxFactory.Token(DotToken.Arrow, "->")),
                attributes: default);

            static DotSyntaxNode CreateEdgeElement(IEnumerable<string> edgeElement)
            {
                if (edgeElement.Skip(1).Any())
                {
                    var subgraphStatements = DotSyntaxFactory.List(
                        edgeElement.Select(v => (DotStatementSyntax)CreateVertexStatement(v, default)));

                    return DotSyntaxFactory.Graph(
                        default,
                        default,
                        default,
                        DotSyntaxFactory.StatementList(subgraphStatements));
                }
                else
                {
                    var vertex = edgeElement.Single();
                    return CreateVertexStatement(vertex, default);
                }
            }
        }
    }
}
