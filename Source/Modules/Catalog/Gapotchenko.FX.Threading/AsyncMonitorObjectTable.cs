// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading;

static class AsyncMonitorObjectTable
{
    public static Descriptor GetDescriptor(object obj)
    {
        if (m_Descriptors.TryGetValue(obj, out var descriptor))
            return descriptor;
        else
            return GetDescriptorRare(obj);
    }

    static Descriptor GetDescriptorRare(object obj)
    {
        var descriptors = m_Descriptors;
        lock (descriptors)
        {
            if (descriptors.TryGetValue(obj, out var descriptor))
            {
                return descriptor;
            }
            else
            {
                descriptor = new Descriptor();
                descriptors.Add(obj, descriptor);
                return descriptor;
            }
        }
    }

    static readonly ConditionalWeakTable<object, Descriptor> m_Descriptors = new();

    public sealed class Descriptor
    {
#if PREVIEW

        public AsyncMonitorSlim SlimMonitor => m_CachedSlimMonitor ?? GetSlimMonitorRare();

        AsyncMonitorSlim GetSlimMonitorRare()
        {
            lock (this)
                return m_CachedSlimMonitor ??= new(m_Mutex, m_ConditionVariable);
        }

        AsyncMonitorSlim? m_CachedSlimMonitor;

#endif

        // ------------------------------------------

        public AsyncMonitor Monitor => m_CachedMonitor ?? GetMonitorRare();

        AsyncMonitor GetMonitorRare()
        {
            lock (this)
                return m_CachedMonitor ??= new(new AsyncRecursiveLockableImpl<IAsyncLockable>(m_Mutex), m_ConditionVariable);
        }

        AsyncMonitor? m_CachedMonitor;

        // ------------------------------------------

        readonly AsyncCriticalSection m_Mutex = new();
        readonly AsyncConditionVariableImpl m_ConditionVariable = new();
    }
}
