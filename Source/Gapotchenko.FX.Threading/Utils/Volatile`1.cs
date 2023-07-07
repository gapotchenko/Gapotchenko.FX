using System.Diagnostics;

namespace Gapotchenko.FX.Threading.Utils;

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
