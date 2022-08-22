using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Expressions
{
    /// <summary>
    /// 静态对象表达式
    /// </summary>
    public class StaticMemberExpression : Expression
    {
        private Type _type;

        public override Type Type => _type;

        public StaticMemberExpression(Type type)
        {
            this._type = type;
        }
    }
}
