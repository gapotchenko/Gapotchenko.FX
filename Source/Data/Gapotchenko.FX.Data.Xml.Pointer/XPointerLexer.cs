using System;
using System.Xml;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// XPointer lexical analyzer.
    /// </summary>
    internal class XPointerLexer
    {
        private string _ptr;
        private int _ptrIndex;
        private LexKind _kind;
        private char _currChar;
        private int _number;
        private string _ncname;
        private string _prefix;
        private bool _canBeSchemaName;

        public XPointerLexer(string p)
        {
            if (p == null)
                throw new ArgumentNullException("pointer", "XPointer pointer cannot be null");
            _ptr = p;
            NextChar();
        }

        public bool NextChar()
        {
            if (_ptrIndex < _ptr.Length)
            {
                _currChar = _ptr[_ptrIndex++];
                return true;
            }
            else
            {
                _currChar = '\0';
                return false;
            }
        }

        public LexKind Kind
        {
            get { return _kind; }
        }

        public int Number
        {
            get { return _number; }
        }

        public string NCName
        {
            get { return _ncname; }
        }

        public string Prefix
        {
            get { return _prefix; }
        }

        public bool CanBeSchemaName
        {
            get { return _canBeSchemaName; }
        }

        public void SkipWhiteSpace()
        {
            while (LexUtils.IsWhitespace(_currChar))
                NextChar();
        }

        public bool NextLexeme()
        {
            switch (_currChar)
            {
                case '\0':
                    _kind = LexKind.Eof;
                    return false;
                case '(':
                case ')':
                case '=':
                case '/':
                    _kind = (LexKind)Convert.ToInt32(_currChar);
                    NextChar();
                    break;
                case '^':
                    NextChar();
                    if (_currChar == '^' || _currChar == '(' || _currChar == ')')
                    {
                        _kind = LexKind.EscapedData;
                        NextChar();
                    }
                    else
                        throw new XPointerSyntaxException("Circumflex character must be escaped");
                    break;
                default:
                    if (Char.IsDigit(_currChar))
                    {
                        _kind = LexKind.Number;
                        int start = _ptrIndex - 1;
                        int len = 0;
                        while (Char.IsDigit(_currChar))
                        {
                            NextChar(); len++;
                        }
                        _number = XmlConvert.ToInt32(_ptr.Substring(start, len));
                        break;
                    }
                    else if (LexUtils.IsStartNameChar(_currChar))
                    {
                        _kind = LexKind.NCName;
                        _prefix = String.Empty;
                        _ncname = ParseName();
                        if (_currChar == ':')
                        {
                            //QName?
                            NextChar();
                            _prefix = _ncname;
                            _kind = LexKind.QName;
                            if (LexUtils.IsStartNCNameChar(_currChar))
                                _ncname = ParseName();
                            else
                                throw new XPointerSyntaxException("Wrong Name token: " + _prefix + ":" + _currChar);
                        }
                        _canBeSchemaName = _currChar == '(';
                        break;
                    }
                    else if (LexUtils.IsWhitespace(_currChar))
                    {
                        _kind = LexKind.Space;
                        while (LexUtils.IsWhitespace(_currChar))
                            NextChar();
                        break;
                    }
                    else
                    {
                        _kind = LexKind.EscapedData;
                        break;
                    }
            }
            return true;
        }

        private string ParseName()
        {
            int start = _ptrIndex - 1;
            int len = 0;
            while (LexUtils.IsNCNameChar(_currChar))
            {
                NextChar(); len++;
            }
            return _ptr.Substring(start, len);
        }

        public string ParseEscapedData()
        {
            int depth = 0;
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                switch (_currChar)
                {
                    case '^':
                        if (!NextChar())
                            throw new XPointerSyntaxException("Unexpected end of schema data");
                        else if (_currChar == '^' || _currChar == '(' || _currChar == ')')
                            sb.Append(_currChar);
                        else
                            throw new XPointerSyntaxException("Circumflex character must be escaped");
                        break;
                    case '(':
                        depth++;
                        goto default;
                    case ')':
                        if (depth-- == 0)
                        {
                            //Skip ')'
                            NextLexeme();
                            return sb.ToString();
                        }
                        else
                            goto default;
                    default:
                        sb.Append(_currChar);
                        break;
                }
                if (!NextChar())
                    throw new XPointerSyntaxException("Unexpected end of schema data");
            }
        }

        public enum LexKind
        {
            NCName = 'N',
            QName = 'Q',
            LRBracket = '(',
            RRBracket = ')',
            Circumflex = '^',
            Number = 'd',
            Eq = '=',
            Space = 'S',
            Slash = '/',
            EscapedData = 'D',
            Eof = 'E'
        }

    }
}
