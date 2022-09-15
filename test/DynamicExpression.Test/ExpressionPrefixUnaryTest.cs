using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionPrefixUnaryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("!((value1+value2)>7)", 3, 4, true)]
        [TestCase("!((value1+value2)>6)", 3, 4, false)]
        public void Unary_NotOperation(string expression, object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }
    }
}
