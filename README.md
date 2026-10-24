# Drone Simulator — Windows 11 prototype

**Status:** Source package, not compiled or hardware validated. Seven DJI aircraft profiles are metadata only. The GUI currently uses a hardcoded seven-model list; importing a JSON profile does not dynamically register it in the GUI yet.

## Build

1. Install .NET 8 SDK and Inno Setup 6 on Windows 11.
2. Open this folder in VS Code.
3. Run `powershell -ExecutionPolicy Bypass -File .\build-installer.ps1`.
4. After a successful build, inspect `publish\DroneSimulator.exe` and `dist\DroneSimulatorSetup.exe`.
5. Review and approve installer execution locally.

## Safety / scope

The GUI only simulates altitude, battery, displacement, and GNSS fault. There is **no** real DJI protocol emulator, real aircraft connectivity, MQTT gateway, SDK bridge, flight dynamics, or validated mission execution. Never connect this prototype to live aircraft or operational command endpoints.

## Next development milestones

- Load and validate model profiles from `AircraftLibrary` instead of hardcoding names.
- Build per-aircraft deterministic state machines and test cases.
- Add a loopback-only simulated telemetry gateway with schema tests.
- Add authenticated, opt-in integration adapters based on documented SDK support and real captures.
- Produce traceable test evidence and separate simulation from hardware validation.

## GitHub delivery

Create a separate **private** `DroneSimulator` repository. Do not import into VERSION-5-COLLAB. Grant the existing GitHub/Codex connection access to the new repository if it uses selected-repository access.

This folder is a standalone Git repository ready to push. Keep generated `publish`, `bin`, `obj`, and `dist` directories out of source control.

The workflow `.github/workflows/windows.yml` builds on pushes to main, pull requests, and manual runs. It publishes a self-contained Windows x64 application, compiles the Inno Setup installer, verifies the expected outputs and seven JSON profiles, and uploads the application and installer as the `DroneSimulator-Windows-x64` Actions artifact. Open the repository's Actions page, select a successful run, and download that artifact. Use the complete publish folder for the portable app; WPF native DLLs sit alongside the EXE. The installer packages those files together.

GitHub-hosted Windows runner reference: https://github.com/actions/runner-images/blob/main/images/windows/Windows2022-Readme.md (Inno Setup 6 included at preparation time).

No release is published automatically. The installer is unsigned; code signing and interactive install/uninstall checks remain pending. See VALIDATION.md for observed local results.
