// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Threading;

static class AsyncMonitorObjectTable
{
    public sealed class Descriptor
    {
        readonly AsyncCriticalSection m_Mutex = new();
        readonly AsyncConditionVariableImpl m_ConditionVariable = new();

        // ------------------------------------------

        public AsyncMonitor Monitor => m_CachedMonitor ?? GetMonitorRare();

        AsyncMonitor GetMonitorRare()
        {
            lock (this)
                return m_CachedMonitor ??= new(m_Mutex, m_ConditionVariable);
        }

        AsyncMonitor? m_CachedMonitor;

        // ------------------------------------------

        public AsyncRecursiveMonitor RecursiveMonitor => m_CachedRecursiveMonitor ?? GetRecursiveMonitorRare();

        AsyncRecursiveMonitor GetRecursiveMonitorRare()
        {
            lock (this)
                return m_CachedRecursiveMonitor ??= new(new AsyncRecursiveLockableImpl<IAsyncLockable>(m_Mutex), m_ConditionVariable);
        }

        AsyncRecursiveMonitor? m_CachedRecursiveMonitor;
    }

    static readonly ConditionalWeakTable<object, Descriptor> m_Descriptors = new();

    public static Descriptor GetDescriptor(object obj)
    {
        Debug.Assert(obj is not null);

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
}
