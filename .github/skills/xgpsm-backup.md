---
name: xgpsm-backup
description: Creates a timestamped backup of an Xbox Game Pass WGS save folder before any write operations. Always run this before replacing or injecting saves. If the user wants to replace or inject a save, this skill must be invoked first as a safety gate.
allowed-tools: [shell]
---

# XGP-SM: Backup WGS Profile

Create a safe timestamped backup of a game's WGS save container.

## Steps

1. If the package name is not already known, run the xgpsm-scan skill first.

2. Execute the backup command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe backup --package <package> --json
   ```

3. Parse the result:
   - `backupPath`: The path to the `.bak` folder created

4. Confirm to the user that a backup was created and show the path before proceeding with any replace/inject operation.

## Notes

- This is a non-negotiable safety step. Never skip it.
- If the Xbox app is currently running, warn the user that cloud sync may overwrite the backup or conflict with local changes. Recommend closing the Xbox app first.
- The backup folder follows the naming pattern: `<PackageName>.bak.<timestamp>`
