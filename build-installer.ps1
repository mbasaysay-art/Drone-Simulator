$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & .\build-windows.ps1
    $iscc = @('C:\Program Files (x86)\Inno Setup 6\ISCC.exe', 'C:\Program Files\Inno Setup 6\ISCC.exe') | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $iscc) { throw 'Install Inno Setup 6 to create the installer. Application files are in publish.' }
    & $iscc .\installer\DroneSimulator.iss
    if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed.' }
    if (-not (Test-Path .\dist\DroneSimulatorSetup.exe)) { throw 'Installer output is missing.' }
    Write-Host 'Installer: dist\DroneSimulatorSetup.exe'
} finally { Pop-Location }
