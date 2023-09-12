# DynamicExpression
DynamicExpression，Eval

You can search `DynamicExpression.Core` on nuget


DynamicExpression using `Roslyn` to parse strings into grammar trees,
which are then converted into expressions

By the way, i also find the similar features in `DataTable`

```csharp
var value = new DataTable().Compute("1+2",null);//3
```
 

## Reference
```
Microsoft.CodeAnalysis.CSharp
```

## Pay attention 

In these examples, there are operations related to types (creating objects, defining variables, static method calls) that can only be used with preset types (int, string, double...). Other types need to be manually configured with `SetPredefinedType` without duplicate type names in order to be used

## Examples


>Arithmetic Operator
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1+2");
var result = func.DynamicInvoke();
```
>Compare

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
var func = expressionCompiler.Compile("1<2");
var result = func.DynamicInvoke();
```

>Parameter

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("value+2");
var result = func.DynamicInvoke(3);
```

>Method Call

instance method call
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<string>("value2");
var func = expressionCompiler.Compile("value1.ToString().Substring(2,2)+value2.Substring(2)");
var result = func.DynamicInvoke(12345,"67890");
//34890
```

static methord call

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<string>("value1");
expressionCompiler.SetParameter<string>("value2");
var func = expressionCompiler.Compile("string.Concat(value1, value2)");
var result = func.DynamicInvoke("12345","67890");
//1234567890
```

>Block

```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value");
var func = expressionCompiler.Compile("{value = value + 2;return value;}");
var result = func.DynamicInvoke(3);
```

>Defining Variables

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

>Logical Operation

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

>Bitwise Operation

^ 
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int>("value1");
expressionCompiler.SetParameter<int>("value2");
var func = expressionCompiler.Compile("value1^value2");
var result = func.DynamicInvoke(3,4);
//7
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

>Indexer this[]

array：
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int[]>("values");
var func = expressionCompiler.Compile("values[0]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
var result = func.DynamicInvoke(new int[] { 1,2,3});
//result: 1
```

string:
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<string>("values");
var func = expressionCompiler.Compile("values[0]");
var result = func.DynamicInvoke("123");
//result: '1'
```

class:
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<Test>("values");
var func = expressionCompiler.Compile("values[0]");
var result = func.DynamicInvoke(new Test());
//result: null
```


Jagged arrays:
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int[][]>("values");
var func = expressionCompiler.Compile<Func<int[][], int[]>>("values[1]");//value[0,0] 多维数组/矩形数组 是在this中有多个参数的
int[][] value = new int[3][];
value[0] = new int[4] { 1, 2, 3, 4 };
value[1] = new int[3] { 5, 6, 7 };
value[2] = new int[3] { 8, 9, 0 };
var result = func.Invoke(value);
//result: { 5, 6, 7 }
```

When a jagged arrays is used as an input parameter, it needs to be compiled into a specific type of expression, using the `Invoke` method instead of `DynamicInvoke`.


>Nullable Types

Nullable types also require calling the Invoke method

`??`
```csharp
ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int?>("value1");
var func = expressionCompiler.Compile<Func<int?,int?>>("value1??0");
var result1 = func.Invoke(null);//0
var result1 = func.Invoke(2);//2
```

?.

```csharp

ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int?>("value1");
var func = expressionCompiler.Compile<Func<int?, string>>("value1?.ToString()");
var result1 = func.Invoke(null);//null
var result1 = func.Invoke(2);//"2"

```
?[]
```csharp

ExpressionCompiler expressionCompiler = new ExpressionCompiler();
expressionCompiler.SetParameter<int[]>("value1");

var func = expressionCompiler.Compile<Func<int[], int?>>("value1?[0]");//value[0,0] 多维数组/矩形
var result1 = func.Invoke(null);//null
var result1 = func.Invoke(new int[] { 10086, 110 });//10086

```


# Expression Merge

## Example

>use in EFCore

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

>multi parameter expression


```csharp
Expression<Func<int,int, bool>> epxr1 = (value1,value2)=>value1+value2==4;
Expression<Func<int, int, bool>> epxr2 = (value3, value4) => value3 - value4 == 0;
var epxr3 = new List<Expression<Func<int,int, bool>>>() { epxr1, epxr2 };
var andPredicate = epxr3.AndAlso();
Func<int, int, bool> func = andPredicate.Compile();//(value1,value2)=>(value1 + value2 == 4) AndAlso (value1 - value2 == 0)
bool result1 = func(2, 2);//true
bool result2 = func(1, 3);//flase
```

## Support Operator

### Operator

> Arithmetic operator

Unary operator: ++ -- + -

binary operation： * / % + -

> logical operation

Unary operator：！

binary operation：&& ||

> Bitwise operation

binary operation：  & | ^ << >>  ~按位求补

>Comparative operation

== ！= < > <= >= 

> member access

. (Properties, Fields, Methods)

[] (Indexer)

>Assignment Operators

= 

> Compound Assignment

+= -= *= /= %= ^= &= |= ??=

>?? 

> ternary 

? :

### Block

>block

{.....;return x;}

>define parameter

{int i=0;return i}


## Todo List


* 成员访问运算符 Null 条件运算符 ?. 和 ?[]   范围运算符..

    **lambda表达式中不能有?. 和?[]操作**:
    表达式树lambda不能包含空传播运算符

* 类型测试运算符 is as等


* 浮点数写法 12d,12m,12f

* 静态方法调用(已支持，但是只能调用System.Runtime程序集内的类型，类型相关的操作需要处理程序集加载问题)

* new(延后，类型相关需要处理程序集)

* 类型强制转换(延后，类型相关需要处理程序集)


