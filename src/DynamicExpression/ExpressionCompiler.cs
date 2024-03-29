﻿using DynamicExpression.Expressions;
using DynamicExpression.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression
{
    /// <summary>
    /// 表达式编译器
    /// </summary>
    public class ExpressionCompiler : CSharpSyntaxRewriter
    {
        /// <summary>
        /// 参数
        /// </summary>
        private readonly Dictionary<string, ParameterExpression> Parameters = new Dictionary<string, ParameterExpression>();

        /// <summary>
        /// 变量
        /// </summary>
        private readonly Dictionary<string, ParameterExpression> Variables = new Dictionary<string, ParameterExpression>();

        /// <summary>
        /// 预设类型<!--https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types-->
        /// </summary>
        private readonly Dictionary<string, Type> PredefinedTypes = new Dictionary<string, Type>()
        {
            {"bool",typeof(bool) },
            {"byte",typeof(byte) },
            {"sbyte",typeof(sbyte) },
            {"char",typeof(char) },
            {"decimal",typeof(decimal) },
            {"double",typeof(double) },
            {"float",typeof(float) },
            {"int",typeof(int) },
            {"uint",typeof(uint) },
            {"nint",typeof(nint) },
            {"nuint",typeof(nuint) },
            {"long",typeof(long) },
            {"ulong",typeof(ulong) },
            {"short",typeof(short) },
            {"ushort",typeof(ushort) },
            {"object",typeof(object) },
            {"string",typeof(string) },
            {"dynamic",typeof(object) }
        };

        /// <summary>
        /// 栈
        /// </summary>
        Stack<Expression> Expressions = new Stack<Expression>();

        //public Type GetType(string name)
        //{

            
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            Visit(node.Left);
            var left = Expressions.Pop();
            Visit(node.Right);
            var right = Expressions.Pop();
            switch (node.Kind())
            {
                case SyntaxKind.AddExpression://+
                    MethodInfo method = null;
                    if (left.Type == typeof(string) || right.Type == typeof(string))
                    {
                        method = typeof(string).GetMethod("Concat",
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            CallingConventions.Any,
                            new Type[] { typeof(object), typeof(object) },
                        null);
                    }
                    Expressions.Push(Expression.Add(left, right, method));
                    break;
                case SyntaxKind.SubtractExpression://-
                    Expressions.Push(Expression.Subtract(left, right));
                    break;
                case SyntaxKind.MultiplyExpression://*
                    Expressions.Push(Expression.Multiply(left, right));
                    break;
                case SyntaxKind.DivideExpression:// /
                    Expressions.Push(Expression.Divide(left, right));
                    break;
                case SyntaxKind.ModuloExpression:// %
                    Expressions.Push(Expression.Modulo(left, right));
                    break;
                case SyntaxKind.EqualsExpression:// ==
                    Expressions.Push(Expression.Equal(left, right));
                    break;
                case SyntaxKind.NotEqualsExpression://!=
                    Expressions.Push(Expression.NotEqual(left,right));
                    break;
                case SyntaxKind.LessThanExpression:// <
                    Expressions.Push(Expression.LessThan(left, right));
                    break;
                case SyntaxKind.LessThanOrEqualExpression:// <=
                    Expressions.Push(Expression.LessThanOrEqual(left, right));
                    break;
                case SyntaxKind.GreaterThanExpression:// >
                    Expressions.Push(Expression.GreaterThan(left, right));
                    break;
                case SyntaxKind.GreaterThanOrEqualExpression:// >=
                    Expressions.Push(Expression.GreaterThanOrEqual(left, right));
                    break;
                case SyntaxKind.BitwiseAndExpression:// &
                    Expressions.Push(Expression.And(left, right));
                    break;
                case SyntaxKind.LogicalAndExpression:// &&
                    Expressions.Push(Expression.AndAlso(left, right));
                    break;
                case SyntaxKind.BitwiseOrExpression:// |
                    Expressions.Push(Expression.Or(left, right));
                    break;
                case SyntaxKind.LogicalOrExpression:// ||
                    Expressions.Push(Expression.OrElse(left, right));
                    break;
                case SyntaxKind.ExclusiveOrExpression:// ^
                    Expressions.Push(Expression.ExclusiveOr(left, right));
                    break;
                case SyntaxKind.LeftShiftExpression:// <<
                    Expressions.Push(Expression.LeftShift(left, right));
                    break;
                case SyntaxKind.RightShiftExpression:// >>
                    Expressions.Push(Expression.RightShift(left, right));
                    break;
                case SyntaxKind.CoalesceExpression:// ??
                    right = Expression.Convert(right, left.Type);
                    Expressions.Push(Expression.Coalesce(left,right));
                    break;
                    
            }
            return node;
        }

        /// <summary>
        ///  () 括号,这个好像不用处理
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return base.VisitParenthesizedExpression(node);
        }

        /// <summary>
        /// 字面量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                    Expressions.Push(Expression.Constant(node.Token.Value));
                    break;
                case SyntaxKind.StringLiteralExpression:
                    Expressions.Push(Expression.Constant(node.Token.Value));
                    break;
                case SyntaxKind.NullLiteralExpression:
                    Expressions.Push(Expression.Constant(node.Token.Value));
                    break;
            }
            return node;
        }

        /// <summary>
        /// 方法调用
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            List<Expression> arguments = new List<Expression>();
            Expression expression = null;
            string methodName = null;
            foreach (var item in node.ArgumentList.ChildNodes())
            {
                Visit(item);
                arguments.Add(Expressions.Pop());
            }

            if (node.Expression is MemberAccessExpressionSyntax)
            {
                var member = (MemberAccessExpressionSyntax)node.Expression;
                Visit(member.Expression);
                expression = Expressions.Pop();
                methodName = member.Name.Identifier.ValueText;
                
            }
            else {
                var member = (MemberBindingExpressionSyntax)node.Expression;
                //Visit(member.Expression);
                expression = Expressions.Pop();
                methodName = member.Name.Identifier.ValueText;
                
            }
            //var invocation = Expressions.Pop();
            var argumentTypes = arguments.Select(m => m.Type).ToArray();
            MethodInfo method = expression.Type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                null,
                CallingConventions.Any,
                argumentTypes,
                null);
            if (method == null)
            {
                throw new Exception($"找不到成员{methodName}");
            }
            Expressions.Push(Expression.Call(expression is StaticMemberExpression ? null : expression, (MethodInfo)method, arguments.ToArray()));

            return node;
            //return base.VisitInvocationExpression(node);
        }

        /// <summary>
        /// 绑定表达式 value?.Value时，就是MemberBindingExpression(.ToString())
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            return base.VisitMemberBindingExpression(node);
        }

        /// <summary>
        /// visit 属性(值处理属性，方法会在Invocation中处理)  MemberAccessExpression(value.Value)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            Visit(node.Expression);
            var expression = Expressions.Pop();
            string name = node.Name.Identifier.ValueText;
            MemberInfo member = expression.Type.GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).FirstOrDefault();

            if (member == null)
            {
                throw new Exception($"找不到成员{name}");
            }
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                case MemberTypes.Field:
                    Expressions.Push(Expression.MakeMemberAccess(expression is StaticMemberExpression ? null : expression, member));
                    break;
                case MemberTypes.Method:
                    Expressions.Push(Expression.Call(expression is StaticMemberExpression ? null : expression, (MethodInfo)member));
                    break;
                default: throw new Exception($"暂不支持{name}");
            }
            return node;
        }

        /// <summary>
        /// 关键字类型，string,int 等
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitPredefinedType(PredefinedTypeSyntax node)
        {
            string name = node.Keyword.ValueText;
            if (PredefinedTypes.TryGetValue(name, out Type type))
            {
                Expressions.Push(new StaticMemberExpression(type));
            }
            return node;
        }

        /// <summary>
        /// 带前缀的一元运算
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            Visit(node.Operand);
            var expression = Expressions.Pop();
            switch (node.Kind())
            {
                case SyntaxKind.LogicalNotExpression://!value
                    Expressions.Push(Expression.Not(expression));
                    break;
                case SyntaxKind.PreIncrementExpression://++value
                    Expressions.Push(Expression.PreIncrementAssign(expression));
                    break;
                case SyntaxKind.PreDecrementExpression://--value
                    Expressions.Push(Expression.PreDecrementAssign(expression));
                    break ;
                case SyntaxKind.UnaryMinusExpression://-value
                    Expressions.Push(Expression.Negate(expression));
                    break;
                case SyntaxKind.UnaryPlusExpression://+value
                    Expressions.Push(Expression.UnaryPlus(expression));
                    break;
                case SyntaxKind.BitwiseNotExpression://~value
                    Expressions.Push(Expression.Not(expression));
                    break;
            }

            return node;
        }

        /// <summary>
        /// 后缀的一元表达式 value++ value--
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            Visit(node.Operand);
            var expression = Expressions.Pop();
            switch (node.Kind())
            {
                case SyntaxKind.PostIncrementExpression://value++
                    Expressions.Push(Expression.PostIncrementAssign(expression));
                    break;
                case SyntaxKind.PostDecrementExpression://value--
                    Expressions.Push(Expression.PostDecrementAssign(expression));
                    break;
            }

            return node;
        }

        /// <summary>
        /// 三元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            Visit(node.Condition);
            var condition = Expressions.Pop();
            Visit(node.WhenFalse);
            var whenFalse = Expressions.Pop();
            Visit(node.WhenTrue);
            var whenTrue = Expressions.Pop();
            Expressions.Push(Expression.Condition(condition,whenTrue,whenFalse));
            return node;
        }

        /// <summary>
        /// 标识,静态类、参数名、变量名等
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            string name = node.Identifier.ValueText;
            switch (node.Kind())
            {
                case SyntaxKind.IdentifierName:
                    VisitName(node.Identifier);
                    break;

            }
            return node;
        }

        /// <summary>
        /// 访问名称，获取变量、类等
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SyntaxToken VisitName(SyntaxToken token)
        {
            string name = token.ValueText;
            if (string.IsNullOrWhiteSpace(name)) return token;
            if (Parameters.TryGetValue(name, out var parameter))
            {
                Expressions.Push(parameter);
            }
            else if (Variables.TryGetValue(name, out var variable))
            {
                Expressions.Push(variable);
            }
            else if (PredefinedTypes.TryGetValue(name, out var predefinedType))
            {
                Expressions.Push(new StaticMemberExpression(predefinedType));
            }
            else
            {
                //Assembly assembly = Assembly.Load("System.Runtime");
                Assembly assembly = typeof(DateTime).Assembly;
                Type type = assembly.GetTypes().Where(m => m.Name == name).FirstOrDefault();
                if (type == null)
                {
                    throw new Exception($"找不到成员{name}");
                }
                Expressions.Push(new StaticMemberExpression(type));
            }
            return token;
        }

        /// <summary>
        /// 指针类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitPointerType(PointerTypeSyntax node)
        {
            Visit(node.ElementType);
            return node;
        }

        /**
         * 带参的乘法 value1*value2 会分析为PointerTypeSyntax
         * 
         * c#中只能在安全模式中使用指针，所以直接把他处理回乘法，暂不考虑在动态表达式中允许指针的使用
         * 
         * 也可以使用SyntaxFactory.ParseExpression来解析，这样解析出来是乘法操作，但是该方式没办法解析块代码{},可以解析()=>{}这样的块代码
         */


        /// <summary>
        /// 声明
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            switch (node.Type.Kind())
            {
                case SyntaxKind.PointerType:
                    {
                        Visit(node.Type);
                        var left = Expressions.Pop();
                        var variable = node.Variables.FirstOrDefault();
                        VisitName(variable.Identifier);
                        var right = Expressions.Pop();
                        Expressions.Push(Expression.Multiply(left,right));
                    }
                    break;
                default:
                    {
                        foreach (var item in node.Variables)
                        {
                            Visit(node.Type);
                            Visit(item);
                        }
                            
                    }break;
                //default:
                //    {
                //        Visit(node.Type);
                //        var left = Expressions.Pop();

                //    }
                //    break;
            }
            return node;
        }

        /// <summary>
        /// 声明变量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var type = Expressions.Pop();
            string name = node.Identifier.ValueText;
            var left = Expression.Variable(type.Type, name);
            Variables.Add(name, left);

            if (node.Initializer != null)
            {
                Visit(node.Initializer);
                var right = Expressions.Pop();
                Expressions.Push(Expression.Assign(left, right));
            }
            return node;
        }

        /// <summary>
        /// 变量定义
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return base.VisitLocalDeclarationStatement(node);
        }

        /// <summary>
        /// 代码块{}
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            List<Expression> expressions = new List<Expression>();

            foreach (var item in node.Statements)
            {
                Visit(item);
                while (Expressions.TryPop(out var expression))
                {
                    expressions.Add(expression);
                }
            }
            Expressions.Push(Expression.Block(Variables.Values, expressions));
            return node;
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            Visit(node.Expression);
            var item  = Expressions.Pop();
            List<Expression> paramters = new List<Expression>();
            foreach (var parameter in node.ArgumentList.Arguments)
            {
                Visit(parameter);
                paramters.Add(Expressions.Pop());
            }
            var types = paramters.Select(m => m.Type).ToArray();
            var indexer = GetIndexer(item, types);
            Expressions.Push(Expression.MakeIndex(item,indexer, paramters));
            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            var item = Expressions.Pop();
            List<Expression> paramters = new List<Expression>();
            foreach (var parameter in node.ArgumentList.Arguments)
            {
                Visit(parameter);
                paramters.Add(Expressions.Pop());
            }
            var types = paramters.Select(m => m.Type).ToArray();
            var indexer = GetIndexer(item, types);
            Expressions.Push(Expression.MakeIndex(item, indexer, paramters));
            return node;
        }

        /// <summary>
        /// ?.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            Visit(node.Expression); 
            var expression = Expressions.Pop();
            Expressions.Push(expression);
            Visit(node.WhenNotNull);
            
            var whenNotNull = Expressions.Pop();
            if (whenNotNull.Type.IsValueType)
            { 
                whenNotNull = Expression.Convert(whenNotNull, GetNullableType(whenNotNull.Type));
            }
            var nullValue = Expression.Constant(null, whenNotNull.Type);
            var condition = Expression.Equal(expression, Expression.Constant(null));
            Expressions.Push(Expression.Condition(condition,nullValue,whenNotNull));
            return node;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            Visit(node.Left);
            var left = Expressions.Pop();
            Visit(node.Right);
            var right = Expressions.Pop();
            switch (node.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    Expressions.Push(Expression.Assign(left, right));
                    break;
                case SyntaxKind.AddAssignmentExpression:
                    Expressions.Push(Expression.AddAssign(left, right));
                    break;
                case SyntaxKind.SubtractAssignmentExpression:
                    Expressions.Push(Expression.SubtractAssign(left, right));
                    break;
                case SyntaxKind.MultiplyAssignmentExpression:
                    Expressions.Push(Expression.MultiplyAssign(left, right));
                    break;
                case SyntaxKind.DivideAssignmentExpression:
                    Expressions.Push(Expression.DivideAssign(left, right));
                    break;
                case SyntaxKind.ModuloAssignmentExpression:
                    Expressions.Push(Expression.ModuloAssign(left, right));
                    break;
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    Expressions.Push(Expression.ExclusiveOrAssign(left, right));
                    break;
                case SyntaxKind.AndAssignmentExpression://value&=1
                    Expressions.Push(Expression.AndAssign(left, right));
                    break;
                case SyntaxKind.OrAssignmentExpression://value|=1
                    Expressions.Push(Expression.OrAssign(left, right));
                    break;
                //case SyntaxKind.CoalesceAssignmentExpression://好像不支持
                //    Expressions.Push(Expression.Coalesce(left, right));
                //    break;
                case SyntaxKind.LeftShiftAssignmentExpression:
                    Expressions.Push(Expression.LeftShiftAssign(left, right));
                    break;
                case SyntaxKind.RightShiftAssignmentExpression:
                    Expressions.Push(Expression.RightShiftAssign(left, right));
                    break;
                    
            }



            return node;
        }

        /// <summary>
        /// 编译表达式，如果是简单的表达式，推荐使用<see cref="CompileSimpleExpression"/> 方法
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Delegate Compile(string pattern)
        {
            var expressionSyntax = SyntaxFactory.ParseStatement(pattern);
            Visit(expressionSyntax);
            Expression expression = Expressions.Pop();
            return Expression.Lambda(expression, Parameters.Values.ToArray()).Compile();
        }

        /// <summary>
        /// 编译表达式，如果是简单的表达式，推荐使用<see cref="CompileSimpleExpression"/> 方法
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public T Compile<T>(string pattern)
        {
            var expressionSyntax = SyntaxFactory.ParseStatement(pattern);
            Visit(expressionSyntax);
            Expression expression = Expressions.Pop();
            return Expression.Lambda<T>(expression, Parameters.Values.ToArray()).Compile();
        }

        /// <summary>
        /// 编译简单的表达式，也就是一条语句的表达式
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Delegate CompileSimpleExpression(string pattern)
        {
            var expressionSyntax = SyntaxFactory.ParseExpression(pattern);
            Visit(expressionSyntax);
            Expression expression = Expressions.Pop();
            return Expression.Lambda(expression, Parameters.Values.ToArray()).Compile();
        }

        /// <summary>
        /// 配置表达式参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public void SetParameter<T>(string name)
        {
            ParameterExpression input = Expression.Parameter(typeof(T), name);
            Parameters.Add(name, input);
        }

        /// <summary>
        /// 配置表达式参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public void SetParameter(Type type, string name)
        {
            ParameterExpression input = Expression.Parameter(type, name);
            Parameters.Add(name, input);
        }

        /// <summary>
        /// 配置类型关键字，可以直接使用类型关键字来访问
        /// </summary>
        /// <param name="name"></param>
        public void SetPredefinedType<T>(string name)
        {
            SetPredefinedType(typeof(T), name);
        }

        /// <summary>
        /// 配置类型关键字，可以直接使用类型关键字来访问
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void SetPredefinedType(Type type,string name)
        {
            PredefinedTypes.Add(name, type);
        }

        /// <summary>
        /// 获取类型的可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Type GetNullableType(Type type)
        {
            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
            //type = Nullable.GetUnderlyingType(type) ?? type; // avoid type becoming null
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }

        /// <summary>
        /// 获取索引器对应的属性
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public PropertyInfo GetIndexer(Expression expression, Type[] parameters)
        {
            var properties = expression.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                ParameterInfo[] parameterInfo = property.GetIndexParameters();
                Type[] parameterInfoType = parameterInfo.Select(x => x.ParameterType).ToArray();
                if (parameterInfoType.Length == parameters.Length && parameters.ArrayEquals(parameterInfoType))
                {
                    return property;
                }
            }
            return null;
        }
    }
}
