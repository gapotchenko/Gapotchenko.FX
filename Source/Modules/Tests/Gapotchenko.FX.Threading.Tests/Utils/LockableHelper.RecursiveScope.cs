using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Tests.Utils;

partial class LockableHelper
{
    public readonly struct RecursiveScope : ILockableScope
    {
        public static RecursiveScope Enter(
            ILockable lockable,
            int recursionLevel,
            CancellationToken cancellationToken = default)
        {
            ValidateRecursion(lockable, recursionLevel);

            if (recursionLevel > 1 && lockable is IReentrableLockable reentrableLockable)
            {
                reentrableLockable.Enter(recursionLevel, cancellationToken);
            }
            else
            {
                int currentRecursionLevel = 0;
                try
                {
                    while (currentRecursionLevel < recursionLevel)
                    {
                        lockable.Enter(cancellationToken);
                        ++currentRecursionLevel;
                    }
                }
                catch
                {
                    Exit(lockable, currentRecursionLevel);
                    throw;
                }
            }

            return new(lockable, recursionLevel);
        }

        public static Task<RecursiveScope> EnterAsync(
            IAsyncLockable lockable,
            int recursionLevel,
            CancellationToken cancellationToken = default)
        {
            ValidateRecursion(lockable, recursionLevel);

            return recursionLevel switch
            {
                > 1 when lockable is IAsyncReentrableLockable reentrableLockable =>
                    reentrableLockable.EnterAsync(recursionLevel, cancellationToken)
                    .Then(() => new RecursiveScope(reentrableLockable, recursionLevel)),

                0 => Task.FromResult(new RecursiveScope(lockable, 0)),

                1 =>
                    lockable.EnterAsync(cancellationToken)
                    .Then(() => new RecursiveScope(lockable, recursionLevel)),

                _ => throw new NotImplementedException("Is there a way to correctly implement this?")
            };
        }

        [StackTraceHidden]
        static void ValidateRecursion(ILockable lockable, int recursionLevel)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(lockable);
            ArgumentOutOfRangeException.ThrowIfNegative(recursionLevel);
#endif

            if (recursionLevel > 1 && !lockable.IsRecursive)
                throw new ArgumentException("The synchronization primitive does not support recursion.", nameof(lockable));
        }

        RecursiveScope(ILockable lockable, int recursionLevel)
        {
            m_Lockable = lockable;
            m_RecursionLevel = recursionLevel;
        }

        public bool HasLock => m_RecursionLevel > 0;

        public void Dispose()
        {
            Exit(m_Lockable, m_RecursionLevel);
        }

        readonly ILockable m_Lockable;
        readonly int m_RecursionLevel;
    }
}
