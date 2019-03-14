using System;
using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Linq
{
    sealed class ReadOnlyCharList : IReadOnlyList<char>
    {
        public ReadOnlyCharList(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            _S = s;
        }

        readonly string _S;

        public char this[int index] => _S[index];

        public int Count => _S.Length;

        public IEnumerator<char> GetEnumerator() => _S.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_S).GetEnumerator();
    }
}
