﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Marks derived exception as the one affecting a program's control flow.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public interface IControlFlowException
    {
    }
}
