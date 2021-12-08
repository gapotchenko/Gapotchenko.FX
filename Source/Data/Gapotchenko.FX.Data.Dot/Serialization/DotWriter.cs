using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    /// <summary>
    /// A forward-only DOT document data writer.
    /// </summary>
    public class DotWriter : IDisposable
    {
        TextWriter _writer;

        /// <summary>
        /// Creates a <see cref="DotWriter"/> instance.
        /// </summary>
        protected DotWriter(TextWriter reader)
        {
            _writer = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>
        /// Writes a token with its default value.
        /// </summary>
        /// <param name="token">Token kind.</param>
        public void Write(DotTokenKind token)
        {
            if (!token.TryGetDefaultValue(out var value))
                throw new ArgumentException("Unexpected token.", nameof(token));
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a token with a provided value.
        /// </summary>
        /// <param name="token">Token kind.</param>
        /// <param name="value">Token value.</param>
        public void Write(DotTokenKind token, string value)
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
