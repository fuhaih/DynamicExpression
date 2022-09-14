using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionBlockTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("{value1 = value1+value2;return value1;}", 3,4,7)]
        public void Block_WithParameter(string expression,object value1,object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        [TestCase("{int value1 = value+1;value = value1+1;value1 = value; return value1;}", 3, 5)]
        [TestCase("{int value1=1,value2=2; value1 =value1 + value2 + value; return value1;}", 3, 6)]
        public void Block_Variable(string expression, object value, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value.GetType(), "value");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value);
            Assert.AreEqual(result, assert);
        }
    }
}
