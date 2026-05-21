---
name: xgpsm-replace
description: Replaces save entries inside an Xbox Game Pass WGS container with files from another source such as a Steam or Epic save folder. Use when the user wants to migrate their progress from Steam or Epic into Xbox Game Pass. Always run xgpsm-backup first before invoking this skill.
allowed-tools: [shell]
---

# XGP-SM: Replace / Inject Save Files

Inject external save data (e.g. from Steam) into an Xbox Game Pass WGS container.

## Steps

1. If package name and XUID are not already resolved, run the xgpsm-scan skill.

2. **Always run xgpsm-backup first.** Do not proceed without a confirmed backup path.

3. Execute the replace command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe replace --package <package> --xuid <xuid> --source <source_path> --json
   ```

4. Parse the result:
   - `replaced`: Number of save entries replaced
   - `removed`: Number of stale entries removed

5. Confirm to the user what was changed. Remind them to launch the game to verify the save loaded correctly.

## Notes

- `source_path` should be the folder or file path of the replacement save data (e.g. a Steam `userdata/<steamid>/<appid>/remote/` directory or an extracted `.sav` file).
- Not all container entries are save slots — some store critical metadata. The CLI handles safe filtering automatically.
- If the operation fails midway, the backup can be used to restore the original state.
