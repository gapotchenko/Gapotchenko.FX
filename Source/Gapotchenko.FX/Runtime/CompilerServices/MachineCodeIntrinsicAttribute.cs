using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Runtime.CompilerServices
{
    /// <summary>
    /// Defines machine code intrinsic for a specified architecture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    sealed class MachineCodeIntrinsicAttribute : Attribute
    {
        public MachineCodeIntrinsicAttribute(Architecture architecture, params byte[] code)
        {
            Architecture = architecture;
            Code = code;
        }

        public Architecture Architecture { get; }

        public byte[] Code { get; }
    }
}
