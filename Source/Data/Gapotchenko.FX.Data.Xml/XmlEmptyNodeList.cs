using System;
using System.Collections;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml
{
    /// <summary>
    /// Empty XML node list.
    /// </summary>
    public sealed class XmlEmptyNodeList : XmlNodeList
    {
        /// <summary>
        /// Retrieves a node at the given index.
        /// </summary>
        /// <param name="index">Zero-based index into the list of nodes.</param>
        /// <returns>
        /// The <see cref="XmlNode"/> in the collection.
        /// If index is greater than or equal to the number of nodes in the list, this returns <c>null</c>.
        /// </returns>
        public override XmlNode Item(int index) => throw new InvalidOperationException();

        /// <summary>
        /// Provides a simple "foreach" style iteration over the collection of nodes in the <see cref="XmlNodeList"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/>.</returns>
        public override IEnumerator GetEnumerator() => Enumerator.Instance;

        sealed class Enumerator : IEnumerator
        {
            public bool MoveNext() => false;
            public void Reset() { }
            public object Current => throw new InvalidOperationException();
            public static readonly Enumerator Instance = new Enumerator();
        }

        /// <summary>
        /// Gets the number of nodes in the <see cref="XmlNodeList"/>.
        /// </summary>
        public override int Count => 0;

        /// <summary>
        /// Gets the default instance of <see cref="XmlEmptyNodeList"/>.
        /// </summary>
        public static XmlEmptyNodeList Instance { get; } = new XmlEmptyNodeList();
    }
}
