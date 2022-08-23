# DynamicExpression
动态表达式，dotnet版本eval

## 调用例子


>加减乘除
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1+2");
var result = func.DynamicInvoke();
```
>比较

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1<2");
var result = func.DynamicInvoke();
```

>带参

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("value+2");
var result = func.DynamicInvoke(3);
```

>代码块

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("{value = value + 2;return value;}");
var result = func.DynamicInvoke(3);
```

## 计划列表

* && || & | ^ ! 操作符操作

* 变量定义 这个也会涉及到程序集的加载

* this[] 操作，用在数组上

* 完善preType

* 浮点数写法 12d,12m,12f

* 静态方法调用(已支持，但是只能调用System.Runtime程序集内的类型，类型相关的操作需要处理程序集加载问题)

* new(延后，类型相关需要处理程序集)

* 类型强制转换(延后，类型相关需要处理程序集)