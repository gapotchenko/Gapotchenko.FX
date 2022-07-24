using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions;

sealed class ParameterExpressionRegistry
{
    readonly List<ParameterExpression> m_Indeces = new List<ParameterExpression>();

    public int GetIndex(ParameterExpression parameter)
    {
        int index = m_Indeces.IndexOf(parameter);
        if (index >= 0)
            return index;

        m_Indeces.Add(parameter);
        return m_Indeces.Count - 1;
    }

    public void AddRange(IEnumerable<ParameterExpression> parameters) => m_Indeces.AddRange(parameters);
}
