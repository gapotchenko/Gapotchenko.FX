using System.Collections.Generic;
using System.Linq.Expressions;

namespace Gapotchenko.FX.Linq.Expressions
{
    sealed class ParameterExpressionRegistry
    {
        private readonly List<ParameterExpression> _Indeces = new List<ParameterExpression>();

        public int GetIndex(ParameterExpression parameter)
        {
            int num = _Indeces.IndexOf(parameter);
            if (num >= 0)
                return num;
            _Indeces.Add(parameter);
            return _Indeces.Count - 1;
        }

        public void AddRange(IEnumerable<ParameterExpression> parameters) => _Indeces.AddRange(parameters);
    }
}
