using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    public enum XPointerTokenType
    {
        None,
        NCName,
        QName,
        LeftBracket = '(',
        RightBracket = ')',
        Whitespace,
        EscapedData
    }
}
