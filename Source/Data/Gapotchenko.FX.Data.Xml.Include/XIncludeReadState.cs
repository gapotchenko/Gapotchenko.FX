using System;
using System.Collections.Generic;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Include
{
    enum XIncludeReadState
    {
        //Default state
        Default,
        //xml:base attribute is being exposed
        ExposingXmlBaseAttr,
        //xml:base attribute value is being exposed
        ExposingXmlBaseAttrValue
    }
}
