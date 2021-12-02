using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    interface ISyntaxSlotProvider
    {
        abstract int SlotCount { get; }
        abstract SyntaxSlot GetSlot(int i);
    }
}
