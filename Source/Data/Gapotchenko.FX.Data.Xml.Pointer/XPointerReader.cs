using Gapotchenko.FX.ComponentModel;
using System;
using System.IO;

#nullable enable

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    public abstract class XPointerReader : DisposableBase
    {
        /// <summary>
        /// Creates a new <see cref="XPointerReader"/> instance by using the specified text reader and settings.
        /// </summary>
        /// <param name="input">The text reader from which to read the XML pointer data.</param>
        /// <param name="settings">
        /// The settings for the new <see cref="XPointerReader"/>.
        /// This value can be null.
        /// </param>
        /// <returns>An object that is used to read the XML pointer data in the stream.</returns>
        public static XPointerReader Create(TextReader input, XPointerReaderSettings? settings)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (settings == null)
            {
                if (m_DefaultSettings == null)
                    m_DefaultSettings = new XPointerReaderSettings();
                settings = m_DefaultSettings;
            }

            return new XPointerReaderImpl(input, settings);
        }

        static XPointerReaderSettings? m_DefaultSettings;

        /// <summary>
        /// Creates a new <see cref="XPointerReader"/> instance by using the specified text reader.
        /// </summary>
        /// <param name="input">The text reader from which to read the XML pointer data.</param>
        /// <returns>An object that is used to read the XML pointer data in the stream.</returns>
        public static XPointerReader Create(TextReader input) => Create(input, null);

        /// <summary>
        /// Creates a new <see cref="XPointerReader"/> instance by using the specified input string.
        /// </summary>
        /// <param name="input">The string from which to read the XML pointer data.</param>
        /// <returns>An object that is used to read the XML pointer data in the stream.</returns>
        public static XPointerReader Create(string input) => Create(new StringReader(input ?? throw new ArgumentNullException(nameof(input))));

        protected override void Dispose(bool disposing)
        {
            if (disposing && ReadState != XPointerReadState.Closed)
                Close();
            base.Dispose(disposing);
        }

        public abstract XPointerReadState ReadState { get; }

        public abstract bool Read();

        /// <summary>
        /// Returns <c>true</c> when the <see cref="XPointerReader"/> is positioned at the end of the stream.
        /// </summary>
        public abstract bool EOF { get; }

        /// <summary>
        /// Closes the underlying storage medium if <see cref="XPointerReaderSettings.CloseInput"/> is <c>true</c>,
        /// changes the <see cref="ReadState"/> to <see cref="XPointerReadState.Closed"/>,
        /// and sets all value properties back to default values.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Gets the text value of the current token.
        /// </summary>
        public abstract string? Value { get; }

        /// <summary>
        /// Gets the type of the current token.
        /// </summary>
        public abstract XPointerTokenType TokenType { get; }
    }
}
