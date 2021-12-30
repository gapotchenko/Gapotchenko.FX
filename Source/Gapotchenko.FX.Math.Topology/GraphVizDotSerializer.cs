using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// GraphVis DOT format serializer.
    /// </summary>
    struct GraphVizDotSerializer
    {
        public string? Format { get; set; }
        public IFormatProvider? FormatProvider { get; set; }

        public void Serialize<T>(Graph<T> graph, TextWriter writer)
        {
            writer.WriteLine("digraph {");

            var map = new AssociativeArray<T, int>(graph.VertexComparer);

            int GetVerticeId(T v)
            {
                if (!map.TryGetValue(v, out var id))
                {
                    id = map.Count + 1;
                    map.Add(v, id);
                }
                return id;
            }

            void WriteVerticeId(T v)
            {
                writer.Write('v');
                writer.Write(GetVerticeId(v));
            }

            foreach (var (v, row) in graph.AdjacencyList)
            {
                writer.Write('\t');
                WriteVerticeId(v);

                if (!row.IsNullOrEmpty())
                {
                    writer.Write(" -> ");
                    writer.Write("{ ");

                    bool first = true;
                    foreach (var i in row)
                    {
                        if (first)
                            first = false;
                        else
                            writer.Write("; ");
                        WriteVerticeId(i);
                    }

                    writer.Write(" }");
                }

                writer.WriteLine();
            }

            if (map.Count != 0)
            {
                writer.WriteLine();

                foreach (var i in map.Keys)
                {
                    writer.Write('\t');
                    WriteVerticeId(i);

                    writer.Write(" [label=");
                    WriteString(writer, GetObjectString(i));
                    writer.WriteLine("]");
                }
            }

            writer.Write("}");
        }

        string? GetObjectString(object? obj) =>
            obj switch
            {
                null => null,
                IFormattable f => f.ToString(Format, FormatProvider),
                _ => obj.ToString()
            };

        static void WriteString(TextWriter writer, string? s)
        {
            writer.Write('"');
            if (s != null)
            {
                foreach (var c in s)
                {
                    switch (c)
                    {
                        case '"':
                            writer.Write('\\');
                            break;
                    }
                    writer.Write(c);
                }
            }
            writer.Write('"');
        }
    }
}
