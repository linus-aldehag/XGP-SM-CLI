---
name: xgpsm-analyze
description: Reads the magic bytes of the largest binary blob in an Xbox Game Pass save container to heuristically determine the native file format (e.g., Unreal Engine .sav, SQLite database, etc.). Use this skill when attempting to export or replace saves for an unregistered game to figure out what standard PC file extension it expects.
allowed-tools: [shell]
---

# XGP-SM: Analyze Magic Bytes

Acts as a forensic tool to inspect raw Xbox WGS save chunks, reading the file headers (magic bytes) to guess the original game save format.

## Steps

1. If package name and XUID are not already resolved, run the xgpsm-scan skill.

2. Execute the analyze command:
   ```powershell
   .\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe analyze --package <package> --xuid <xuid> --json
   ```

3. Parse the JSON result:
   - `magicBytesHex`: The raw hex signature of the file header.
   - `magicBytesAscii`: The ASCII string representation.
   - `guessedFormat`: A heuristic guess (e.g. "Unreal Engine Save (.sav)").

4. Use this information to correctly construct a specialized handler for exporting/replacing, or simply to rename the files properly after extraction.
