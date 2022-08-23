using NUnit.Framework;
using System;
using System.Reflection;

namespace DynamicExpression.Test
{
    public class ExpressionBinaryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// 优先级计算
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        [TestCase("2-2*2",-2)]
        [TestCase("(2-2)*2",0)]
        [TestCase("(34-8)-(12-2)*2",6)]
        public void Binary_Priority(string expression,object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke();
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 加法操作，可支持字符串
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(1,2,3)]
        [TestCase(12,22,34)]
        [TestCase("12","22","1222")]
        public void Binary_AddExpression(object value1,object value2,object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(),"value1");
            expressionCompiler.SetParameter(value2.GetType(),"value2");
            var func = expressionCompiler.Compile("value1+value2");
            var assert = func.DynamicInvoke(value1,value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 减法操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(4, 2, 2)]
        [TestCase(2, 4, -2)]
        [TestCase(64, 43, 21)]   
        public void Binary_SubtractExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1-value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 乘法操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(4, 2, 8)]
        [TestCase(0, 2, 0)]
        [TestCase(0, 0, 0)]
        [TestCase(-2, -2, 4)]
        [TestCase(-2, 6, -12)]
        public void Binary_MultiplyExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1*value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(-2, -2, 1)]
        [TestCase(-2, 2, -1)]
        [TestCase(13, 3, 4)]//<0.5
        [TestCase(14, 3, 4)]//>0.5
        [TestCase(0, 3, 0)]//>0.5
        public void Binary_DivideExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1/value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 除零异常
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase("value1/value2", 14, 0, 0)]
        [TestCase("value1%value2", 14, 0, 0)]
        public void Binary_DivieZeroExpression(string expression, object value1, object value2, object result)
        {
            var exception = Assert.Throws<TargetInvocationException>(() => {
                ExpressionCompiler expressionCompiler = new ExpressionCompiler();
                expressionCompiler.SetParameter(value1.GetType(), "value1");
                expressionCompiler.SetParameter(value2.GetType(), "value2");
                var func = expressionCompiler.Compile(expression);
                var assert = func.DynamicInvoke(value1, value2);
                Assert.AreEqual(result, assert);
            });
            Assert.IsInstanceOf<DivideByZeroException>(exception.InnerException);
        }

        [TestCase(14, 2, 0)]
        [TestCase(14, 3, 2)]
        [TestCase(0, 3, 0)]
        public void Binary_ModuloExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1%value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);

        }

        /// <summary>
        /// 字符串不支持的表达式操作
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase("value1-value2", "1222", "22", "1222")]
        [TestCase("value1*value2", "1222", "22", "1222")]
        [TestCase("value1/value2", "1222", "22", "1222")]
        [TestCase("value1%value2", "1222", "22", "1222")]
        public void Binary_StringInvalidOperation(string expression,string value1, string value2, string result)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                ExpressionCompiler expressionCompiler = new ExpressionCompiler();
                expressionCompiler.SetParameter<string>("value1");
                expressionCompiler.SetParameter<string>("value2");
                var func = expressionCompiler.Compile(expression);
                var assert = func.DynamicInvoke(value1, value2);
            });
        }

    }
}