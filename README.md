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

##表达式合并

```csharp
string connectString = "Data Source=.;Initial Catalog=RportTest;Integrated Security=True";
var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
optionsBuilder.UseSqlServer(connectString);
using (TestContext ctx = new TestContext(optionsBuilder.Options))
{

    Expression<Func<ReportData, bool>> epxr1 = report => report.ID == 2023;
    Expression<Func<ReportData, bool>> epxr2 = report => report.Name == "test1";

    var epxr3 = new List<Expression<Func<ReportData, bool>>>() { epxr1, epxr2 };

    var andPredicate = epxr3.AndAlso();
    var andQuery = ctx.ReportData.Where(andPredicate);
    string andSql = andQuery.ToQueryString();
    var andResult = andQuery.ToList();

    var orPredicate = epxr3.OrElse();
    var orQuery = ctx.ReportData.Where(orPredicate);
    string orSql = orQuery.ToQueryString();
    var orResult = orQuery.ToList();
}
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


## 一些不成熟的小想法

* json转换为Expression用作EF的查询表达式

这种只能用在单表操作，不太好用在多表操作，并且这样会给前端太高的可操作性，数据安全性比较低。

* Expression合并

Where查询的时候，可以通过不同的条件来添加Where筛选

```csharp
if(!string.IsNullOfEmpty(name))
{
    query.Where(m=>m.Name == name);
}
......
```
多次判断的时候，多次调用Where，相当于多个条件的&&操作。有时候会需要||操作，就不太好弄

这里想重新构造多个Expression，合并生成一个新的Expression来操作
