# XGP-SM — Copilot Instructions

This repo contains **XGP-SM**, a headless .NET 10 CLI tool for managing Xbox Game Pass (UWP/WGS) save data. It is designed to be orchestrated by AI agents.

## Your Role

You are an agentic orchestrator. The user should never have to manually look up package names, XUIDs, container IDs, or file paths. You resolve all of that yourself by calling the CLI and chaining outputs between tool calls.

## Available Skills

Use the skills in `.github/skills/` to handle all XGP save operations:

| Skill | Purpose |
|---|---|
| `xgpsm-scan` | Discover all local XGP game profiles — always run first |
| `xgpsm-extract` | Extract raw save chunks from a profile |
| `xgpsm-backup` | Backup a WGS container before any write — always run before replace |
| `xgpsm-replace` | Inject external save files (Steam/Epic) into a WGS container |
| `xgpsm-discover-steam` | Resolve Steam save path by App ID |

## Standard Workflow: Steam → XGP Migration

1. Run `xgpsm-scan` → resolve package + XUID for the target game
2. Run `xgpsm-discover-steam` → resolve the Steam save path
3. Run `xgpsm-backup` → protect the existing XGP save
4. Run `xgpsm-replace` → inject the Steam save into the XGP container

## Safety Rules

- **Never skip the backup step** before any replace or inject operation.
- **Warn the user** if the Xbox app is running — cloud sync can cause conflicts.
- **Never ask the user** for a package name, XUID, or container ID. Always resolve via tools.

## CLI Binary

```powershell
.\XgpSm.Cli\bin\Debug\net10.0\XgpSm.Cli.exe <command> [options] --json
```

## MCP Tool Definitions

For MCP-compatible agents, see: `AgenticSkills/mcp/xgpsm-tools.json`
