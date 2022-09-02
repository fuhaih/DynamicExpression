using DynamicExpression.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static Expression<T> AndAlso<T>(this IList<Expression<T>> exprs)
        {
            if (exprs.Count == 0) return null;
            if (exprs.Count == 1) return exprs[0];

            var leftExpr = exprs[0];
            var left = leftExpr.Body;
            for (int i = 1; i < exprs.Count; i++)
            {
                var expr = exprs[i];
                var visitor = GetReplaceExpressionVisitor(expr.Parameters, leftExpr.Parameters);
                var right = visitor.Visit(expr.Body);
                left = Expression.AndAlso(left, right);
            }
            return Expression.Lambda<T>(left, leftExpr.Parameters);
        }

        /// <summary>
        /// 使用AndAlso合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>left AndAlso right</returns>
        public static Expression<T> AndAlso<T>(this Expression<T> left, Expression<T> right)
        {
            return AndAlso(new List<Expression<T>>() { left, right });
        }

        /// <summary>
        /// 使用OrElse合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exprs"></param>
        /// <returns></returns>
        public static Expression<T> OrElse<T>(this IList<Expression<T>> exprs)
        {
            if (exprs.Count == 0) return null;
            if (exprs.Count == 1) return exprs[0];

            var leftExpr = exprs[0];
            var left = leftExpr.Body;
            for (int i = 1; i < exprs.Count; i++)
            {
                var expr = exprs[i];
                var visitor = GetReplaceExpressionVisitor(expr.Parameters, leftExpr.Parameters);
                var right = visitor.Visit(expr.Body);
                left = Expression.OrElse(left, right);
            }
            return Expression.Lambda<T>(left, leftExpr.Parameters);
        }

        /// <summary>
        /// 使用OrElse合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>left OrElse right</returns>
        public static Expression<T> OrElse<T>(this Expression<T> left, Expression<T> right)
        {
            return OrElse(new List<Expression<T>>() { left, right });
        }


        /// <summary>
        /// 构建visitor
        /// </summary>
        /// <param name="oldParameters"></param>
        /// <param name="newParameters"></param>
        /// <returns></returns>
        private static ReplaceExpressionVisitor GetReplaceExpressionVisitor(ReadOnlyCollection<ParameterExpression> oldParameters, ReadOnlyCollection<ParameterExpression> newParameters)
        {
            Dictionary<Expression, Expression> dic = new Dictionary<Expression, Expression>();
            for (int i = 0; i < oldParameters.Count; i++)
            {
                dic.Add(oldParameters[i],newParameters[i]);
            }
            return new ReplaceExpressionVisitor(dic);
        }
    }
}
