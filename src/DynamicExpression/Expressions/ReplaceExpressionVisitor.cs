using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Expressions
{
    /// <summary>
    /// 替换表达式参数
    /// </summary>
    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private Dictionary<Expression, Expression> _parameters;

        public ReplaceExpressionVisitor(Dictionary<Expression,Expression> parameters)
        {
            _parameters = parameters;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_parameters.TryGetValue(node, out Expression _newValue))
            {
                return _newValue;
            }
            return base.Visit(node);
        }
    }
}
