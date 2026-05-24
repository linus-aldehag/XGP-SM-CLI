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
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Game package not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            var containers = manager.FindUserContainers(targetGame.Package);
            var targetContainer = containers.FirstOrDefault(c => c.UserTag == xuid || c.Dir.Contains(xuid));

            if (targetContainer == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Container/XUID not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
                return;
            }

            if (!System.IO.Directory.Exists(source))
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = "Source directory not found" }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
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

            Console.WriteLine(JsonSerializer.Serialize(new ReplaceResult { replaced = replaced, removed = removed }, AppJsonContext.Default.ReplaceResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ErrorResult { error = ex.Message }, AppJsonContext.Default.ErrorResult));
                Environment.ExitCode = 1;
            }
        }
    }
}
