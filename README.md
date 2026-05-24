# 🤖 XGP-SM (Agentic Xbox Game Pass Save Manager)

An intelligent, zero-dependency .NET 10 CLI utility designed to seamlessly enumerate, extract, move, and hot-swap local Xbox Game Pass (UWP/WGS) save data across multiple user profiles, with partial support for alternative PC storefronts (Steam/Epic).

XGP-SM is specifically built as a **headless, deterministic core engine** designed to be orchestrated by Agentic AI models. Rather than relying on a traditional interactive terminal, XGP-SM outputs strict, machine-readable JSON payloads, allowing AI agents to read local save structures, resolve external paths, and migrate saves across local profiles.

## 🌟 Acknowledgments & Kudos

This project builds upon the fantastic foundational work of the community:
- **[brodrigz/XgpSaveTools](https://github.com/brodrigz/XgpSaveTools)**: The robust .NET parsing foundation that this project was forked from.
- **[Z1ni/XGP-save-extractor](https://github.com/Z1ni/XGP-save-extractor)**: The original Python-based tool that pioneered WGS binary extraction and inspired this entire ecosystem.

Thank you to these developers for making this possible!

## 🚀 The Vision: Agentic Orchestration

Ultimately, this vision is about moving away from manually keeping track of random container IDs and bouncing various commands and their results back and forth just to reach your goal. By adding specific, structured capabilities to the application itself—coupled with deep agentic skills that understand how to utilize them fully—we unlock true autonomous save orchestration.

Instead of forcing an AI model to parse complex local binary streams or deep Windows paths directly—which drains token context and introduces hallucination risks—the architecture separates the utility into two distinct layers:

1. **The Deterministic Core CLI (.NET 10):** A high-performance, native executable that interacts directly with the Windows filesystem, reads/writes binary `container.index` files, handles local file backups, and exposes a clean, machine-readable interface via structured JSON payloads.
2. **The Agentic AI Layer:** External AI orchestration scripts (housed in the `AgenticSkills/` directory) that consume the Core CLI's JSON outputs, handle cloud path resolutions, and perform high-level cross-profile splicing.

## 🛠️ Requirements

- Windows 10/11 (UWP package layout located at `%LOCALAPPDATA%\Packages`)
- **.NET 10 Runtime** (if running from source; pre-compiled versions use Native AOT and have zero dependencies).

## 💻 Usage

XGP-SM is intended to be called headlessly by automation scripts or AI agents.

### Scan Local Profiles
Discover installed XGP titles and enumerate user profile footprints.

```bash
xgpsm list
```

### Export Save Data (Best-Effort)
Target a specific user profile and extract all raw save chunks into an output folder. *Note: Formatting these chunks for other storefronts is handler-dependent and provided on a best-effort basis.*

```bash
xgpsm export --package <PackageName> --xuid <XUID>
```

### Backup Profile Data
Create a safe, timestamped backup cache of a profile's WGS save folder before attempting any modifications.

```bash
xgpsm backup --package <PackageName> --xuid <XUID>
```

### List Backups
List all available timestamped profile backups created by the tool.

```bash
xgpsm backups
```

### Replace/Inject Save Data (Best-Effort)
Inject foreign saves securely into the local WGS container, mapping and replacing active binaries. *Note: Matching foreign saves to WGS containers is handler-dependent and provided on a best-effort basis.*

```bash
xgpsm import --package <PackageName> --xuid <XUID> --source <PathToSourceSaves>
```

### Cross-Profile Save Transfer
Move save game data between different Xbox profiles on the same disk. This direct container migration perfectly handles overwriting an existing profile's directories, or populating a completely empty new profile, without needing to perform complex extraction mapping.

```bash
xgpsm migrate --package <PackageName> --source-xuid <SourceXUID> --target-xuid <TargetXUID>
```

### Forensic Magic-Byte Analysis
Read the raw file headers of the Xbox save chunks to heuristically determine the native game engine or file format without relying on external wikis.

```bash
xgpsm analyze --package <PackageName> --xuid <XUID>
```

## 🧩 The Role of Specialized Handlers (Experimental)
- **For `export`**: Where supported, specialized handlers attempt to rename the raw Xbox chunks back into standard PC formats (e.g., `.sav`, `.dat`) on a best-effort basis.
- **For `import`**: Where supported, specialized handlers attempt to map incoming standard PC files to the obfuscated Xbox blobs they need to replace.

## 📦 Installation & Release Structure

XGP-SM is distributed as pre-compiled, self-contained zip packages using .NET 10 Native AOT. This means you do not need to install the .NET runtime or any dependencies to use it.

When you download the latest release zip, the folder structure looks like this:
```text
xgpsm-win-x64/
├── mcp/                       # MCP (Model Context Protocol) tool definitions
├── skills/                    # Native system-level AI skills/scripts
├── AGENT_INSTRUCTIONS.md      # Recommended agent guidelines and best practices
├── games.json                 # Core game database definitions
├── README.md                  # This documentation file
└── xgpsm.exe                  # The core deterministic CLI engine
```

## 🤖 Bootstrapping into an AI Agent

Since XGP-SM is designed specifically for AI orchestration, you can easily plug it into your AI agent of choice using the Model Context Protocol (MCP) definitions provided in the release package.

1. **Extract the Release:** Unzip the package to a permanent directory on your machine (e.g., `C:\Tools\XGP-SM`).
2. **Review Tool Schemas:** The `mcp/xgpsm-tools.json` file contains the complete system prompt configurations, input schemas, and expected JSON output envelopes for every CLI command.
3. **Register the Tools:** 
   - **For MCP-compatible Agents (e.g. Claude Desktop):** You can use an MCP CLI wrapper (like `mcp-cli` or similar) to expose `xgpsm.exe` commands directly to the agent using the JSON definitions.
   - **For Custom Agents (LangChain, AutoGen, etc):** Directly parse `mcp/xgpsm-tools.json` to load the tool descriptions into your LLM's context. Instruct your agent to fulfill these tool calls by executing `xgpsm.exe <command>` via the system shell and parsing the JSON `stdout`.

Because XGP-SM strictly returns consistent JSON envelopes for both successes and failures, your agent will be able to self-correct and autonomously navigate the local filesystem without human intervention.