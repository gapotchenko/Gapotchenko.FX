using System;

namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    /// <summary>
    /// Stack utility for the shift-reduce parser.
    /// </summary>
    /// <remarks>
    /// GPPG parsers have three instances:<br />
    /// (1) The parser state stack,<br />
    /// (2) The semantic value stack,<br />
    /// (3) The location stack.
    /// </remarks>
    sealed class PushdownPrefixState<T>
    {
        //  Note that we cannot use the BCL Stack<T> class
        //  here as derived types need to index into stacks.
        T?[] _array = new T[8];
        int _count = 0;

        /// <summary>
        /// Indexer for values of the stack below the top.
        /// </summary>
        /// <param name="index">Index of the element, starting from the bottom.</param>
        /// <returns>The selected element.</returns>
        public T this[int index]
        {
            get
            {
                if (index is < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _array[index]!;
            }
        }

        /// <summary>
        /// The current depth of the stack.
        /// </summary>
        public int Depth => _count;

        public void Push(T value)
        {
            if (_count >= _array.Length)
            {
                T[] newarray = new T[_array.Length * 2];
                Array.Copy(_array, newarray, _count);
                _array = newarray;
            }
            _array[_count++] = value;
        }

        public T Pop()
        {
            if (_count < 1)
                throw new InvalidOperationException("Stack is empty.");

            T rslt = _array[--_count]!;
            _array[_count] = default;
            return rslt;
        }

        public T TopElement()
        {
            if (_count < 1)
                throw new InvalidOperationException("Stack is empty.");

            return _array[_count - 1]!;
        }

        public bool IsEmpty() => _count == 0;
    }
}
