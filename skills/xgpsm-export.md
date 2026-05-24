---
name: xgpsm-export
description: Exports Xbox Game Pass WGS save containers into raw PC formats (e.g. for Steam or Epic). Use this skill when the user wants to migrate away from Xbox Game Pass and needs the saves structurally unpacked. Do NOT use this skill for transferring saves between Xbox profiles.
allowed-tools: [shell]
---

# XGP-SM: Export Save Files to PC Formats

Extracts the binary UWP save container and exports it as standard, decrypted PC format files inside a zip archive.

## Steps

1. If package name and XUID are not already resolved, run the xgpsm-scan skill.

2. Execute the export command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe export --package <package> --xuid <xuid> --json
   ```

3. Parse the JSON result:
   - `filesExtracted`: Number of files written out
   - `outputPath`: Folder where the extracted files were placed

4. Inform the user where their save files are and how many were extracted.

## Notes

- Some games require specialized handlers to export successfully into a format recognizable by Steam/Epic (e.g., concatenating chunks or appending `.sav`). If a game exports using the `GenericHandler`, the resulting files might require manual renaming.
- Always verify the exported files before deleting original save data.
- Extracted files may have no extension if the game is not registered in `games.json`. This is expected for unregistered titles.
- The extracted folder is safe to copy, zip, or use as a source for replacing saves in another storefront.
- Do not modify the original WGS container — extraction is always read-only.
