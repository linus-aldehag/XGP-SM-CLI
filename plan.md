Project Plan: Agentic Xbox Game Pass Save Manager (XGP-SM)
An intelligent, zero-dependency .NET CLI utility designed to seamlessly enumerate, extract, move, and hot-swap local Xbox Game Pass (UWP/WGS) save data across multiple user profiles and alternative PC storefronts (Steam/Epic).

Architecture Strategy
Instead of forcing an AI model to parse complex local binary streams or deep Windows paths directly—which drains token context and introduces hallucination risks—the architecture separates the utility into two distinct layers:

The Deterministic Core CLI (.NET 8): A high-performance, native executable that interacts directly with the Windows filesystem, reads/writes binary container.index files, handles local file backups, and exposes a clean, machine-readable interface via structured JSON payloads.

The Agentic AI Layer: An external AI CLI tool/skill wrapper that consumes the Core CLI's JSON outputs, handles cloud path resolutions (e.g., parsing Steam userdata or Proton prefixes), executes online lookups for unrecognized games, and acts as a high-level orchestrator.

🛠️ Core Technology & Dependencies
Runtime: .NET 8 (Targeting native single-file publication, ensuring zero external setup requirements on modern Windows 11 systems).

Core Reference Architecture: Foraked or modularly adapted from brodrigz/XgpSaveTools, capitalizing on its programmatic virtual file mapping and container reassembly mechanisms.

CLI Engine: System.CommandLine or standard POSIX arguments to facilitate headless automation scripting.

📋 Architectural Roadmap
┌────────────────────────────────────────┐
│  Phase 1: The Core Engine (.NET CLI)    │
│  • Headless, machine-readable JSON I/O │
│  • Binary container.index parsing      │
└───────────────────┬────────────────────┘
                    ▼
┌────────────────────────────────────────┐
│  Phase 2: The Agentic Skill (Local AI)  │
│  • Map local profiles to Steam paths   │
│  • Multi-profile hot-swapping & backup  │
└───────────────────┬────────────────────┘
                    ▼
┌────────────────────────────────────────┐
│  Phase 3: The Online Fallback (RAG)    │
│  • Scrape PCGamingWiki / Wiki APIs    │
│  • Heuristics on unlisted save formats │
└────────────────────────────────────────┘
Phase 1: Machine-Readable Core Engine
JSON Serialization: Implement structured flags like xgpst --scan --json and xgpst --extract <XUID> --json.

Virtual Name Extraction: Parse the binary container.index within %LOCALAPPDATA%\Packages\<PackageName>\SystemAppData\wgs\<XUID>\ to extract the true filenames assigned by the game, mapping them directly out of their randomized hex GUID files.

Write & Injection Capabilities: Implement safely structured commands to append or modify entries inside the binary container index without bricking local database structures.

Phase 2: Agentic Automation Skills (Local Orchestration)
Cross-Profile Splicing: Develop scripts enabling the AI agent to duplicate an active wgs profile, restructure the virtual folder layout, inject foreign save states, and ensure target indexing keys match seamlessly.

Smart Local Target Discovery: Leverage local environmental scans to find active Steam paths (e.g., tracking down matching APPIDs within user directories like C:\Program Files (x86)\Steam\userdata\<SteamID>\).

Phase 3: Autonomous Web Heuristics (Unknown Titles)
Scraping Fallbacks: When encountering unlisted or newly launched titles, the AI agent initiates background web crawls via PCGamingWiki or community data sources to identify the XGP Package Name alongside target extensions.

Hex Magic-Byte Analysis: If data is missing online, read the initial file bytes directly. Identify headers (e.g., detecting Unreal Engine's GVAS or Unity serializations) to determine file extensions autonomously.

💻 Baseline Skill Blueprint: WGS Profiler
This basic diagnostic routine loops through installed Windows packages, filters system folders, identifies user storage volumes, and returns profile footprint heuristics.

C#
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

class XgpScannerSkill
{
    static void Main(string[] args)
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string packagesPath = Path.Combine(localAppData, "Packages");

        if (!Directory.Exists(packagesPath))
        {
            Console.WriteLine("[-] Could not locate Windows Packages folder.");
            return;
        }

        var appPackages = Directory.GetDirectories(packagesPath);
        
        foreach (var package in appPackages)
        {
            string wgsPath = Path.Combine(package, "SystemAppData", "wgs");
            if (!Directory.Exists(wgsPath)) continue;

            string packageName = Path.GetFileName(package);
            if (packageName.StartsWith("Microsoft.Windows") || packageName.StartsWith("Windows.")) continue;

            Console.WriteLine($"==================================================");
            Console.WriteLine($"GAME PACKAGE: {packageName}");
            Console.WriteLine($"==================================================");

            var profileDirs = Directory.GetDirectories(wgsPath)
                .Where(d => Path.GetFileName(d) != "t")
                .ToList();

            if (!profileDirs.Any())
            {
                Console.WriteLine("  [i] No active local profiles found with save states.");
                continue;
            }

            foreach (var profileDir in profileDirs)
            {
                string profileXuid = Path.GetFileName(profileDir);
                Console.WriteLine($"  👤 Profile XUID: {profileXuid}");

                var saveFiles = Directory.GetFiles(profileDir, "*", SearchOption.AllDirectories)
                    .Where(f => !f.EndsWith("container.index"))
                    .Select(f => new FileInfo(f))
                    .ToList();

                if (!saveFiles.Any())
                {
                    Console.WriteLine("    └── No physical save data chunks written.");
                    continue;
                }

                var lastWrite = saveFiles.Max(f => f.LastWriteTime);
                long totalBytes = saveFiles.Sum(f => f.Length);
                
                Console.WriteLine($"    ├── Last Played  : {lastWrite:yyyy-MM-dd HH:mm:ss} ({(DateTime.Now - lastWrite).Days} days ago)");
                Console.WriteLine($"    ├── Save Footprint: {FormatBytes(totalBytes)} across {saveFiles.Count} data chunks");
                
                string completeness = totalBytes > (1024 * 1024 * 5) ? "Large / Long Playtime" : "Small / Early Game"; 
                Console.WriteLine($"    └── Complexity   : {completeness}\n");
            }
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int counter = 0;
        decimal number = bytes;
        while (Math.Round(number / 1024) >= 1) { number /= 1024; counter++; }
        return $"{number:n2} {suffixes[counter]}";
    }
}
🔒 Security & Data Safety Fallbacks
[!WARNING]
Xbox Cloud Synchronization frameworks can be delicate and easily triggered into resolving conflicts by wiping out irregular local files.

The Mirror Protocol: The absolute first rule of execution inside the AI orchestration script dictates copying the active wgs directory out into an isolated local .bak folder before completing any container read/write operations.

Transactional Writes: When rebuilding custom binary tables or changing indexing configurations, modifications must be completed inside an app sandbox cache, then copied over the live file structure inside a singular, closed operation loop.