# Connector validation — 24 October 2026

- Windows WPF publish with the new pairing UI: PASS locally (SDK 9.0.300, net8.0-windows).
- Real compiled C# ConnectionClient paired to a disposable system server, sent telemetry and GNSS loss: PASS.
- Browser preview showed the C# test unit, position, battery, altitude and GNSS fault in Simulation only; Live feeds stayed empty: PASS.
- Browser stale indication after telemetry stops: PASS.
- Nineteen targeted server tests: PASS. Covers permissions, unauthorized direct requests, CSRF, scoped token isolation, code reuse/expiry, session revocation, schema validation, replay, GNSS position freeze, capacity and existing mission behavior.
- System permission registry, TypeScript checks and production build: PASS in an isolated development copy.
- Existing seven metadata profiles retained.
- Updated GitHub installer build and startup checks: verify the PR's workflow result; the previous 0.1.0 build is not evidence for this version.
- Interactive WPF controls and install/uninstall: NOT VERIFIED. No real aircraft integration exists.

No production runtime data was copied for tests. VERSION-5-COLLAB working files and installation were not updated. The system-side patch targets a snapshot of the current local source, which includes unpublished Mission Operations work. It requires a restored-copy deployment rehearsal before production installation. The isolated browser preview lacked offline map tiles; the telemetry list and marker still worked. The main-system build reports an existing large-bundle advisory.
