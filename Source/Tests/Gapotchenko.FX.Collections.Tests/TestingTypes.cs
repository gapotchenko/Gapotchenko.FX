// (c) Portions of code from the .NET project by Microsoft and .NET Foundation

using System;
using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Collections.Tests;

#region TestClasses

[Serializable]
public struct SimpleInt : IStructuralComparable, IStructuralEquatable, IComparable, IComparable<SimpleInt>
{
    int _val;

    public SimpleInt(int t)
    {
        _val = t;
    }

    public int Val
    {
        get { return _val; }
        set { _val = value; }
    }

    public int CompareTo(SimpleInt other)
    {
        return other.Val - _val;
    }

    public int CompareTo(object? obj)
    {
        if (obj?.GetType() == typeof(SimpleInt))
        {
            return ((SimpleInt)obj).Val - _val;
        }
        return -1;
    }

    public int CompareTo(object? other, IComparer comparer)
    {
        if (other?.GetType() == typeof(SimpleInt))
            return ((SimpleInt)other).Val - _val;
        return -1;
    }

    public bool Equals(object? other, IEqualityComparer comparer)
    {
        if (other?.GetType() == typeof(SimpleInt))
            return ((SimpleInt)other).Val == _val;
        return false;
    }

    public int GetHashCode(IEqualityComparer comparer)
    {
        return comparer.GetHashCode(_val);
    }
}

[Serializable]
public class WrapStructural_SimpleInt : IEqualityComparer<SimpleInt>, IComparer<SimpleInt>
{
    public int Compare(SimpleInt x, SimpleInt y)
    {
        return StructuralComparisons.StructuralComparer.Compare(x, y);
    }

    public bool Equals(SimpleInt x, SimpleInt y)
    {
        return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
    }

    public int GetHashCode(SimpleInt obj)
    {
        return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
    }
}

public sealed class EqualityComparerConstantHashCode<T> : IEqualityComparer<T>
{
    readonly IEqualityComparer<T> _comparer;

    public EqualityComparerConstantHashCode(IEqualityComparer<T> comparer) => _comparer = comparer;

    public bool Equals(T? x, T? y) => _comparer.Equals(x, y);

    public int GetHashCode(T obj) => 42;
}

#endregion
