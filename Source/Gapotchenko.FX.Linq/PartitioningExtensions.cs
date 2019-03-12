using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    /// <summary>
    /// Partitioning extensions.
    /// </summary>
    public static class PartitioningExtensions
    {
        sealed class PartinionedGrouping<TKey, TElement> : Collection<TElement>, IGrouping<TKey, TElement>
        {
            public PartinionedGrouping(TKey key)
            {
                Key = key;
            }

            public TKey Key
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Partition a sequence into groups by the given key preserving the order of elements.
        /// </summary>
        /// <typeparam name="TElement">The type of sequence element.</typeparam>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The sequence of groups.</returns>
        public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            PartinionedGrouping<TKey, TElement> partition = null;

            foreach (var x in source)
            {
                if (partition != null)
                {
                    var newKey = keySelector(x);
                    if (!EqualityComparer<TKey>.Default.Equals(newKey, partition.Key))
                    {
                        yield return partition;
                        partition = null;
                    }
                }

                if (partition == null)
                    partition = new PartinionedGrouping<TKey, TElement>(keySelector(x));

                partition.Add(x);
            }

            if (partition != null)
                yield return partition;
        }
    }
}
