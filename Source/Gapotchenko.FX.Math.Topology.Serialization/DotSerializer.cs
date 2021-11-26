﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
    /// <summary>
    /// Serializes and deserializes graphs into and from DOT documents.
    /// </summary>
    public class DotSerializer
    {
        /// <summary>
        /// Serializes the specified graph.
        /// </summary>
        /// <typeparam name="T">Type of graph vertices.</typeparam>
        /// <param name="graph">The graph to serialize.</param>
        /// <returns>DOT document representing the specified graph.</returns>
        public string Serialize<T>(IReadOnlyGraph<T> graph)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            using var writer = new StringWriter();
            Serialize(graph, writer);
            return writer.ToString();
        }

        /// <summary>
        /// Serializes the specified graph and writes the DOT document to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <typeparam name="T">Type of graph vertices.</typeparam>
        /// <param name="graph">The graph to serialize.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the XML document</param>
        public void Serialize<T>(IReadOnlyGraph<T> graph, TextWriter textWriter)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (textWriter is null)
                throw new ArgumentNullException(nameof(textWriter));

            var dotCompiler = new DotCompiler();
            dotCompiler.Serialize(graph, textWriter);
        }

        /// <summary>
        /// Deserializes the DOT document.
        /// </summary>
        /// <typeparam name="T">Type of graph vertices.</typeparam>
        /// <param name="graph">Destination graph.</param>
        /// <param name="document">DOT document to deserialize.</param>
        public void Deserialize<T>(IGraph<T> graph, string document)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (document is null)
                throw new ArgumentNullException(nameof(document));

            using var reader = new StringReader(document);
            Deserialize(graph, reader);
        }

        /// <summary>
        /// Deserializes the DOT document contained by the specified <see cref="TextReader"/>.
        /// </summary>
        /// <typeparam name="T">Type of graph vertices.</typeparam>
        /// <param name="graph">Destination graph.</param>
        /// <param name="textReader">The <see cref="TextReader"/> that contains the DOT document to deserialize.</param>
        public void Deserialize<T>(IGraph<T> graph, TextReader textReader)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (textReader is null)
                throw new ArgumentNullException(nameof(textReader));

            var intermediateGraph = DotParser.Parse(textReader);
            ConvertGraph(intermediateGraph, graph);
        }

        delegate object StringToVertexConverter(string name, IReadOnlyDictionary<string, string>? properties);
        delegate string VertexToStringConverter(object vertex, IDictionary<string, string>? properties);

        ConcurrentDictionary<Type, StringToVertexConverter>? _stringToVertexConverters;
        ConcurrentDictionary<Type, VertexToStringConverter>? _vertexToStringConverters;

        StringToVertexConverter GetStringToVertexConverter(Type type) =>
            (_stringToVertexConverters ??= new()).GetOrAdd(type, CreateStringToVertexConverter);

        VertexToStringConverter GetVertexToStringConverter(Type type) =>
            (_vertexToStringConverters ??= new()).GetOrAdd(type, CreateVertexToStringConverter);

        static StringToVertexConverter CreateStringToVertexConverter(Type type)
        {
            if (type == typeof(string))
            {
                return (s, p) => s;
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (s, p) => converter.ConvertFromString(s) ??
                    throw new InvalidOperationException("Vertex cannot be null.");
            }

            throw new ArgumentException($"Cannot convert '{typeof(string)}' to '{type}'.");
        }

        static VertexToStringConverter CreateVertexToStringConverter(Type type)
        {
            if (type == typeof(string))
            {
                return (v, p) => (string)v;
            }
            else if (type.GetInterfaces().Contains(typeof(IConvertible)))
            {
                return (v, p) => (string)Convert.ChangeType(v, typeof(string));
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertTo(typeof(string)))
            {
                return (v, p) => converter.ConvertToString(v) ??
                    throw CreateVertexStringNullException();
            }

            return (v, p) => v.ToString() ??
                throw CreateVertexStringNullException();

            static Exception CreateVertexStringNullException() =>
                new InvalidOperationException("Vertex string representation cannot be null.");
        }

        void ConvertGraph<T>(Graph<DotDocumentVertex> source, IGraph<T> destination)
        {
            var verticesMap =
                source.Vertices
                    .OrderBy(v => v.SourceDocumentIndex)
                    .ToDictionary(v => v, DeserializeVertex<T>);

            foreach (var v in source.Vertices)
            {
                destination.Vertices.Add(verticesMap[v]);
            }

            foreach (var (from, to) in source.Edges)
            {
                destination.Edges.Add(verticesMap[from], verticesMap[to]);
            }
        }

        /// <summary>
        /// Deserializes a single vertex.
        /// </summary>
        /// <typeparam name="T">Vertex type.</typeparam>
        /// <param name="vertex">Vertex data.</param>
        /// <returns>Deserialized vertex.</returns>
        protected virtual T DeserializeVertex<T>(IDotVertex vertex)
        {
            var name = vertex.Identifier;
            if (vertex.Attributes?.TryGetValue("label", out var label) == true)
            {
                name = label;
            }

            name ??= string.Empty;

            var converter = GetStringToVertexConverter(typeof(T));
            return (T)converter(name, vertex.Attributes);
        }

        /// <summary>
        /// Serializes a single vertex.
        /// </summary>
        /// <typeparam name="T">Vertex type.</typeparam>
        /// <param name="vertex">Vertex to serialize.</param>
        /// <returns>Serialized vertex.</returns>
        protected virtual IDotVertex SerializeVertex<T>(T vertex)
            where T : notnull
        {
            var converter = GetVertexToStringConverter(typeof(T));
            var identifier = converter(vertex, null);
            return new DotVertex(identifier, null);
        }
    }
}
