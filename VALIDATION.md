# Validation — 24 October 2026

- Imported from the valid local Downloads/DroneSimulator_Windows_Source_20261024.zip.
- Original archive SHA256: 3241F70BB291C49753871E8FAF5BD2B0A47FC80132DC890BC3267E27148B002A.
- Windows x64 self-contained Release publish: PASS using installed SDK 9.0.300, targeting net8.0-windows.
- Seven aircraft JSON profiles parse and are included in publish: PASS.
- Project XML and PowerShell script syntax: PASS.
- Compiled application remained alive for five seconds after hidden launch: PASS. Process then stopped by the check. This does not verify UI interactions.
- Installer compilation: PASS on GitHub Windows runner with Inno Setup 6.7.1; Inno Setup remains absent locally.
- Interactive flight controls, installation/uninstallation, and real hardware/protocol compatibility: NOT VERIFIED.
- GitHub Actions run #1: PASS, including output/profile checks and artifact upload: https://github.com/mbasaysay-art/Drone-Simulator/actions/runs/40072227699 . Source commit c75c5023ba6e8cbb2eb9e386c6d4b4a1b2299687 matches the local source. A Node 20 deprecation warning prompted updating the three official actions to their documented current majors; that follow-up run must be checked separately.

The restricted build initially failed on .NET/NuGet user-directory access. An approved build with workspace-local CLI and package caches succeeded. No changes were made to VERSION-5-COLLAB; that checkout already contains unrelated local changes.

The imported app remains a basic local telemetry prototype. It has no DJI SDK bridge or real aircraft connection. The GUI aircraft list is hardcoded; JSON profiles are metadata.

