using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionInvocationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("value1.ToString().Substring(2,2)+value2.Substring(2)", 12345, "67890", "34890")]
        public void Invocation_InstanceFunction(string expression, object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        [TestCase("string.Concat(value1, value2)", "12345", "67890", "1234567890")]
        public void Invocation_StaticFunction(string expression, object value1, object value2, object result)
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
