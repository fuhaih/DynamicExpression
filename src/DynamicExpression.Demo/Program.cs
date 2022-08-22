using System;

namespace DynamicExpression.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionCompiler.SetParameter<int>("value");
            var func = expressionCompiler.Compile("value+2");
            var result = func.DynamicInvoke(3);
            Console.ReadLine();
        }
    }
}
