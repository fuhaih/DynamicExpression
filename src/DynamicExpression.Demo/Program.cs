using System;
using System.Diagnostics;

namespace DynamicExpression.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestExpression1();
            Console.ReadLine();
        }

        public static void TestExpression()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int>("value");
            var func = expressionCompiler.Compile("{value = value + 2;return value;}");
            var result = func.DynamicInvoke(3);
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
}
