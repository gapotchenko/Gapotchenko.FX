using Gapotchenko.FX.Data.Xml.Pointer.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer.Serialization
{
    /// <summary>
    /// XML pointer serializer.
    /// </summary>
    public sealed class XPointerSerializer
    {
        /// <summary>
        /// Serializes XML pointer to text.
        /// </summary>
        /// <param name="writer">The text writer.</param>
        /// <param name="pointer">The XML pointer.</param>
        public void Serialize(TextWriter writer, XPointer pointer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (pointer == null)
                throw new ArgumentNullException(nameof(pointer));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes XML pointer to string.
        /// </summary>
        /// <param name="pointer">The XML pointer.</param>
        public string Serialize(XPointer pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException(nameof(pointer));

            var writer = new StringWriter();
            Serialize(writer, pointer);
            return writer.ToString();
        }

        /// <summary>
        /// Deserializes XML pointer from text.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        /// <returns>A deserialized XML pointer.</returns>
        public XPointer Deserialize(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes XML pointer from string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A deserialized XML pointer.</returns>
        public XPointer Deserialize(string s) => Deserialize(new StringReader(s ?? throw new ArgumentNullException(nameof(s))));
    }
}
