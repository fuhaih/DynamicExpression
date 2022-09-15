# DynamicExpression
动态表达式，dotnet版本eval

目前还未成熟，正式项目中慎用。

悄咪咪的告诉你，`DataTable`中也有计算表达式的功能

```csharp
var value = new DataTable().Compute("1+2",null);//3
```

回到我的动态表达式，是使用Roslyn解析表达式字符串，构建为表达式

## 依赖

是用的dotnet的Roslyn编译器，需要引用以下包

```
Microsoft.CodeAnalysis.CSharp
```

## 注意

这些例子里有关于类型的操作(创建对象、定义变量、静态方法调用)，都只有预设类型(int,string,double......)能用，其他类型需要在类型名称不重复的情况下，使用`SetPredefinedType`手动配置类型和名称，才能使用

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

>方法调用

实例方法
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<string>("value2");
var func = expressionCompiler.Compile("value1.ToString().Substring(2,2)+value2.Substring(2)");
var result = func.DynamicInvoke(12345,"67890");
//34890
```

静态方法

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<string>("value1");
expressionCompiler.SetParameter<string>("value2");
var func = expressionCompiler.Compile("string.Concat(value1, value2)");
var result = func.DynamicInvoke("12345","67890");
//1234567890
```

>代码块

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("{value = value + 2;return value;}");
var result = func.DynamicInvoke(3);
```

>代码块内变量定义

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("{int value1 = value+1;value = value1+1;value1 = value; return value1;}");
var result = func.DynamicInvoke(3);
//输出5
```

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("{int value1=1,value2=2; value1 =value1 + value2 + value; return value1;}");
var result = func.DynamicInvoke(3);
//输出6
```

>其他逻辑运行

&&
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1>1&&value2>3");
var result = func.DynamicInvoke(2,3);
//false
```
||
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1>1||value2>3");
var result = func.DynamicInvoke(2,3);
//true
```

!
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("!((value1+value2)>7");
var result = func.DynamicInvoke(3,4);
//true
```

>位运行

^ 
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1^value2");
var result = func.DynamicInvoke(3,4);
//1
```

|
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1|value2");
var result = func.DynamicInvoke(6,10);
//14
```
&
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1&value2");
var result = func.DynamicInvoke(6,10);
//2
```

`>>`

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("value>>2");
var result = func.DynamicInvoke(14);
//3
```

`<<`

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("value<<2");
var result = func.DynamicInvoke(16);
//64
```

# 表达式合并

## 调用例子

>ef中操作

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

>多个参数表达式

多个表达式中的参数名不一致也没关系，最终会用第一个表达式的参数来代替。

```csharp
Expression<Func<int,int, bool>> epxr1 = (value1,value2)=>value1+value2==4;
Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
var epxr3 = new List<Expression<Func<int,int, bool>>>() { epxr1, epxr2 };
var andPredicate = epxr3.AndAlso();
Func<int, int, bool> func = andPredicate.Compile();//(value1,value2)=>(value1 + value2 == 4) AndAlso (value1 - value2 == 0)
bool result1 = func(2, 2);//true
bool result2 = func(1, 3);//flase
```



## 计划列表

* && || & | ^ ! >> <<操作符操作(已完成)

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

## 一些帮助摸鱼的小想法

* 自动部署程序

某个项目中有很多的第三方dll，每次使用vs远程发布的时候，都需要传输大量文件，比较耗时

想法：在iis服务端进行应用的编译，发布，这个就涉及到从代码库中拉取代码->iis服务器编译->发布

[相关资料](https://www.ecanarys.com/Blogs/ArticleID/409/Deploy-NET-application-on-IIS-using-GitHub-actions)
