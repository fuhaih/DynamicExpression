using DynamicExpression.Extension;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionMergeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(2,2,true)]
        [TestCase(1,3,false)]
        [TestCase(0,4,false)]
        public void Merge_AndAlso_List(int value1,int value2,bool result)
        {
            Expression<Func<int, int, bool>> epxr1 = (value1, value2) => value1 + value2 == 4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            Expression<Func<int, int, bool>> epxr3 = (value3, value4) => value3 % value4 == 0;
            var epxr4 = new List<Expression<Func<int, int, bool>>>() { epxr1, epxr2 , epxr3 };
            var andPredicate = epxr4.AndAlso();
            Func<int, int, bool> func = andPredicate.Compile();
            Assert.AreEqual(result, func(value1, value2));
        }

        [TestCase(2, 2, true)]
        [TestCase(1, 3, false)]
        [TestCase(0, 4, false)]
        public void Merge_AndAlso(int value1, int value2, bool result)
        {
            Expression<Func<int, int, bool>> epxr1 = (value1, value2) => value1 + value2 == 4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            var andPredicate = epxr1.AndAlso(epxr2);
            Func<int, int, bool> func = andPredicate.Compile();
            Assert.AreEqual(result, func(value1, value2));
        }


        [TestCase(2, 2, true)]
        [TestCase(1, 3, true)]
        [TestCase(0, 5, true)]
        public void Merge_OrElse_List(int value1, int value2, bool result)
        {
            Expression<Func<int, int, bool>> epxr1 = (value1, value2) => value1 + value2 == 4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            Expression<Func<int, int, bool>> epxr3 = (value3, value4) => value3 % value4 == 0;
            var epxr4 = new List<Expression<Func<int, int, bool>>>() { epxr1, epxr2, epxr3 };
            var andPredicate = epxr4.OrElse();
            Func<int, int, bool> func = andPredicate.Compile();
            Assert.AreEqual(result, func(value1, value2));
        }

        [TestCase(2, 2, true)]
        [TestCase(1, 3, true)]
        [TestCase(0, 5, false)]
        public void Merge_OrElse(int value1, int value2, bool result)
        {
            Expression<Func<int, int, bool>> epxr1 = (value1, value2) => value1 + value2 == 4;
            Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
            var andPredicate = epxr1.OrElse(epxr2);
            Func<int, int, bool> func = andPredicate.Compile();
            Assert.AreEqual(result, func(value1, value2));
        }
    }
}
