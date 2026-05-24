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
        public static void Handle(string package, string xuid)
        {
            try
            {
                var manager = new XboxContainerRepository();
                var games = GameList.DiscoverUserGames(GameList.ReadGameList());
                var targetGame = games.FirstOrDefault(g => g.Package == package);
                
                if (targetGame == null)
                {
                    Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ExtractResult> { success = false, error = new ErrorDetail { code = "ERROR", message = "Game package not found" } }, AppJsonContext.Default.ApiResponseExtractResult));

                    return;
                }

                var containers = manager.FindUserContainers(targetGame.Package);
                var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

                if (targetContainer == null)
                {
                    Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ExtractResult> { success = false, error = new ErrorDetail { code = "ERROR", message = "Container/XUID not found" } }, AppJsonContext.Default.ApiResponseExtractResult));

                    return;
                }

                int extracted = manager.Extract(targetGame, targetContainer);

                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ExtractResult> { success = true, data = new ExtractResult { success = true, filesExtracted = extracted } }, AppJsonContext.Default.ApiResponseExtractResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ExtractResult> { success = false, error = new ErrorDetail { code = "ERROR", message = ex.Message } }, AppJsonContext.Default.ApiResponseExtractResult));

            }
        }
    }
}

