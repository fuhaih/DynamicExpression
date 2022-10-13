using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionUnaryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("++value", 3, 4)]
        [TestCase("value++", 3, 3)]
        [TestCase("--value", 3, 2)]
        [TestCase("value--", 3, 3)]
        public void Unary_Test(string expression, object value, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value.GetType(), "value");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value);
            Assert.AreEqual(result, assert);
        }
    }
}
