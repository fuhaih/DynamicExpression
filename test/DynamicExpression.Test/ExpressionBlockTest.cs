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
            var func = expressionCompiler.Compile("value1+value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }
    }
}
