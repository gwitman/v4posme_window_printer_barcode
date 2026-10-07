# Tech Stack

## Platform

- Language: C# (`WinExe`)
- Framework: .NET Framework 4.7.2
- UI: Windows Forms (`System.Windows.Forms`, `System.Drawing`)
- Printing: `System.Drawing.Printing`
- Build system: MSBuild (`.csproj`, ToolsVersion 15.0), classic non-SDK project format
- IDE: Visual Studio (solution-based, `.sln` under `v4posme_PrinterBarCode/`)

## Dependencies

- Newtonsoft.Json 13.0.3 — all JSON serialization/deserialization. Restored via `packages.config` into the local `packages/` folder (classic NuGet, not PackageReference).

Avoid introducing new dependencies without reason; the project intentionally stays close to the BCL plus Newtonsoft.Json.

## Networking conventions

- HTTP calls use `HttpClient` with a per-request `HttpRequestMessage` (not `DefaultRequestHeaders`), mirroring the companion MAUI app.
- TLS is forced to **TLS 1.2 only** in `Program.ConfigureSecurityProtocol()`. Do not add TLS 1.3 — it changes the handshake fingerprint and the server's WAF returns 403.
- `Expect: 100-continue` is disabled for the same WAF reason.
- The default `User-Agent` imitates PowerShell because browser User-Agents are blocked by the WAF.
- Keep these settings intact unless explicitly changing server-compatibility behavior.

## Configuration

- Runtime config lives in `config.json`, copied to the output directory (`CopyToOutputDirectory=PreserveNewest`), read from `AppDomain.CurrentDomain.BaseDirectory`.
- Models bind JSON via `[JsonProperty("...")]` with sensible defaults in the model constructors/initializers.

## Common commands

Run from the solution folder `v4posme_PrinterBarCode/`.

```bash
# Restore NuGet packages (classic)
nuget restore v4posme_PrinterBarCode.sln

# Build (Debug / Release)
msbuild v4posme_PrinterBarCode.sln /p:Configuration=Debug
msbuild v4posme_PrinterBarCode.sln /p:Configuration=Release

# Run the built app
./v4posme_PrinterBarCode/bin/Debug/v4posme_PrinterBarCode.exe
```

There is currently no automated test project. If adding one, prefer a standard .NET Framework-compatible test framework and keep it as a separate project in the solution.

## Logging

- Use the static `Logger` (`Logger.Info/Warn/Error`) for all diagnostics; it is thread-safe and never throws.
- Initialize it early via `Logger.Initialize(path)` before other work.
