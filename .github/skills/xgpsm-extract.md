---
name: xgpsm-extract
description: Extracts save files from an Xbox Game Pass game profile to a local folder. Use when the user wants to get their XGP save files out — to back them up, inspect them, or import them into another storefront like Steam or Epic. Always run xgpsm-scan first to resolve the package name and XUID.
allowed-tools: [shell]
---

# XGP-SM: Extract Save Files

Extract raw save data chunks from an Xbox Game Pass WGS profile.

## Steps

1. If package name and XUID are not already known, run the xgpsm-scan skill first.

2. Execute the extract command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe extract --package <package> --xuid <xuid> --json
   ```

3. Parse the JSON result:
   - `filesExtracted`: Number of files written out
   - `outputPath`: Folder where the extracted files were placed

4. Inform the user where their save files are and how many were extracted.

## Notes

- Extracted files may have no extension if the game is not registered in `games.json`. This is expected for unregistered titles.
- The extracted folder is safe to copy, zip, or use as a source for replacing saves in another storefront.
- Do not modify the original WGS container — extraction is always read-only.
