﻿using DynamicExpression.Expressions;
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
        private readonly Dictionary<string, ParameterExpression> Parameters = new Dictionary<string, ParameterExpression>();

        private readonly Dictionary<string, Type> PredefinedTypes = new Dictionary<string, Type>()
        {
            {"string",typeof(string) },
            {"int",typeof(int) },
            {"long",typeof(long) },
            {"float",typeof(float) },
            {"double",typeof(double) },
            {"decimal",typeof(decimal) },
            {"byte",typeof(byte) },
        };

        Stack<Expression> Expressions = new Stack<Expression>();
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
            }
            return node;
        }

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            List<Expression> arguments = new List<Expression>();

            foreach (var item in node.ArgumentList.ChildNodes())
            {
                Visit(item);
                arguments.Add(Expressions.Pop());
            }
            var member = (MemberAccessExpressionSyntax)node.Expression;
            Visit(member.Expression);
            var expression = Expressions.Pop();
            string name = member.Name.Identifier.ValueText;
            //var invocation = Expressions.Pop();
            var argumentTypes = arguments.Select(m => m.Type).ToArray();
            MethodInfo method = expression.Type.GetMethod(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                null,
                CallingConventions.Any,
                argumentTypes,
                null);
            if (method == null)
            {
                throw new Exception($"找不到成员{name}");
            }
            Expressions.Push(Expression.Call(expression is StaticMemberExpression ? null : expression, (MethodInfo)method, arguments.ToArray()));
            return node;
            //return base.VisitInvocationExpression(node);
        }

        /// <summary>
        /// visit 属性
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
                    if (Parameters.TryGetValue(name, out var parameter))
                    {
                        Expressions.Push(parameter);
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
                    break;

            }
            return node;
        }

        /// <summary>
        /// 编译表达式
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Delegate Compile(string pattern)
        {
            ExpressionSyntax expressionSyntax = SyntaxFactory.ParseExpression(pattern);
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
        /// 配置类型关键字，可以直接使用类型关键字来访问
        /// </summary>
        /// <param name="name"></param>
        public void SetPredefinedType<T>(string name)
        {
            SetPredefinedType(name, typeof(T));
        }

        /// <summary>
        /// 配置类型关键字，可以直接使用类型关键字来访问
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void SetPredefinedType(string name, Type type)
        {
            PredefinedTypes.Add(name, type);
        }
    }
}
