# DynamicExpression
动态表达式，dotnet版本eval

## 调用例子


>加减乘除
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1+2");
var result = func.DynamicInvoke(3);
```
>比较

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1<2");
var result = func.DynamicInvoke(3);
```

>带参

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("value+2");
var result = func.DynamicInvoke(3);
```