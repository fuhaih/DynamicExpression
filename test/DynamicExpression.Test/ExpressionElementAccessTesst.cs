using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Test
{
    public class ExpressionElementAccessTesst
    {
        [SetUp]
        public void Setup()
        {
        }
        /// <summary>
        /// 数组索引器
        /// </summary>
        [Test]
        public void ElementAccess_Array()
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int[]>("values");
            var func = expressionCompiler.Compile("values[1]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            int[] value = new int[3] { 5, 6, 7 };
            var result = func.DynamicInvoke(value);
            Assert.AreEqual(result, 6);
        }
        /// <summary>
        /// 多维数组索引器，需要注意，多维数组为入参时，需要编译为固定类型的Expression<T>,才能接收其作为参数，否则会报错
        /// </summary>
        [Test]
        public void ElementAccess_MultidimensionalArrays()
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int[][]>("values");
            var func = expressionCompiler.Compile<Func<int[][], int[]>>("values[1]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            int[][] value = new int[3][];
            value[0] = new int[4] { 1, 2, 3, 4 };
            value[1] = new int[3] { 5, 6, 7 };
            value[2] = new int[3] { 8, 9, 0 };
            var result = func.Invoke(value);
            Assert.AreEqual(result, value[1]);
        }
        /// <summary>
        /// 字符串索引器
        /// </summary>
        [Test]
        public void ElementAccess_String()
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<string>("values");
            var func = expressionCompiler.Compile("values[1]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            string value = "12345";
            var result = func.DynamicInvoke(value);
            char assert = '2';
            Assert.AreEqual(result, assert);
        }
        /// <summary>
        /// 自定义类的索引器
        /// </summary>
        [Test]
        public void ElementAccess_Custom()
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<CustomIndexer>("values");
            var func = expressionCompiler.Compile("values[1]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            CustomIndexer value = new CustomIndexer();
            var result = func.DynamicInvoke(value);
            string assert = "2";
            Assert.AreEqual(result, assert);
        }

       
    }

    public class CustomIndexer
    {
        public string Value = "1234567890";

        public string this[int index]
        {
            get { return Value.Substring(index, 1); }
        }
    }
}
