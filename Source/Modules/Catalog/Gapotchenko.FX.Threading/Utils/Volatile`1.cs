// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

/// <summary>
/// Volatile access modifier for a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of a value to provide the volatile access for.</typeparam>
[DebuggerDisplay("{Value}")]
struct Volatile<T>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T m_Value;

    public T Value
    {
        readonly get
        {
            var result = m_Value;
            Thread.MemoryBarrier();
            return result;
        }
        set
        {
            Thread.MemoryBarrier();
            m_Value = value;
        }
    }
}
