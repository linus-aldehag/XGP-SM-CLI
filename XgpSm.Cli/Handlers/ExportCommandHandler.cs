using System;
using System.Linq;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class ExportCommandHandler
    {
        public static void Handle(string package, string xuid, bool json)
        {
            var manager = new XboxContainerRepository();
            var games = GameList.DiscoverUserGames(GameList.ReadGameList());
            var targetGame = games.FirstOrDefault(g => g.Package == package);
            
            if (targetGame == null)
            {
                if (json) Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Game package not found" }, AppJsonContext.Default.ErrorResult));
                else Console.WriteLine("Error: Game package not found");
                return;
            }

            var containers = manager.FindUserContainers(targetGame.Package);
            var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

            if (targetContainer == null)
            {
                if (json) Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Container/XUID not found" }, AppJsonContext.Default.ErrorResult));
                else Console.WriteLine("Error: Container/XUID not found");
                return;
            }

            int extracted = manager.Extract(targetGame, targetContainer);

            if (json)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ExtractResult { success = true, filesExtracted = extracted }, AppJsonContext.Default.ExtractResult));
            }
            else
            {
                Console.WriteLine($"Success: {extracted} files extracted.");
            }
        }
    }
}
