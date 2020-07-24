using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// Specifies a set of features to support on the <see cref="XPointerReader"/> object.
    /// </summary>
    public sealed class XPointerReaderSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to close an input <see cref="TextReader"/> when <see cref="XPointerReader"/> is closed.
        /// </summary>
        public bool CloseInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore insignificant white space.
        /// </summary>
        /// <value>
        /// <c>true</c> to ignore white space; otherwise <c>false</c>.
        /// The default is <c>false</c>.
        /// </value>
        /// <remarks>
        /// White space that is not considered to be significant includes spaces, tabs, and blank lines used to set apart the markup for greater readability.
        /// </remarks>
        public bool IgnoreWhitespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the maximum allowable number of characters in a XPointer document.
        /// A zero (0) value means no limits on the size of the XPointer document. A non-zero value specifies the maximum size, in characters.
        /// </summary>
        /// <value>
        /// The maximum number of characters in document.
        /// The default is zero (0).
        /// </value>
        /// <remarks>
        /// <para>
        /// A zero (0) value means no limits on the number of characters in the parsed document.
        /// A non-zero value specifies the maximum number of characters that can be parsed.
        /// </para>
        /// <para>
        /// If the reader attempts to read a document with a size that exceeds this property, a <see cref="XPointerException"/> will be thrown.
        /// </para>
        /// <para>
        /// This property allows you to mitigate denial of service attacks where the attacker submits extremely large XPointer documents.
        /// By limiting the size of a document, you can detect the attack and recover reliably.
        /// </para>
        /// </remarks>
        public int MaxCharactersInDocument { get; set; }
    }
}
