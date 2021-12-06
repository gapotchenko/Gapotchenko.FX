using Gapotchenko.FX.Data.Dot.ParserToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    /// <summary>
    /// A forward-only DOT document data reader.
    /// </summary>
    public class DotReader : IDisposable
    {
        DotLex _lexer;

        DotToken _currentToken;

        /// <summary>
        /// Creates a reader instance.
        /// </summary>
        protected DotReader(TextReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            _lexer = new DotLex(reader);
        }

        /// <summary>
        /// Reads the next node from the stream.
        /// </summary>
        /// <returns>true if the next node was read successfully; otherwise, false.</returns>
        public bool Read()
        {
            _currentToken = (DotToken)_lexer.yylex();
            return _currentToken != DotToken.EOF;
        }

        /// <summary>
        /// Gets the type of the current token.
        /// </summary>
        public DotToken TokenType =>
            _currentToken;

        /// <summary>
        /// Gets the text value of the current node.
        /// </summary>
        public string Value =>
            _lexer.yylval ?? string.Empty;

        /// <summary>
        /// Gets the current node location.
        /// </summary>
        public (int Line, int Column) Location =>
            (_lexer.Line, _lexer.Col);

        /// <summary>
        /// Creates a reader instance by used the specified text reader.
        /// </summary>
        /// <param name="input">
        /// The text reader from which to read the data.
        /// </param>
        public static DotReader Create(TextReader input)
        {
            return new DotReader(input);
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
                    if (_lexer is not null)
                    {
                        _lexer.Dispose();
                        _lexer = null!;
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
