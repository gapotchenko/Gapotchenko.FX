// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Security;

namespace Gapotchenko.FX.Threading.Utils;

static class ThreadHelper
{
    public static bool TryResetAbort()
    {
#if TFF_THREAD_ABORT
        try
        {
            Thread.ResetAbort();
            return true;
        }
        catch (ThreadStateException)
        {
            // Was not aborted by Thread.Abort().
        }
        catch (PlatformNotSupportedException)
        {
            // Not supported.
        }
#endif
        return false;
    }

    public static bool TryAbort(Thread thread)
    {
#if TFF_THREAD_ABORT
        try
        {
            thread.Abort();
            return true;
        }
        catch (ThreadStateException)
        {
            // Already aborted or no longer running.
        }
        catch (SecurityException)
        {
            // Not allowed.
        }
        catch (PlatformNotSupportedException)
        {
            // Not supported.
        }
#endif
        return false;
    }

    /// <summary>
    /// Tries to cancel the specified thread.
    /// </summary>
    /// <param name="thread">The thread to cancel.</param>
    public static bool TryCancel(Thread thread)
    {
#if TFF_THREAD_ABORT
        if (TryAbort(thread))
            return true;
#endif

        try
        {
            thread.Interrupt();
            return true;
        }
        catch (SecurityException)
        {
            // Not allowed.
        }

        return false;
    }
}
