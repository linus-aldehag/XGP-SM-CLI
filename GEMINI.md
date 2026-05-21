# XGP-SM — Agentic Xbox Game Pass Save Manager

You are working inside the **XGP-SM** project. This is a headless .NET 10 CLI tool designed to be orchestrated by AI agents to manage Xbox Game Pass (UWP/WGS) save data — scanning profiles, extracting saves, injecting saves from other storefronts (Steam/Epic), and backing up containers.

## Core Principle

Never manually chase down container IDs, bounce commands back and forth, or ask the user to look things up. You have the tools to resolve everything yourself. Always chain tool calls — scan first to resolve identifiers, then act.

## CLI Binary

The CLI tool is located at:
```
XgpSm.Cli/bin/Debug/net10.0/XgpSm.Cli.exe
```

Or when published:
```
bin/Release/net10.0/publish/win-x64/XgpSm.Cli.exe
```

Invoke it from PowerShell. Always pass `--json` for machine-readable output.

## Tool Call Order

1. **Always call `xgpsm_scan` first** to resolve package names and XUIDs.
2. **Call `xgpsm_backup` before any write operation** (replace/inject).
3. **Call `xgpsm_extract` or `xgpsm_replace`** to act on specific profiles.
4. **Call `xgpsm_discover_steam`** when the user wants to migrate from Steam.

## Shell Command Mappings

| Skill Tool | Shell Command |
|---|---|
| `xgpsm_scan` | `xgpsm scan --json` |
| `xgpsm_extract` | `xgpsm extract --package <pkg> --xuid <xuid> --json` |
| `xgpsm_backup` | `xgpsm backup --package <pkg> --json` |
| `xgpsm_replace` | `xgpsm replace --package <pkg> --xuid <xuid> --source <path> --json` |
| `xgpsm_discover_steam` | `xgpsm discover-steam --appid <id> --json` |

## Safety Rules

- **ALWAYS** back up before any write/replace/inject operation.
- **NEVER** delete or overwrite the live WGS container without a confirmed `.bak` folder in place.
- If cloud sync is active (Xbox app running), warn the user before modifying files.

## MCP Tool Definitions

The full MCP tool schema for this project is defined at:
```
AgenticSkills/mcp/xgpsm-tools.json
```
