#define MyAppName "Drone Simulator"
#define MyAppVersion "0.2.0"
#define MyAppExeName "DroneSimulator.exe"
[Setup]
AppId={{8A4B82CB-0F5A-4C58-9D6C-A6B2D3D0F27C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\DroneSimulator
DefaultGroupName={#MyAppName}
OutputDir=..\dist
OutputBaseFilename=DroneSimulatorSetup
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
[Icons]
Name: "{group}\Drone Simulator"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\Drone Simulator"; Filename: "{app}\{#MyAppExeName}"
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Drone Simulator"; Flags: nowait postinstall skipifsilent
