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
        [TestCase("(34-8)-(12-2)*2 == 6",true)]
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

        /// <summary>
        /// 取余操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
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
        /// 等号操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(14, 2, false)]
        [TestCase(14, 14, true)]
        [TestCase("123", "3456", false)]
        [TestCase("test", "test", true)]
        public void Binary_EqualsExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1==value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 小于号操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(14, 2, false)]
        [TestCase(14, 14, false)]
        [TestCase(2, 14, true)]
        [TestCase(-2, 14, true)]
        [TestCase(-2, -14, false)]
        public void Binary_LessThanExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1<value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 小于等于操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(14, 2, false)]
        [TestCase(14, 14, true)]
        [TestCase(2, 14, true)]
        [TestCase(-2, 14, true)]
        [TestCase(-2, -14, false)]
        [TestCase(-14, -14, true)]
        public void Binary_LessThanOrEqualExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1<=value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);

        }

        /// <summary>
        /// 大于号操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(14, 2, true)]
        [TestCase(14, 14, false)]
        [TestCase(2, 14, false)]
        [TestCase(-2, 14, false)]
        [TestCase(-2, -14, true)]
        [TestCase(-14, -14, false)]
        public void Binary_GreaterThanExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1>value2");
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        /// <summary>
        /// 大于等于操作
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="result"></param>
        [TestCase(14, 2, true)]
        [TestCase(14, 14, true)]
        [TestCase(2, 14, false)]
        [TestCase(-2, 14, false)]
        [TestCase(-2, -14, true)]
        [TestCase(-14, -14, true)]
        public void Binary_GreaterThanOrEqualExpression(object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile("value1>=value2");
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
        [TestCase("value1>value2", "1222", "22", false)]
        [TestCase("value1>=value2", "1222", "22", false)]
        [TestCase("value1<value2", "1222", "22", false)]
        [TestCase("value1<=value2", "1222", "22", false)]
        [TestCase("value1-value2", "1222", "22", "1222")]
        [TestCase("value1*value2", "1222", "22", "1222")]
        [TestCase("value1/value2", "1222", "22", "1222")]
        [TestCase("value1%value2", "1222", "22", "1222")]
        
        public void Binary_StringInvalidOperation(string expression,string value1, string value2, object result)
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

        [TestCase("value1>1&&value2>2", 2, 3, true)]
        [TestCase("value1>1&&value2>3", 2, 3, false)]
        [TestCase("value1>1&&value2>3||value2==3", 2, 3, true)]
        [TestCase("value1>1||value2>2", 2, 3, true)]
        [TestCase("value1>1||value2>3", 2, 3, true)]
        public void Binary_AndAlsoOrElse(string expression, object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }


        #region 位运算
        [TestCase("value1^value2", 2, 3, 1)]
        [TestCase("value1^value2", 2, 0, 2)]
        [TestCase("value1^value2", 3, 0, 3)]
        public void Binary_ExclusiveOr(string expression, object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        [TestCase("value>>2", 16, 4)]
        [TestCase("value>>2", 14, 3)]
        [TestCase("value<<2", 16, 64)]
        public void Binary_LeftAndRightShift(string expression, object value, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value.GetType(), "value");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value);
            Assert.AreEqual(result, assert);
        }

        [TestCase("value1&value2", 3, 2, 2)]
        [TestCase("value1&value2", 6, 10, 2)]
        [TestCase("value1|value2", 3, 2, 3)]
        [TestCase("value1|value2", 6, 10, 14)]
        public void Binary_AndOrOperation(string expression, object value1, object value2, object result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter(value1.GetType(), "value1");
            expressionCompiler.SetParameter(value2.GetType(), "value2");
            var func = expressionCompiler.Compile(expression);
            var assert = func.DynamicInvoke(value1, value2);
            Assert.AreEqual(result, assert);
        }

        #endregion

    }
}