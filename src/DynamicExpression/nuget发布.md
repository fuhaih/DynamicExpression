## ������

��ѡ��Realeseģʽ����������Ŀ

�޸� `DynamicExpression.nuspec`�ļ����°汾��

�ڿ���̨������`cd DynamicExpression`��ת����ǰ��Ŀ�£�Ȼ��������������

```cmd
nuget pack DynamicExpression.csproj -Prop Configuration=Release -IncludeReferencedProjects
```

��Ŀ����Ŀ���

```cmd
dotnet pack DynamicExpression.csproj -p:NuspecFile=DynamicExpression.nuspec -c Release
```

## �ϴ�

```cmd
nuget push DynamicExpression.Core.{version}.nupkg -source http://nuget.senruisoft.com/ -apikey 
# �����ֱ�ӷ�����nuget��
nuget push DynamicExpression.Core.{version}.nupkg -source nuget.org
```

`dotnet nugetָ��Ҳ����`

```cmd
dotnet nuget push DynamicExpression.Core.{version}.nupkg --source http://nuget.senruisoft.com/ --api-key srnugetpublishkey
```

## ��Ŀ����Ŀ

�Ҽ���Ŀ->�༭��Ŀ�ļ� ,��ֱ�Ӵ򿪲��༭csproj�ļ�
```xml
<PropertyGroup>
	<TargetFramework>net5.0/TargetFramework>
</PropertyGroup>
```
ԭ��Ŀ��net5Ŀ����

�޸ĳ�֧�ֶ�Ŀ���,�����һ��dll�汾��Ϣ
```xml
  <PropertyGroup>
    <TargetFrameworks>net5.0;net6.0;net7.0</TargetFrameworks>
    <Version>0.2.4</Version>
    <Authors>fuhaih</Authors>
    <AssemblyVersion>0.2.4</AssemblyVersion>
    <FileVersion>0.2.4</FileVersion>
  </PropertyGroup>
```
��ͬĿ��汾��Ӳ�һ��������

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
����������`net5.0`�õ���`Microsoft.Data.SqlClient 4.1.0`,`net6;net7`�õ���`Microsoft.Data.SqlClient 5.1.1`

nuget�����ļ�ҲҪ����һ��������Ϣ

�򿪲��༭nuspec�ļ�
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

