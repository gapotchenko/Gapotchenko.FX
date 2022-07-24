namespace Gapotchenko.FX.Console;

/// <summary>
/// Specifies virtual terminal processing preference.
/// </summary>
public enum VirtualTerminalProcessingMode
{
    /// <summary>
    /// If possible, virtual terminal processing is provided by the host platform; otherwise virtual terminal is emulated.
    /// </summary>
    Auto,

    /// <summary>
    /// Virtual terminal processing is provided only via the emulation.
    /// </summary>
    Emulation
}
