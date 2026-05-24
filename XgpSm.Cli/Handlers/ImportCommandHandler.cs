using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSaveTools.Records;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class ImportCommandHandler
    {
        public static void Handle(string package, string xuid, string source)
        {
            try
            {
            var manager = new XboxContainerRepository();
            var games = GameList.DiscoverUserGames(GameList.ReadGameList());
            var targetGame = games.FirstOrDefault(g => g.Package == package);
            
            if (targetGame == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ReplaceResult> { success = false, error = new ErrorDetail { code = "ERROR", message = "Game package not found" } }, AppJsonContext.Default.ApiResponseReplaceResult));

                return;
            }

            var containers = manager.FindUserContainers(targetGame.Package);
            var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

            if (targetContainer == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ReplaceResult> { success = false, error = new ErrorDetail { code = "ERROR", message = "Container/XUID not found" } }, AppJsonContext.Default.ApiResponseReplaceResult));

                return;
            }

            if (!System.IO.Directory.Exists(source))
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ReplaceResult> { success = false, error = new ErrorDetail { code = "ERROR", message = "Source directory not found" } }, AppJsonContext.Default.ApiResponseReplaceResult));

                return;
            }

            var entries = manager.GetSaveEntries(targetGame, targetContainer).ToList();
            var sourceFiles = System.IO.Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            
            var replacements = new List<EntryReplacement>();
            int replaced = 0;
            int removed = 0;

            foreach (var entry in entries)
            {
                var matchingSource = sourceFiles.FirstOrDefault(sf => Path.GetFileName(sf) == entry.OutputName);
                
                if (matchingSource != null)
                {
                    replacements.Add(new EntryReplacement(entry.ContainerEntry, new FileInfo(matchingSource)));
                    replaced++;
                }
                else
                {
                    replacements.Add(new EntryReplacement(entry.ContainerEntry, null));
                    removed++;
                }
            }

            manager.ReplaceEntries(targetGame, targetContainer, replacements);

            Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ReplaceResult> { success = true, data = new ReplaceResult { replaced = replaced, removed = removed } }, AppJsonContext.Default.ApiResponseReplaceResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<ReplaceResult> { success = false, error = new ErrorDetail { code = "ERROR", message = ex.Message } }, AppJsonContext.Default.ApiResponseReplaceResult));

            }
        }
    }
}

