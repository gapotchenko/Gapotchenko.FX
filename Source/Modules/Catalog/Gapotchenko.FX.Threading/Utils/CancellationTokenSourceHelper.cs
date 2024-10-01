using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading.Utils;

static class CancellationTokenSourceHelper
{
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static CancellationTokenSource CreateLinked(CancellationToken cancellationToken)
    {
        return
#if NET5_0_OR_GREATER
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
#else
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, default);
#endif
    }

    public static CancellationTokenSource CreateLinked(CancellationToken cancellationToken, TimeSpan timeout)
    {
        if (cancellationToken.CanBeCanceled)
        {
            var cts = CreateLinked(cancellationToken);
            if (timeout != Timeout.InfiniteTimeSpan)
                cts.CancelAfter(timeout);
            return cts;
        }
        else
        {
            if (timeout != Timeout.InfiniteTimeSpan)
                return new CancellationTokenSource(timeout);
            else
                return new CancellationTokenSource();
        }
    }
}
