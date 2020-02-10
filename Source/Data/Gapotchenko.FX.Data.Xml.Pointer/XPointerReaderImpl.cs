using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

#nullable enable

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    sealed class XPointerReaderImpl : XPointerReader, IXmlLineInfo
    {
        public XPointerReaderImpl(TextReader input, XPointerReaderSettings settings)
        {
            m_Input = input;

            m_CloseInput = settings.CloseInput;
            m_IgnoreWhitespace = settings.IgnoreWhitespace;
            m_MaxCharactersInDocument = settings.MaxCharactersInDocument;

            m_LineNumber = 1;
            ResetLinePosition();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly TextReader m_Input;

        #region Settings

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly bool m_CloseInput;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly bool m_IgnoreWhitespace;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly int m_MaxCharactersInDocument;

        #endregion

        public override void Close()
        {
            if (m_ReadState == XPointerReadState.Closed)
                return;
            if (m_CloseInput)
                m_Input?.Close();
            ResetToken();
            m_ReadState = XPointerReadState.Closed;
        }

        #region State

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_NumberOfCharactersInDocument;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_LineNumber;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_LinePosition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool m_EOF;

        public override bool EOF => m_EOF;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        XPointerReadState m_ReadState;

        public override XPointerReadState ReadState => m_ReadState;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        XPointerTokenType m_TokenType;

        public override XPointerTokenType TokenType => m_TokenType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? m_Value;

        public override string? Value => m_Value;

        void ResetToken()
        {
            m_TokenType = XPointerTokenType.None;
            m_Value = null;
        }

        bool IXmlLineInfo.HasLineInfo() => true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_TokenLineNumber;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_TokenLinePosition;

        public int LineNumber => m_TokenLineNumber;
        public int LinePosition => m_TokenLinePosition;

        void SetTokenLineInfo()
        {
            m_TokenLineNumber = m_LineNumber;
            m_TokenLinePosition = m_LinePosition;
        }

        const int LineStartPosition = 0;

        void ResetLinePosition()
        {
            m_LinePosition = LineStartPosition;
        }

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        char? m_ReturnedChar;

        bool TryReadChar(out char ch)
        {
            if (m_ReturnedChar.HasValue)
            {
                ch = m_ReturnedChar.Value;
                m_ReturnedChar = null;
                return true;
            }

            int c = m_Input.Read();
            if (c == -1)
            {
                m_EOF = true;
                ch = default;
                return false;
            }

            if (m_MaxCharactersInDocument != 0)
            {
                ++m_NumberOfCharactersInDocument;
                if (m_NumberOfCharactersInDocument > m_MaxCharactersInDocument)
                    throw CreatePositionalXPointerException("Reached maximum allowed number of characters in XPointer document.");
            }

            ch = (char)c;
            ++m_LinePosition;

            return true;
        }

        void ReturnChar(char ch)
        {
            if (m_ReturnedChar.HasValue)
                throw new InvalidOperationException();
            m_ReturnedChar = ch;
        }

        public override bool Read()
        {
            try
            {
                if (ReadCore())
                    return true;
            }
            catch
            {
                m_ReadState = XPointerReadState.Error;
                throw;
            }

            if (m_EOF && m_ReadState == XPointerReadState.Interactive)
                m_ReadState = XPointerReadState.EndOfFile;
            ResetToken();

            return false;
        }

        bool ReadCore()
        {
            if (m_ReadState != XPointerReadState.Interactive)
                return false;

            Again:
            if (!TryReadChar(out var ch))
                return false;

            // TODO

            if (XmlConvert.IsWhitespaceChar(ch))
            {
                _ReadWhitespace(ch);
                if (m_IgnoreWhitespace)
                    goto Again;
                return true;
            }

            //if (ReadPrimitive(ch))
            //    return true;

            throw CreatePositionalXPointerException(string.Format("Unexpected character '{0}'.", ch));
        }

        void _ReadWhitespace(char firstChar)
        {
            SetTokenLineInfo();

            var sb = m_IgnoreWhitespace ? null : new StringBuilder();

            InterpretWhitespaceChar(firstChar);
            sb?.Append(firstChar);

            for (; ; )
            {
                if (!TryReadChar(out var ch))
                    break;

                if (!XmlConvert.IsWhitespaceChar(ch))
                {
                    ReturnChar(ch);
                    break;
                }

                InterpretWhitespaceChar(ch);
                sb?.Append(ch);
            }

            if (sb != null)
            {
                m_Value = sb.ToString();
                m_TokenType = XPointerTokenType.Whitespace;
            }
        }

        void InterpretWhitespaceChar(char ch)
        {
            switch (ch)
            {
                case '\n':
                    ++m_LineNumber;
                    ResetLinePosition();
                    break;
            }
        }

        XPointerException CreatePositionalXPointerException(string message) => new XPointerException(message, null, m_LineNumber, m_LinePosition);
    }
}
