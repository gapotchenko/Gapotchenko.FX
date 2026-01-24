// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Linq.Expressions;
using System.Reflection;

namespace Gapotchenko.FX.Math.Intervals.Utils;

static class ReflectionHelper
{
    public static MethodInfo MethodOf<T>(Expression<Func<T>> expression) => ((MethodCallExpression)expression.Body).Method;
}

readonly struct MethodOf<TFunc> where TFunc : Delegate
{
    MethodOf(TFunc func)
    {
        m_Method = func.Method;
    }

    public static implicit operator MethodOf<TFunc>(TFunc func)
    {
        ArgumentNullException.ThrowIfNull(func);

        return new MethodOf<TFunc>(func);
    }

    public static implicit operator MethodInfo(MethodOf<TFunc> methodOf) => methodOf.m_Method;

    readonly MethodInfo m_Method;
}
