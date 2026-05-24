using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class MigrateCommandHandler
    {
        public static void Handle(string package, string sourceXuid, string targetXuid)
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

            string resultPath = manager.TransferFolder(targetGame, sourceXuid, targetXuid);

            if (string.IsNullOrEmpty(resultPath))
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Transfer failed or source not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            Console.WriteLine(JsonSerializer.Serialize(new TransferResult { success = true, targetPath = resultPath }, AppJsonContext.Default.TransferResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = ex.Message }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
            }
        }
    }
}
