using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class TransferCommandHandler
    {
        public static void Handle(string package, string sourceXuid, string targetXuid, bool json)
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

            string resultPath = manager.TransferFolder(targetGame, sourceXuid, targetXuid);

            if (string.IsNullOrEmpty(resultPath))
            {
                if (json) Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Transfer failed or source not found" }, AppJsonContext.Default.ErrorResult));
                else Console.WriteLine("Error: Transfer failed or source not found");
                return;
            }

            if (json)
            {
                Console.WriteLine(JsonSerializer.Serialize(new TransferResult { success = true, targetPath = resultPath }, AppJsonContext.Default.TransferResult));
            }
            else
            {
                Console.WriteLine($"Transfer complete. Profile copied to: {resultPath}");
            }
        }
    }
}
