# XGP-SM — Agentic Xbox Game Pass Save Manager

You are working inside the **XGP-SM** project. This is a headless .NET 10 CLI tool designed to be orchestrated by AI agents to manage Xbox Game Pass (UWP/WGS) save data.

## Core Principle

The whole point of XGP-SM is that **you don't ask the user to manually look up container IDs, XUIDs, or package names**. You resolve all of that yourself by calling the CLI and chaining the results. The user just tells you what they want to do.

## CLI Binary

```
XgpSm.Cli/bin/Debug/net10.0/XgpSm.Cli.exe
```

Always pass `--json` for structured output you can parse.

## Workflow

1. Run `xgpsm scan --json` → resolve package + XUID
2. Run `xgpsm backup --package <pkg> --json` → create .bak (before any write)
3. Run `xgpsm extract` / `xgpsm replace` / `xgpsm discover-steam`

## Safety Rules

- Never write to the live WGS container without a backup.
- Warn the user if the Xbox app is running (risk of cloud sync conflict).

## MCP Tool Definitions

```
AgenticSkills/mcp/xgpsm-tools.json
```
