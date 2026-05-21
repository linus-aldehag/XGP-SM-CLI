---
name: xgpsm-discover-steam
description: Scans the local Steam userdata directory to find the save folder for a specific game by its Steam App ID. Use when the user wants to migrate their Steam save progress into Xbox Game Pass. Run this to resolve the source path, then pass it to xgpsm-replace.
allowed-tools: [shell]
---

# XGP-SM: Discover Steam Save Path

Find where Steam has stored save data for a game so it can be injected into Xbox Game Pass.

## Steps

1. Ask the user for the game name. Look up the Steam App ID yourself using context or common knowledge (e.g. Starfield = 1716740, Hades = 1145360). Do not ask the user for the App ID.

2. Execute the discover command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe discover-steam --appid <steam_app_id> --json
   ```

3. Parse the result:
   - `steamUserId`: Resolved Steam user ID
   - `savePath`: Full path to the Steam save folder for this game

4. Confirm the found path with the user, then pass `savePath` as the `--source` argument to xgpsm-replace.

## Notes

- Steam saves are typically at: `C:\Program Files (x86)\Steam\userdata\<SteamID>\<AppID>\remote\`
- If the path is not found via the CLI, fall back to manually checking the above path pattern.
- If the user has multiple Steam accounts on the machine, list all found profiles and ask which one to use.
