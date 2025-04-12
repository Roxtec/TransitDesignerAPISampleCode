param(
    [Parameter(Mandatory=$true)]
    [string]$name
)

if (Test-Path $name) {
  throw "Sample $name already exists"
}

New-Item -Type Directory $name

$projectFileName = "$name.csproj"
$sampleFileName = "$name.cs"

@"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="..\Common.cs" />
    <Compile Include="..\..\..\..\Full.cs" />
  </ItemGroup>

</Project>
"@ | Out-File $name\$projectFileName


@"
using RtdApiCodeSamples;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var options = Common.ParseCommandLine(args);
    }
}
"@ | Out-File $name\$sampleFileName
