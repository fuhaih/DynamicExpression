using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionNullableTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(null,null)]
        [TestCase(10086, "10086")]
        public void Nullable_Invocation(int? value,string result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int?>("value1");

            var func = expressionCompiler.Compile<Func<int?, string>>("value1?.ToString()");
            var assert = func.Invoke(value);
            Assert.AreEqual(result, assert);
        }

        [TestCase(null, null)]
        [TestCase(new int[] { 10086, 110 }, 10086)]
        public void Nullable_Indexer(int[] value, int? result)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int[]>("value1");

            var func = expressionCompiler.Compile<Func<int[], int?>>("value1?[0]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            //new int[] { 1, 2 }
            var assert = func.Invoke(value);
            Assert.AreEqual(result, assert);
        }
    }
}
