namespace Gapotchenko.FX;

sealed class OptionalComparer<T>(IComparer<T>? valueComparer) : IComparer<Optional<T>>
{
    public int Compare(Optional<T> x, Optional<T> y) => CompareCore(x, y, m_ValueComparer);

    readonly IComparer<T> m_ValueComparer = valueComparer ?? Comparer<T>.Default;

    internal static int CompareCore(Optional<T> x, Optional<T> y, IComparer<T> valueComparer)
    {
        if (x.HasValue)
        {
            if (y.HasValue)
                return valueComparer.Compare(x.Value, y.Value);
            else
                return 1;
        }
        else if (y.HasValue)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
