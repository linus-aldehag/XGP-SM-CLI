using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class AnalyzeCommandHandler
    {
        public static void Handle(string package, string xuid)
        {
            try
            {
            var manager = new XboxContainerRepository();
            var games = GameList.DiscoverUserGames(GameList.ReadGameList());
            var targetGame = games.FirstOrDefault(g => g.Package == package);
            
            if (targetGame == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Game package not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            var containers = manager.FindUserContainers(package);
            var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

            if (targetContainer == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "XUID container not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            var entries = manager.GetSaveEntries(targetGame, targetContainer).ToList();
            if (!entries.Any())
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "No save entries found in container" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            // Find the largest file, which is almost always the actual save data rather than metadata
            var largestEntry = entries.OrderByDescending(e => new FileInfo(e.ContainerEntry.Path).Length).First();
            var fileInfo = new FileInfo(largestEntry.ContainerEntry.Path);
            
            if (fileInfo.Length == 0)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Largest file is 0 bytes" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            byte[] magicBytes = new byte[Math.Min(16, fileInfo.Length)];
            using (var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _ = fs.Read(magicBytes, 0, magicBytes.Length);
            }

            string hexStr = BitConverter.ToString(magicBytes).Replace("-", " ");
            
            // Convert to ASCII, replacing non-printables with '.'
            var sb = new StringBuilder(magicBytes.Length);
            foreach (byte b in magicBytes)
            {
                if (b >= 32 && b <= 126) sb.Append((char)b);
                else sb.Append('.');
            }
            string asciiStr = sb.ToString();

            string guess = "Unknown Format";
            if (asciiStr.StartsWith("GVAS")) guess = "Unreal Engine Save (.sav)";
            else if (asciiStr.StartsWith("SQLite format 3")) guess = "SQLite Database (.sqlite / .db)";
            else if (asciiStr.StartsWith("{") || asciiStr.StartsWith("[\"")) guess = "JSON Document (.json)";
            else if (asciiStr.StartsWith("PK")) guess = "Zip Archive (.zip)";
            else if (hexStr.StartsWith("00 00 00 00")) guess = "Generic Binary / Unknown (.dat)";

            var result = new AnalyzeResult
            {
                largestFileId = largestEntry.OutputName,
                sizeBytes = fileInfo.Length,
                magicBytesHex = hexStr,
                magicBytesAscii = asciiStr,
                guessedFormat = guess
            };

            Console.WriteLine(JsonSerializer.Serialize(result, AppJsonContext.Default.AnalyzeResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = ex.Message }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
            }
        }
    }
}
