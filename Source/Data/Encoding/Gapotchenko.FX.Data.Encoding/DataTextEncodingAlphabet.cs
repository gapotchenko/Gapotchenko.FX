﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines an alphabet and related operations for <see cref="IDataTextEncoding"/> implementations.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DataTextEncodingAlphabet
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
        public DataTextEncodingAlphabet(string symbols, bool caseSensitive) :
            this(symbols, caseSensitive, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataTextEncodingAlphabet"/>.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <param name="caseSensitive">Indicates whether alphabet is case sensitive.</param>
        /// <param name="synonyms">The synonyms.</param>
        public DataTextEncodingAlphabet(
            string symbols,
            bool caseSensitive,
            IReadOnlyDictionary<char, string>? synonyms)
        {
            if (symbols == null)
                throw new ArgumentNullException(nameof(symbols));

            m_Symbols = symbols;

            m_LookupTable = TryCreateLookupTable(symbols, caseSensitive, synonyms);

            if (m_LookupTable != null)
                m_LookupDictionary = null;
            else
                m_LookupDictionary = CreateLookupDictionary(symbols, caseSensitive, synonyms);
        }

        static byte[]? TryCreateLookupTable(
            string symbols,
            bool caseSensitive,
            IReadOnlyDictionary<char, string>? synonyms)
        {
            if (symbols.Length >= 0xff)
            {
                // Discard lookup table creation due to inability to represent symbol variety.
                return null;
            }

            var table = new byte[LookupTableSize];

#if NETCOREAPP || (NETSTANDARD && !NETSTANDARD2_0)
            Array.Fill(table, (byte)0xff);
#else
            for (int i = 0; i < LookupTableSize; ++i)
                table[i] = 0xff;
#endif

            for (int i = 0; i < symbols.Length; ++i)
            {
                foreach (var c in EnumerateSymbolVariations(symbols[i], caseSensitive, synonyms))
                {
                    if (c < LookupTableBaseSymbol)
                    {
                        // Discard lookup table creation due to underflow.
                        return null;
                    }

                    int key = c - LookupTableBaseSymbol;
                    if (key >= LookupTableSize)
                    {
                        // Discard lookup table creation due to overflow.
                        return null;
                    }

                    if (table[key] != 0xff)
                        throw CreateSymbolClashException(c);

                    table[key] = (byte)i;
                }
            }

            return table;
        }

        static Dictionary<char, int> CreateLookupDictionary(
            string symbols,
            bool caseSensitive,
            IReadOnlyDictionary<char, string>? synonyms)
        {
            int symbolCount = symbols.Length;
            var dictionary = new Dictionary<char, int>(symbolCount);

            for (int i = 0; i < symbolCount; ++i)
                foreach (var c in EnumerateSymbolVariations(symbols[i], caseSensitive, synonyms))
                {
                    if (dictionary.ContainsKey(c))
                        throw CreateSymbolClashException(c);

                    dictionary[c] = (byte)i;
                }

            return dictionary;
        }

        static Exception CreateSymbolClashException(char symbol) =>
            new Exception(
                string.Format(
                    "Encountered a clash in data encoding alphabet for symbol '{0}' (0x{1:X}).",
                    symbol,
                    (int)symbol));

        static IEnumerable<char> EnumerateSymbolVariations(
            char symbol,
            bool caseSensitive,
            IReadOnlyDictionary<char, string>? synonyms)
        {
            foreach (var c in EnumerateSymbolVariations(symbol, caseSensitive))
                yield return c;

            if (synonyms != null &&
                synonyms.TryGetValue(symbol, out var s) &&
                s != null)
            {
                foreach (var c in s.SelectMany(x => EnumerateSymbolVariations(x, caseSensitive)))
                    yield return c;
            }
        }

        static IEnumerable<char> EnumerateSymbolVariations(char symbol, bool caseSensitive)
        {
            if (caseSensitive)
            {
                yield return symbol;
            }
            else
            {
                char c1 = char.ToLowerInvariant(symbol);
                yield return c1;

                char c2 = char.ToUpperInvariant(symbol);
                if (c2 != c1)
                    yield return c2;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string m_Symbols;

        const int LookupTableSize = 80;
        const char LookupTableBaseSymbol = '0';

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly byte[]? m_LookupTable;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Dictionary<char, int>? m_LookupDictionary;

        /// <summary>
        /// Gets the size of this alphabet.
        /// The value is equivalent to radix, which is the number of unique symbols in positional numeral system of the encoding.
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
            var lookupTable = m_LookupTable;
            if (lookupTable != null)
            {
                int key = symbol - LookupTableBaseSymbol;
                if (key < 0 || key >= LookupTableSize)
                    return -1;

                byte index = lookupTable[key];
                if (index == 0xff)
                    return -1;

                return index;
            }

            var lookupDictionary = m_LookupDictionary;
            if (lookupDictionary != null)
            {
                if (lookupDictionary.TryGetValue(symbol, out var index))
                    return index;
                return -1;
            }

            return Symbols.IndexOf(symbol);
        }
    }
}
