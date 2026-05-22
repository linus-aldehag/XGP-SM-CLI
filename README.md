# 🤖 XGP-SM (Agentic Xbox Game Pass Save Manager)

An intelligent, zero-dependency .NET 10 CLI utility designed to seamlessly enumerate, extract, move, and hot-swap local Xbox Game Pass (UWP/WGS) save data across multiple user profiles and alternative PC storefronts (Steam/Epic).

XGP-SM is specifically built as a **headless, deterministic core engine** designed to be orchestrated by Agentic AI models. Rather than relying on a traditional interactive terminal, XGP-SM outputs strict, machine-readable JSON payloads, allowing AI agents to read local save structures, resolve Steam paths, and seamlessly migrate saves.

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
xgpsm scan --json
```

### Export Save Data
Target a specific user profile and export all raw save chunks into a zip file formatted for PC storefronts (Steam/Epic).

```bash
xgpsm export --package <PackageName> --xuid <XUID> --json
```

### Backup Profile Data
Create a safe, timestamped backup cache of a profile's WGS save folder before attempting any modifications.

```bash
xgpsm backup --package <PackageName> --xuid <XUID> --json
```

### Replace/Inject Save Data
Inject foreign saves (e.g. from Steam) securely into the local WGS container, mapping and replacing active binaries automatically.

```bash
xgpsm replace --package <PackageName> --xuid <XUID> --source <PathToSourceSaves> --json
```

### Cross-Profile Save Transfer
Move save game data between different Xbox profiles on the same disk. This direct container migration perfectly handles overwriting an existing profile's directories, or populating a completely empty new profile, without needing to perform complex extraction mapping.

```bash
xgpsm transfer --package <PackageName> --source-xuid <SourceXUID> --target-xuid <TargetXUID> --json
```

## 🏗️ Build from Source

Clone the repository and publish utilizing .NET 10 Native AOT for a standalone executable:

```bash
dotnet publish XgpSm.Cli/XgpSm.Cli.csproj \
  -c Release \
  -r win-x64 \
  /p:PublishAot=true \
  --output bin/Release/net10.0/publish/win-x64
```