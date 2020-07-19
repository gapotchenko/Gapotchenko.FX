using System;

#nullable enable

namespace Gapotchenko.FX.Console
{
    /// <summary>
    /// Defines virtual terminal processing states.
    /// </summary>
    public enum VirtualTerminalProcessingState
    {
        /// <summary>
        /// Virtual terminal processing is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// Virtual terminal processing is provided by the host platform.
        /// </summary>
        Platform,

        /// <summary>
        /// Virtual terminal processing is provided by emulation.
        /// </summary>
        Emulation
    }
}
