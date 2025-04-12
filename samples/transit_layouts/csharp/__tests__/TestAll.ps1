param(
    [Parameter(Mandatory=$true)]
    [string]
    $projectId,

    [Parameter(Mandatory=$true)]
    [string]
    $projectApiKey
)

function Exec($scriptBlock) {
    & $scriptBlock
    if ($LastExitCode -ne 0) {
        throw "Command failed"
    }
}
Remove-Item *.testlog

$r = (Get-Random 100000)
$transitName = "Sample Test Transit $r"

Write-Host "Test transit name: $transitName"

Write-Host "Running ErrorHandling..."
Exec { dotnet run -p ..\ErrorHandling\ErrorHandling.csproj $projectId $projectApiKey > errorhandling.testlog }

Write-Host "Running Create..."
Exec { Write-Output $transitName | dotnet run -p ..\Create\Create.csproj $projectId $projectApiKey > create.testlog }

$transitId = (Get-Content create.testlog | Select-String '.*It has transit ID (.*)$' | ForEach-Object { $_.Matches.Groups[1].Value })
Write-Host "Got transit ID $transitId"

Write-Host "Running Get..."
Exec { dotnet run -p ..\Get\Get.csproj $projectId $projectApiKey $transitId > get.testlog }

Write-Host "Running List..."
Exec { dotnet run -p ..\List\List.csproj $projectId $projectApiKey > list.testlog }

Write-Host "Running Update..."
Exec { dotnet run -p ..\Update\Update.csproj $projectId $projectApiKey $transitId > update.testlog }

Write-Host "Running GetDocuments..."
Exec { dotnet run -p ..\GetDocuments\GetDocuments.csproj $projectId $projectApiKey $transitId > getdocuments.testlog }

$saveFilename = (Get-Content getdocuments.testlog | Select-String '.*were saved in (.*)$' | ForEach-Object { $_.Matches.Groups[1].Value })
Write-Host "Document was saved in $saveFilename"

Write-Host "Running VisitRtd..."
cd ..\VisitRtd
Exec { 
    dotnet build /p:Platform=x64 > ..\__tests__\visitrtd.testlog
    .\Launcher\bin\x64\Debug\net9.0\Launcher.exe $projectId $projectApiKey >> ..\__tests__\visitrtd.testlog
}
cd ..\__tests__

Write-Host "Running Delete..."
Exec { dotnet run -p ..\Delete\Delete.csproj $projectId $projectApiKey $transitId > delete.testlog }
