DRONE SIMULATOR STARTER v0.1
This is a source-code prototype, NOT a precompiled EXE or installer.
Windows 11 build:
1. Install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
2. Extract this ZIP.
3. Open PowerShell in extracted directory.
4. Run: powershell -ExecutionPolicy Bypass -File .\build-windows.ps1
5. Launch publish\DroneSimulator.exe
The app is local-only and simulates basic telemetry for 7 selected models. It does not connect to DJI equipment, your mission control, or any live API. Real protocol emulation, evidence import, installation wizard, and automated validation remain future work.
