$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Install the .NET 8 SDK before building.' }
    dotnet publish .\DroneSimulator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o .\publish
    if ($LASTEXITCODE -ne 0) { throw 'Application publish failed.' }
    if (-not (Test-Path .\publish\DroneSimulator.exe)) { throw 'Published executable is missing.' }
    Write-Host 'Built: publish\DroneSimulator.exe'
} finally { Pop-Location }
