using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class ScanCommandHandler
    {
        public static void Handle(bool json)
        {
            var manager = new XboxContainerRepository();
            var games = GameList.DiscoverUserGames(GameList.ReadGameList()).ToList();
            var scannedGames = new List<ScannedGame>();

            foreach (var game in games)
            {
                var scannedGame = new ScannedGame { name = game.Name, package = game.Package };
                var containers = manager.FindUserContainers(game.Package);
                
                foreach (var container in containers)
                {
                    var entries = manager.GetSaveEntries(game, container).ToList();
                    if (!entries.Any()) continue;
                    
                    var lastWrite = entries.Max(e => new FileInfo(e.ContainerEntry.Path).LastWriteTime);
                    long totalBytes = entries.Sum(e => new FileInfo(e.ContainerEntry.Path).Length);
                    
                    scannedGame.profiles.Add(new ProfileInfo
                    {
                        xuid = container.UserTag,
                        lastPlayed = lastWrite,
                        saveSize = totalBytes,
                        chunkCount = entries.Count
                    });
                }
                
                if (scannedGame.profiles.Any())
                {
                    scannedGames.Add(scannedGame);
                }
            }

            if (json)
            {
                Console.WriteLine(JsonSerializer.Serialize(scannedGames, typeof(IEnumerable<ScannedGame>), new AppJsonContext(new JsonSerializerOptions { WriteIndented = true })));
            }
            else
            {
                foreach (var game in scannedGames)
                {
                    Console.WriteLine($"- {game.name} (Package: {game.package})");
                    foreach (var profile in game.profiles)
                    {
                        Console.WriteLine($"  - Profile: {profile.xuid}, Size: {profile.saveSize} bytes, Last Played: {profile.lastPlayed}");
                    }
                }
            }
        }
    }
}
