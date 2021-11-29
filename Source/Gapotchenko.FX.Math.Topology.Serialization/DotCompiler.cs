using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    sealed class DotCompiler
    {
        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        struct IdVertex<T>
        {
            public IdVertex(int id, T value)
            {
                Id = id;
                Value = value;
            }

            public readonly int Id;
            public readonly T Value;
        }

        /// <summary>
        /// Converts given graph into the dot language.
        /// </summary>
        public void Serialize<T>(
            IReadOnlyGraph<T> graph,
            TextWriter writer)
        {
            var indentedWriter = new IndentedTextWriter(writer);

            indentedWriter.WriteLine("digraph {");

            indentedWriter.Indent++;

            HashSet<string> idSet = new();
            Dictionary<int, string> idMap = new();
            Dictionary<int, string> labelsMap = new();

            string GetVertexId(IdVertex<T> v)
            {
                if (!idMap.TryGetValue(v.Id, out var stringId))
                {
                    var label = GetVertexLabel(v.Value) ?? string.Empty;
                    label = EscapeLabel(label);
                    if (string.IsNullOrEmpty(label) ||
                        !ValidIdentifierPattern.IsMatch(label))
                    {
                        label = "\"" + label + "\"";
                    }

                    if (idSet.Add(label))
                    {
                        stringId = label;
                    }
                    else
                    {
                        stringId = $"v{v.Id}";
                        int duplicateCounter = 0;
                        while (!idSet.Add(stringId))
                        {
                            stringId = $"v{v.Id}_{++duplicateCounter}";
                        }

                        labelsMap[v.Id] = label;
                    }

                    idMap[v.Id] = stringId;
                }

                return stringId;
            }

            void WriteVertexId(IdVertex<T> v)
            {
                var stringId = GetVertexId(v);
                indentedWriter.Write(stringId);
            }

            var idGraph = new Graph<IdVertex<T>>(
                graph.Vertices.Select((v, id) => new IdVertex<T>(id, v)),
                (from, to) => graph.Edges.Contains(new GraphEdge<T>(from.Value, to.Value)));

            var transposedIdGraph = idGraph.GetTransposition();

            foreach (var v in idGraph.Vertices)
            {
                var adjacentTo = idGraph.VerticesAdjacentTo(v);
                if (adjacentTo.Any())
                {
                    WriteVertexId(v);

                    indentedWriter.Write(" -> ");

                    var twoOrMore = adjacentTo.Skip(1).Any();
                    if (twoOrMore)
                    {
                        indentedWriter.Write("{ ");
                    }

                    bool first = true;
                    foreach (var w in adjacentTo)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            indentedWriter.Write(' ');
                        }

                        WriteVertexId(w);
                    }

                    if (twoOrMore)
                    {
                        indentedWriter.Write(" }");
                    }

                    indentedWriter.WriteLine();
                }
                else if (!transposedIdGraph.VerticesAdjacentTo(v).Any())
                {
                    WriteVertexId(v);
                    indentedWriter.WriteLine();
                }
            }

            if (labelsMap.Count != 0)
            {
                indentedWriter.WriteLine();

                foreach (var kv in labelsMap)
                {
                    var id = idMap[kv.Key];
                    var label = labelsMap[kv.Key];
                    indentedWriter.WriteLine($"{id} [label={label}]");
                }
            }

            indentedWriter.Indent--;

            indentedWriter.Write("}");
        }

        static string EscapeLabel(string label)
        {
            return label
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\r\\n")
                .Replace("\n", "\\n");
        }

        string? GetVertexLabel(object? obj)
        {
            switch (obj)
            {
                case null:
                    return null;
                case string s:
                    return s;
                case IConvertible:
                    return (string)Convert.ChangeType(obj, typeof(string));
            }

            var converter = TypeDescriptor.GetConverter(obj);
            if (converter.CanConvertTo(typeof(string)))
            {
                return converter.ConvertToString(obj);
            }

            return obj.ToString();
        }
    }
}
