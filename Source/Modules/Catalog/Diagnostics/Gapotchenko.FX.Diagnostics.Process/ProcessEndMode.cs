namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Describes the possible modes of ending a process.
/// </summary>
[Flags]
public enum ProcessEndMode
{
    /// <summary>
    /// No process shutdown.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// A process shutdown by a SIGINT signal (CTRL+C).
    /// </summary>
    Interrupt = 0x01 | Graceful,

    /// <summary>
    /// A process shutdown by closing user interface elements if there are any.
    /// For example, if the process has a main window then it is ended by sending a corresponding window close message.
    /// </summary>
    Close = 0x02 | Graceful,

    /// <summary>
    /// A process shutdown by a SIGKILL signal (process kill).
    /// </summary>
    Kill = 0x04 | Forceful,

    /// <summary>
    /// A process shutdown by issuing an exit request. Applies to a current process only.
    /// </summary>
    Exit = 0x08 | Forceful,

    /// <summary>
    /// A graceful process shutdown.
    /// </summary>
    Graceful = 0x10000,

    /// <summary>
    /// A forceful process shutdown.
    /// </summary>
    Forceful = 0x20000,

    /// <summary>
    /// Allows all process shutdown techniques.
    /// </summary>
    Complete = 0xffffff
}
