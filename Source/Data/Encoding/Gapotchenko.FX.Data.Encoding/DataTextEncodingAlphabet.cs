using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines an alphabet and related operations for <see cref="IDataTextEncoding"/> implementations.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct DataTextEncodingAlphabet
    {
        /// <summary>
        /// Initializes a new instance of a case-sensitive <see cref="DataTextEncodingAlphabet"/>.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public DataTextEncodingAlphabet(string symbols) :
            this(symbols, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataTextEncodingAlphabet"/>.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <param name="caseSensitive">Indicates whether alphabet is case sensitive.</param>
        public DataTextEncodingAlphabet(string symbols, bool caseSensitive)
        {
            if (symbols == null)
                throw new ArgumentNullException(nameof(symbols));

            m_Symbols = symbols;
            m_LookupTable = CreateLookupTable(symbols, caseSensitive);
        }

        const char BaseSymbol = '0';

        static byte[] CreateLookupTable(string symbols, bool caseSensitive)
        {
            var table = new byte[80];

#if NETCOREAPP || (NETSTANDARD && !NETSTANDARD2_0)
            Array.Fill(table, (byte)0xff);
#else
            for (int i = 0; i < table.Length; ++i)
                table[i] = 0xff;
#endif

            for (int i = 0; i < symbols.Length; ++i)
            {
                char symbol = symbols[i];

                if (caseSensitive)
                {
                    table[symbol - BaseSymbol] = (byte)i;
                }
                else
                {
                    table[char.ToLowerInvariant(symbol) - BaseSymbol] = (byte)i;
                    table[char.ToUpperInvariant(symbol) - BaseSymbol] = (byte)i;
                }
            }

            return table;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string m_Symbols;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly byte[] m_LookupTable;

        /// <summary>
        /// Gets the size of this alphabet.
        /// </summary>
        public int Size => m_Symbols.Length;

        /// <summary>
        /// Gets the alphabet symbols.
        /// </summary>
        public string Symbols => m_Symbols;

        /// <summary>
        /// Gets an alphabet symbol at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>An alphabet symbol at the specified index</returns>
        public char this[int index] => m_Symbols[index];

        /// <summary>
        /// Reports a zero-based index of the specified symbol in this alphabet.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A zero-based index position of the symbol if it is found, or -1 if it is not.</returns>
        public int IndexOf(char symbol)
        {
            int lookup = symbol - BaseSymbol;
            if (lookup < 0 || lookup >= m_LookupTable.Length)
                return -1;

            byte index = m_LookupTable[lookup];
            if (index == 0xff)
                return -1;

            return index;
        }
    }
}
