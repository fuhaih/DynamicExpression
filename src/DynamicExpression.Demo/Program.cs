using DynamicExpression.Extension;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DynamicExpression.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var value = new int[4] { 1, 2, 3, 4 };
            var test = value?[0];

            Type[] value1 = new Type[] { typeof(string), typeof(int) };
            Type[] value2 = new Type[] { typeof(string), typeof(string) };
            var result1 = value1.ArrayEquals(value2);
            Func <int?, string> func= value=> value?.ToString();
             
            TestExpression(value=> value.Value);
            Console.ReadLine();
        }

        public static void TestExpression(Expression<Func<int?, int>> expression)
        {
            int? value = null;
            value ++;
            Stopwatch stopwatch = Stopwatch.StartNew();
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int?>("value1");

            var func = expressionCompiler.Compile<Func<int?, int?>>("value1??0");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
            var result = func.Invoke(null);
            //使用DynamicInvoke报错
            //TargetParameterCountException: Parameter count mismatch
            //可能是多层级的结构时候，动态调用无法转换为对应的参数类型，导致异常，这里需要编译为特定类型的委托

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
        public static void TestExpression1()
        {
            //PointerType
            //value1*value2 会被判断为是指针类型的变量定义 LocalDeclarationStatement
            //类型为value1* 名称为value2
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            var func = expressionCompiler.Compile("(34-8)-(12-2)*2");
            var result = func.DynamicInvoke();
        }
    }

    public class Test
    { 
        public string Value { get; set; }
        public string this[int index]
        { 
            get { return Value; }
            set { Value = value; }
        }
    }
}
