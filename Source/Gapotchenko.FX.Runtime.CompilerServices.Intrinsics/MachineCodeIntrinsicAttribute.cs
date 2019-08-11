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
    /// Defines machine code intrinsic for a specified processor architecture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class MachineCodeIntrinsicAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MachineCodeIntrinsicAttribute"/> class.
        /// </summary>
        /// <param name="architecture">The processor architecture.</param>
        /// <param name="code">The machine code.</param>
        public MachineCodeIntrinsicAttribute(Architecture architecture, params byte[] code)
        {
            Architecture = architecture;
            Code = code;
        }

        /// <summary>
        /// Gets processor architecture.
        /// </summary>
        public Architecture Architecture { get; }

        /// <summary>
        /// Gets machine code.
        /// </summary>
        public byte[] Code { get; }
    }
}
