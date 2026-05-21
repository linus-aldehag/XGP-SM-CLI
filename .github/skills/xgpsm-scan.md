---
name: xgpsm-scan
description: Scans the local machine for all Xbox Game Pass games with save data present. Use this whenever the user wants to see what XGP saves they have, or to resolve package names and XUIDs before extracting or replacing saves. This must be run first before any other xgpsm skill.
allowed-tools: [shell]
---

# XGP-SM: Scan Local Profiles

Run the XGP-SM scan command and parse the results.

## Steps

1. Execute the CLI scan:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe scan --json
   ```

2. Parse the JSON output. Each entry contains:
   - `name`: Human-readable game name
   - `package`: Full UWP package identifier (needed for all other commands)
   - `profiles`: Array of XUID profiles, each with `xuid`, `lastPlayed`, `saveSize`, `chunkCount`

3. Present the results clearly to the user — game name, last played date, save footprint.

4. Store the resolved `package` and `xuid` values internally so subsequent skills don't need to ask the user for them.

## Notes

- Never ask the user to provide a package name or XUID manually. Always resolve it from this scan output.
- If no games are found, check that the Xbox app has been used on this machine and that `%LOCALAPPDATA%\Packages` exists.
