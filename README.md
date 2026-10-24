# DroneSimulator 0.2.0 — Windows simulation connector

Seven-model Windows WPF prototype. Sends simulated telemetry to a compatible Mission Operations test server. It does not implement DJI protocols or communicate with real aircraft. Model JSON files remain metadata; the GUI model list is hardcoded.

## Connect

1. Install the matching system connector update on a separate test system; enable WINDOWS_SIMULATOR_ENABLED=1 there. Older servers do not support pairing.
2. Sign in to that system in your browser. Open Mission Operations > Live Monitoring > Simulation > Pair Windows simulator.
3. In DroneSimulator, enter that system's address and the one-time code, then click Connect. Codes expire after two minutes and can be used once.
4. Click Start virtual flight. View position, altitude, battery and GNSS faults in the browser's Simulation view. Set the test origin before starting a flight.
5. Click Disconnect & clear aircraft to remove the test unit. Stopping a flight keeps the connection alive and sends idle telemetry. Closing the app stops telemetry; the last position becomes stale after 15 seconds.

Use HTTPS for other computers. HTTP is accepted only for loopback addresses such as http://localhost:3000. The server must be reachable from the Windows computer; localhost refers to that computer. Redirects and invalid certificates are not bypassed.

The browser account needs VIEW_TAB_MISSION_OPERATIONS, MISSION_SIMULATE and MISSION_CONNECT_SIMULATOR. The new connector permission defaults to Admin only. Keep the originating browser signed in. Pairing does not perform a second login. The scoped token stays in memory, expires after one hour, and is invalidated by re-pairing, disconnect, browser logout or loss of required permissions. It cannot create missions or access other application APIs. No password is entered in this app.

## Build and verification

Install .NET 8 SDK and Inno Setup 6 on Windows, then run build-installer.ps1. Outputs: publish/DroneSimulator.exe with its companion DLLs, and dist/DroneSimulatorSetup.exe. The installer is unsigned.

GitHub Actions builds the installer, runs the C# connector checks, checks five-second application startup, validates seven model profiles, records SHA256 checksums and uploads the Windows artifact for 14 days. It runs for main pushes, pull requests and manual requests. Download the artifact from the successful workflow run and extract it; open dist/DroneSimulatorSetup.exe.

Run client checks: dotnet run --project tests/ConnectorHarness.csproj -c Release. Tests may additionally exercise a real disposable test server when CONNECTOR_E2E_URL and CONNECTOR_E2E_CODE are provided. These values must be test-only.

The system receiver update is maintained separately because the main system source is private. No production system or operational records are updated by this client release. See VALIDATION.md for observed results.
