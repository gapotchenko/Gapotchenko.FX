using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Defines machine code intrinsic for a specified processor architecture.
/// </summary>
[CLSCompliant(false)]
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
