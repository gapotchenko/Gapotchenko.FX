﻿using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Linq
{
    sealed class ReadOnlyCharList : IReadOnlyList<char>
    {
        public ReadOnlyCharList(string source)
        {
            m_Source = source;
        }

        readonly string m_Source;

        public char this[int index] => m_Source[index];

        public int Count => m_Source.Length;

        public IEnumerator<char> GetEnumerator() => m_Source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Source).GetEnumerator();
    }
}
