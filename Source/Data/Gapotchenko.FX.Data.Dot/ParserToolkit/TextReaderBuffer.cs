using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    sealed class TextReaderBuffer : ScanBuff
    {
        TextReader _reader;

        public override void Dispose()
        {
            if (_reader is not null)
            {
                _reader.Dispose();
                _reader = null!;
            }
        }

        delegate int BlockReader(char[] block, int index, int count);

        // Double buffer for char stream.
        sealed class BufferElement
        {
            StringBuilder _bldr = new();
            StringBuilder _next = new();
            int _minIx;
            int _maxIx;
            int _brkIx;
            bool _appendToNext;

            public int MaxIndex => _maxIx;

            public char this[int index]
            {
                get
                {
                    if (index < _minIx || index >= _maxIx)
                        throw new BufferException("Index was outside data buffer");
                    else if (index < _brkIx)
                        return _bldr[index - _minIx];
                    else
                        return _next[index - _brkIx];
                }
            }

            public void Append(char[] block, int count)
            {
                _maxIx += count;
                if (_appendToNext)
                {
                    _next.Append(block, 0, count);
                }
                else
                {
                    _bldr.Append(block, 0, count);
                    _brkIx = _maxIx;
                    _appendToNext = true;
                }
            }

            public string GetString(int start, int limit)
            {
                if (limit <= start)
                {
                    return "";
                }
                else if (start >= _minIx && limit <= _maxIx)
                {
                    if (limit < _brkIx) // String entirely in bldr builder
                    {
                        return _bldr.ToString(start - _minIx, limit - start);
                    }
                    else if (start >= _brkIx) // String entirely in next builder
                    {
                        return _next.ToString(start - _brkIx, limit - start);
                    }
                    else // Must do a string-concatenation
                    {
                        return
                            _bldr.ToString(start - _minIx, _brkIx - start) +
                            _next.ToString(0, limit - _brkIx);
                    }
                }
                else
                {
                    throw new BufferException("String was outside data buffer");
                }
            }

            public void Mark(int limit)
            {
                if (limit > _brkIx + 16) // Rotate blocks
                {
                    StringBuilder temp = _bldr;
                    _bldr = _next;
                    _next = temp;
                    _next.Length = 0;
                    _minIx = _brkIx;
                    _brkIx = _maxIx;
                }
            }
        }

        readonly BufferElement _data = new();
        int _position;

        public TextReaderBuffer(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));

            if (reader is StreamReader sr &&
                sr.BaseStream is FileStream fileStream)
            {
                FileName = fileStream.Name;
            }
        }

        /// <summary>
        /// Marks a conservative lower bound for the buffer,
        /// allowing space to be reclaimed.  If an application 
        /// needs to call GetString at arbitrary past locations 
        /// in the input stream, Mark() is not called.
        /// </summary>
        public override void Mark() { _data.Mark(_position - 2); }

        public override int Pos
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Read returns the ordinal number of the next char, or 
        /// EOF (-1) for an end of stream.  Note that the next
        /// code point may require *two* calls of Read().
        /// </summary>
        public override int Read()
        {
            //
            //  Characters at positions 
            //  [data.offset, data.offset + data.bldr.Length)
            //  are available in data.bldr.
            //
            if (_position < _data.MaxIndex)
            {
                // ch0 cannot be EOF
                return _data[_position++];
            }
            else // Read from underlying stream.
            {
                // Blocks of page size.
                char[] chrs = new char[4096];
                int count = _reader.Read(chrs, 0, 4096);
                if (count == 0)
                {
                    return EndOfFile;
                }
                else
                {
                    _data.Append(chrs, count);
                    return _data[_position++];
                }
            }
        }

        public override string GetString(int begin, int limit)
        {
            return _data.GetString(begin, limit);
        }
    }
}
