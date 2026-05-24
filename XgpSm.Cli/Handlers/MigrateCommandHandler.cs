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
        public static void Handle(string package, string sourceXuid, string targetXuid, bool dryRun = false)
        {
            try
            {
            var manager = new XboxContainerRepository();
            var games = GameList.DiscoverUserGames(GameList.ReadGameList());
            var targetGame = games.FirstOrDefault(g => g.Package == package);
            
            if (targetGame == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<TransferResult> { success = false, error = new ErrorDetail { code = "GAME_NOT_FOUND", message = "Game package not found" } }, AppJsonContext.Default.ApiResponseTransferResult));

                return;
            }

            string resultPath = manager.TransferFolder(targetGame, sourceXuid, targetXuid, dryRun);

            if (string.IsNullOrEmpty(resultPath))
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<TransferResult> { success = false, error = new ErrorDetail { code = "TRANSFER_FAILED", message = "Transfer failed or source not found" } }, AppJsonContext.Default.ApiResponseTransferResult));

                return;
            }

            Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<TransferResult> { success = true, data = new TransferResult { success = true, dryRun = dryRun, targetPath = resultPath } }, AppJsonContext.Default.ApiResponseTransferResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<TransferResult> { success = false, error = new ErrorDetail { code = "MIGRATE_FAILED", message = ex.Message } }, AppJsonContext.Default.ApiResponseTransferResult));

            }
        }
    }
}

