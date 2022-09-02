using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DynamicExpression.Extension;
namespace DynamicExpression.Demo
{
    public static class ExpressionMerge
    {
        public static void ExpressionAndMerge()
        {
            Expression<Func<int,int, bool>> epxr1 = (value1,value2)=>value1+value2==4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            var epxr3 = new List<Expression<Func<int,int, bool>>>() { epxr1, epxr2 };
            var andPredicate = epxr3.AndAlso();
            Func<int, int, bool> func = andPredicate.Compile();
            bool result1 = func(2, 2);
            bool result2 = func(1, 3);
            var andPredicate2 = epxr1.AndAlso(epxr2);
            Func<int, int, bool> func2 = andPredicate.Compile();
            bool result3 = func2(2, 2);
            bool result4 = func2(1, 3);
        }
        public static void ExpressionOrMerge()
        {
            Expression<Func<int, int, bool>> epxr1 = (value1, value2) => value1 + value2 == 4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            var epxr3 = new List<Expression<Func<int, int, bool>>>() { epxr1, epxr2 };
            var orPredicate = epxr3.OrElse();
            Func<int, int, bool> func = orPredicate.Compile();
            bool result1 = func(3, 3);
            bool result2 = func(3, 4);
            var orPredicate2 = epxr1.OrElse(epxr2);
            Func<int, int, bool> func2 = orPredicate2.Compile();
            bool result3 = func2(3, 3);
            bool result4 = func2(3, 4);
        }
    }
}
