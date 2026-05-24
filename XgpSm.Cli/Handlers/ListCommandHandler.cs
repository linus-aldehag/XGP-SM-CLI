using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using XgpSaveTools;
using XgpSaveTools.Common;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class ListCommandHandler
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        public static async Task HandleAsync()
        {
            try
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
                    
                    string username = $"Unknown ({container.UserTag})";
                    try
                    {
                        ulong decimalXuid = Convert.ToUInt64(container.UserTag, 16);
                        var response = await _httpClient.GetAsync($"https://playerdb.co/api/player/xbox/{decimalXuid}");
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var playerDbResult = JsonSerializer.Deserialize(content, AppJsonContext.Default.PlayerDbResponse);
                            if (playerDbResult != null && playerDbResult.success && playerDbResult.data?.player != null && !string.IsNullOrWhiteSpace(playerDbResult.data.player.username))
                            {
                                username = playerDbResult.data.player.username;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore lookup errors and fallback to Unknown
                    }
                    
                    scannedGame.profiles.Add(new ProfileInfo
                    {
                        xuid = container.UserTag,
                        username = username,
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

                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<IEnumerable<ScannedGame>> { success = true, data = scannedGames }, typeof(ApiResponse<IEnumerable<ScannedGame>>), new AppJsonContext(new JsonSerializerOptions { WriteIndented = true })));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<IEnumerable<ScannedGame>> { success = false, error = new ErrorDetail { code = "ERROR", message = ex.Message } }, typeof(ApiResponse<IEnumerable<ScannedGame>>), AppJsonContext.Default));

            }
        }
    }
}

