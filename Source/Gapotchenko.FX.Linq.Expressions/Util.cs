using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Linq.Expressions
{
    static class Util
    {
        public static IEnumerable<Pair<TLeft, TRight>> Zip<TLeft, TRight>(this IEnumerable<TLeft> ls, IEnumerable<TRight> rs)
        {
            if (ls == null)
                throw new ArgumentNullException(nameof(ls));
            if (rs == null)
                throw new ArgumentNullException(nameof(rs));

            using (var le = ls.GetEnumerator())
            using (var re = rs.GetEnumerator())
            {
                while (le.MoveNext() && re.MoveNext())
                    yield return new Pair<TLeft, TRight>(le.Current, re.Current);
            }
        }
    }
}
