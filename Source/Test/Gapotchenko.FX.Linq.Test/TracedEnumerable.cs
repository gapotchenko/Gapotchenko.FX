using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Linq.Test
{
    sealed class TracedEnumerable<T> : IEnumerable<T>
    {
        public TracedEnumerable(IEnumerable<T> underlyingEnumerable)
        {
            _UnderlyingEnumerable = underlyingEnumerable;
        }

        readonly IEnumerable<T> _UnderlyingEnumerable;

        public bool EnumeratorRetrieved
        {
            get;
            private set;
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnumeratorRetrieved = true;
            return _UnderlyingEnumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnumeratorRetrieved = true;
            return ((IEnumerable)_UnderlyingEnumerable).GetEnumerator();
        }
    }
}
