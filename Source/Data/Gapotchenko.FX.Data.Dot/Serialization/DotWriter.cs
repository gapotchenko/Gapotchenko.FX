using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    public class DotWriter : IDisposable
    {
        TextWriter _writer;

        /// <summary>
        /// Creates a writer instance.
        /// </summary>
        protected DotWriter(TextWriter reader)
        {
            _writer = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Write(DotToken token)
        {
            switch (token)
            {
                case DotToken.EOF:
                    break;
                case DotToken.DIGRAPH:
                    _writer.Write("digraph");
                    break;
                case DotToken.GRAPH:
                    _writer.Write("graph");
                    break;
                case DotToken.ARROW:
                    _writer.Write("->");
                    break;
                case DotToken.WHITESPACE:
                    _writer.Write(' ');
                    break;
                default:
                    if (token < DotToken.EOF)
                    {
                        _writer.Write((char)token);
                        break;
                    }

                    throw new ArgumentException("Unexpected token.", nameof(token));
            }
        }

        public void Write(DotToken token, string value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Creates a writer instance by used the specified text writer.
        /// </summary>
        /// <param name="output">The destination text writer.</param>
        public static DotWriter Create(TextWriter output)
        {
            return new DotWriter(output);
        }

        bool _disposed;

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_writer is not null)
                    {
                        _writer.Dispose();
                        _writer = null!;
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
