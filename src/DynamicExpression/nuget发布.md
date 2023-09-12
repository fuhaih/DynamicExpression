## 构建包

先选择Realese模式重新生成项目

修改 `DynamicExpression.nuspec`文件更新版本号

在控制台中输入`cd DynamicExpression`跳转到当前项目下，然后再输入命令构造包

```cmd
nuget pack DynamicExpression.csproj -Prop Configuration=Release -IncludeReferencedProjects
```

多目标项目打包

```cmd
dotnet pack DynamicExpression.csproj -p:NuspecFile=DynamicExpression.nuspec -c Release
```

## 上传

```cmd
nuget push DynamicExpression.Core.{version}.nupkg -source http://nuget.senruisoft.com/ -apikey 
# 这个会直接发布到nuget上
nuget push DynamicExpression.Core.{version}.nupkg -source nuget.org
```

`dotnet nuget指令也可以`

```cmd
dotnet nuget push DynamicExpression.Core.{version}.nupkg --source http://nuget.senruisoft.com/ --api-key srnugetpublishkey
```

## 多目标项目

右键项目->编辑项目文件 ,会直接打开并编辑csproj文件
```xml
<PropertyGroup>
	<TargetFramework>net5.0/TargetFramework>
</PropertyGroup>
```
原项目是net5目标框架

修改成支持多目标的,并添加一下dll版本信息
```xml
  <PropertyGroup>
    <TargetFrameworks>net5.0;net6.0;net7.0</TargetFrameworks>
    <Version>0.2.4</Version>
    <Authors>fuhaih</Authors>
    <AssemblyVersion>0.2.4</AssemblyVersion>
    <FileVersion>0.2.4</FileVersion>
  </PropertyGroup>
```
不同目标版本添加不一样的依赖

```xml
  <ItemGroup Condition=" '$(TargetFramework)'=='net5.0'">
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="4.1.0" />
  </ItemGroup>
  <ItemGroup Condition=" '$(TargetFramework)'=='net6.0'">
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.1" />
  </ItemGroup>
  <ItemGroup Condition=" '$(TargetFramework)'=='net7.0'">
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.1" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="MySqlConnector" Version="2.2.6" />
  </ItemGroup>
```
上面配置中`net5.0`用的是`Microsoft.Data.SqlClient 4.1.0`,`net6;net7`用的是`Microsoft.Data.SqlClient 5.1.1`

nuget配置文件也要配置一下依赖信息

打开并编辑nuspec文件
```xml
    <dependencies>
      <group targetFramework="net5.0">
        <dependency id="Microsoft.Data.SqlClient" version="4.1.0" />
        <dependency id="MySqlConnector" version="2.2.6" />
      </group>
      <group targetFramework="net6.0">
        <dependency id="Microsoft.Data.SqlClient" version="5.1.1" />
        <dependency id="MySqlConnector" version="2.2.6" />
      </group>
      <group targetFramework="net7.0">
        <dependency id="Microsoft.Data.SqlClient" version="5.1.1" />
        <dependency id="MySqlConnector" version="2.2.6" />
      </group>
    </dependencies>
```

