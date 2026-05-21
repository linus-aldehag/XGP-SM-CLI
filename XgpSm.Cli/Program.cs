using System;
using System.CommandLine;
using System.Text.Json;
using System.Threading.Tasks;
using XgpSaveTools;
using XgpSaveTools.Common;

namespace XgpSm.Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Xbox Game Pass Save Manager (XGP-SM) deterministic CLI");

            var jsonOption = new Option<bool>(
                name: "--json",
                description: "Output machine-readable JSON");

            var scanCommand = new Command("scan", "Scans local XGP packages")
            {
                jsonOption
            };

            scanCommand.SetHandler((json) =>
            {
                var manager = new XboxContainerRepository();
                var games = GameList.DiscoverUserGames(GameList.ReadGameList());
                
                if (json)
                {
                    Console.WriteLine(JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true }));
                }
                else
                {
                    foreach (var game in games)
                    {
                        Console.WriteLine($"- {game.Name} (Package: {game.Package})");
                    }
                }
            }, jsonOption);

            var packageOption = new Option<string>(
                name: "--package",
                description: "Target game package name") { IsRequired = true };

            var xuidOption = new Option<string>(
                name: "--xuid",
                description: "Target user XUID") { IsRequired = true };

            var extractCommand = new Command("extract", "Extract files from a profile")
            {
                packageOption,
                xuidOption,
                jsonOption
            };

            extractCommand.SetHandler((package, xuid, json) =>
            {
                var manager = new XboxContainerRepository();
                var games = GameList.DiscoverUserGames(GameList.ReadGameList());
                var targetGame = games.FirstOrDefault(g => g.Package == package);
                
                if (targetGame == null)
                {
                    Console.WriteLine(json ? "{\"error\": \"Game package not found\"}" : "Error: Game package not found");
                    return;
                }

                var containers = manager.FindUserContainers(targetGame.Package);
                var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

                if (targetContainer == null)
                {
                    Console.WriteLine(json ? "{\"error\": \"Container/XUID not found\"}" : "Error: Container/XUID not found");
                    return;
                }

                int extracted = manager.Extract(targetGame, targetContainer);

                if (json)
                {
                    Console.WriteLine(JsonSerializer.Serialize(new { success = true, filesExtracted = extracted }));
                }
                else
                {
                    Console.WriteLine($"Success: {extracted} files extracted.");
                }
            }, packageOption, xuidOption, jsonOption);

            rootCommand.AddCommand(scanCommand);
            rootCommand.AddCommand(extractCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}