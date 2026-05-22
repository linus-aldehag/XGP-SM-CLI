---
name: xgpsm-transfer
description: Moves full directory structures between Xbox Game Pass profiles on disk. Use this skill when the user wants to copy or move Xbox saves from one Xbox profile to another. Do NOT use this skill for exporting to Steam/Epic.
allowed-tools: [shell]
---

# XGP-SM: Cross-Profile Save Transfer

Directly duplicates and moves an Xbox Game Pass WGS profile directory from one XUID to another on the same local disk. Since both endpoints are native Xbox saves, this process perfectly bypasses the need for complex structural replacement.

## Steps

1. **Always run xgpsm-backup first** on the target XUID if data exists, to ensure safety.

2. Execute the transfer command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe transfer --package <package> --source-xuid <xuid1> --target-xuid <xuid2> --json
   ```

3. Parse the JSON result:
   - `success`: A boolean indicating if the operation succeeded
   - `targetPath`: The path to the newly generated/overwritten Xbox profile

4. Inform the user that the saves have been transferred between Xbox profiles.
