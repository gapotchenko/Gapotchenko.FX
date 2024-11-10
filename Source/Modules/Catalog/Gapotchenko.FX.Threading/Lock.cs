// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

#if NET9_0_OR_GREATER
#define TFF_THREADING_LOCK
#endif

using System.Runtime.CompilerServices;

#if !TFF_THREADING_LOCK

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement.

namespace System.Threading;

/// <summary>
/// Provides a way to get mutual exclusion in regions of code between different threads.
/// A lock may be held by one thread at a time.
/// </summary>
/// <remarks>
/// <para>
/// Threads that cannot immediately enter the lock may wait for the lock to be exited or until a specified timeout.
/// A thread that holds a lock may enter the lock repeatedly without exiting it, such as recursively, in which case
/// the thread should eventually exit the lock the same number of times to fully exit the lock
/// and allow other threads to enter the lock.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </remarks>
public sealed class Lock
{
    /// <summary>
    /// Enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <remarks>
    /// When the method returns, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered.
    /// If the lock is already held by the current thread, the lock is entered again.
    /// To fully exit the lock and allow other threads to enter the lock,
    /// the current thread should exit the lock as many times as it has entered the lock.
    /// </remarks>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    public void Enter() => Monitor.Enter(this);

    /// <summary>
    /// Enters the lock, waiting if necessary until the lock can be entered.
    /// </summary>
    /// <returns>
    /// A <see cref="Scope"/> that can be disposed to exit the lock.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If the lock can't be entered immediately, the method waits until the lock can be entered.
    /// If the lock is already held by the current thread, the lock is entered again.
    /// To fully exit the lock and allow other threads to enter the lock,
    /// the current thread should dispose the returned <see cref="Scope"/> to exit the lock as many times as it has entered the lock.
    /// </para>
    /// <para>
    /// This method is intended to be used with a language construct that automatically disposes the <see cref="Scope"/>,
    /// such as the C# <see langword="using"/> keyword.
    /// </para>
    /// </remarks>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    public Scope EnterScope()
    {
        Enter();
        return new Scope(this);
    }

    /// <summary>
    /// Represents a <see cref="Lock"/> that might have been entered.
    /// </summary>
    /// <remarks>
    /// This type is intended to be used with the <see cref="EnterScope"/> method,
    /// which returns a <see cref="Scope"/> representing the lock that was entered,
    /// and with a language construct that automatically disposes the <see cref="Scope"/>,
    /// such as the C# <see langword="using"/> keyword.
    /// Disposing the <see cref="Scope"/> exits the lock.
    /// Ensure that <see cref="Dispose"/> is called even in case of exceptions.
    /// </remarks>
    public ref struct Scope
    {
        internal Scope(Lock lockObj)
        {
            m_LockObj = lockObj;
        }

        /// <summary>
        /// Exits the lock if the <see cref="Scope"/> represents a lock that was entered.
        /// </summary>
        /// <remarks>
        /// If the current thread holds the lock multiple times, such as recursively, the lock is exited only once.
        /// The current thread should ensure that each <see cref="EnterScope"/> is matched with a <see cref="Dispose"/> on the <see cref="Scope"/> returned by <see cref="EnterScope"/>.
        /// </remarks>
        /// <exception cref="SynchronizationLockException">
        /// The <see cref="Scope"/> represents a lock that was entered and the current thread does not hold the lock.
        /// </exception>
        public void Dispose()
        {
            var lockObj = m_LockObj;
            if (lockObj != null)
            {
                m_LockObj = null;
                lockObj.Exit();
            }
        }

        Lock? m_LockObj;
    }

    /// <summary>
    /// Tries to enter the lock without waiting.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current thread; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method returns <see langword="false"/> without waiting for the lock.
    /// If the lock is already held by the current thread, the lock is entered again.
    /// To fully exit the lock and allow other threads to enter the lock,
    /// the current thread should exit the lock as many times as it has entered the lock.
    /// </remarks>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    public bool TryEnter() => Monitor.TryEnter(this);

    /// <summary>
    /// Tries to enter the lock, waiting if necessary for the specified number of milliseconds until the lock can be entered.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait until the lock can be entered. 
    /// Specify <see cref="Timeout.Infinite"/> (<c>-1</c>) to wait indefinitely, or <c>0</c> to not wait.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current thread; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or
    /// until the timeout specified by the <paramref name="millisecondsTimeout"/> parameter expires.
    /// If the timeout expires before entering the lock, the method returns <see langword="false"/>.
    /// If the lock is already held by the current thread, the lock is entered again.
    /// To fully exit the lock and allow other threads to enter the lock,
    /// the current thread should exit the lock as many times as it has entered the lock.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="millisecondsTimeout"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    public bool TryEnter(int millisecondsTimeout) => Monitor.TryEnter(this, millisecondsTimeout);

    /// <summary>
    /// Tries to enter the lock, waiting if necessary until the lock can be entered or until the specified timeout expires.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait until the lock can be entered.
    /// Specify a value that represents <see cref="Timeout.Infinite"/> (<c>-1</c>) milliseconds to wait indefinitely,
    /// or a value that represents <c>0</c> milliseconds to not wait.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the lock was entered by the current thread; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// When the method returns <see langword="true"/>, the current thread is the only thread that holds the lock.
    /// If the lock can't be entered immediately, the method waits until the lock can be entered or until the specified timeout expires.
    /// If the timeout expires before entering the lock, the method returns <see langword="false"/>.
    /// If the lock is already held by the current thread, the lock is entered again.
    /// To fully exit the lock and allow other threads to enter the lock,
    /// the current thread should exit the lock as many times as it has entered the lock.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="timeout"/>, after its conversion to an integer millisecond value, represents a value that is less
    /// than <c>-1</c> milliseconds or greater than <see cref="int.MaxValue"/> milliseconds.
    /// </exception>
    /// <exception cref="LockRecursionException">
    /// The lock has reached the limit of repeated entries by the current thread.
    /// The limit is implementation-defined and is intended to be high enough that it would not be reached in normal situations.
    /// </exception>
    public bool TryEnter(TimeSpan timeout) => Monitor.TryEnter(this, timeout);

    /// <summary>
    /// Exits the lock.
    /// </summary>
    /// <remarks>
    /// If the current thread holds the lock multiple times, such as recursively, the lock is exited only once.
    /// The current thread should ensure that each enter is matched with an exit.
    /// </remarks>
    /// <exception cref="SynchronizationLockException">
    /// The current thread does not hold the lock.
    /// </exception>
    public void Exit() => Monitor.Exit(this);

    /// <summary>
    /// Gets a value that indicates whether the lock is held by the current thread.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current thread holds the lock; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsHeldByCurrentThread => Monitor.IsEntered(this);
}

#else

[assembly: TypeForwardedTo(typeof(Lock))]

#endif
