using DynamicExpression.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Extension
{
    /// <summary>
    /// 表达式扩展
    /// </summary>
    public static class ExpressionExtension
    {
        /// <summary>
        /// 使用AndAlso合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this IList<Expression<Func<T, bool>>> exprs)
        {
            //var parameter = Expression.Parameter(typeof(T));

            if (exprs.Count == 0) return null;
            if(exprs.Count==1) return exprs[0];

            var leftExpr = exprs[0];

            var parameter = leftExpr.Parameters[0];
            var left = leftExpr.Body;
            for (int i = 1; i < exprs.Count; i++)
            { 
                var expr = exprs[i];
                var visitor = new ReplaceExpressionVisitor(expr.Parameters[0], parameter);
                var right = visitor.Visit(expr.Body);
                left = Expression.AndAlso(left, right);
            }
            return Expression.Lambda<Func<T, bool>>(left, parameter);
        }

        /// <summary>
        /// 使用OrElse合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this IList<Expression<Func<T, bool>>> exprs)
        {
            if (exprs.Count == 0) return null;
            if (exprs.Count == 1) return exprs[0];

            var leftExpr = exprs[0];

            var parameter = leftExpr.Parameters[0];
            var left = leftExpr.Body;
            for (int i = 1; i < exprs.Count; i++)
            {
                var expr = exprs[i];
                var visitor = new ReplaceExpressionVisitor(expr.Parameters[0], parameter);
                var right = visitor.Visit(expr.Body);
                left = Expression.OrElse(left, right);
            }
            return Expression.Lambda<Func<T, bool>>(left, parameter);
        }
    }
}
