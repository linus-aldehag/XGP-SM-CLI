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
