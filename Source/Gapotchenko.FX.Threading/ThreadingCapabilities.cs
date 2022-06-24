using System;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides information about multithreading capabilities.
/// </summary>
public static class ThreadingCapabilities
{
    const int LogicalProcessorCountRefreshIntervalMS = 30000;

    static volatile int m_LogicalProcessorCount;
    static volatile int m_LastLogicalProcessorCountRefreshTicks;

    /// <summary>
    /// Gets the number of logical processors available to the current process.
    /// </summary>
    public static int LogicalProcessorCount
    {
        get
        {
            var now = Environment.TickCount;

            if (m_LogicalProcessorCount == 0 ||
                now - m_LastLogicalProcessorCountRefreshTicks >= LogicalProcessorCountRefreshIntervalMS)
            {
                m_LogicalProcessorCount = Environment.ProcessorCount;
                m_LastLogicalProcessorCountRefreshTicks = now;
            }

            return m_LogicalProcessorCount;
        }
    }
}
