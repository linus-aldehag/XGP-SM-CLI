using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using XgpSaveTools.Extensions;
using XgpSm.Cli.Models;

namespace XgpSm.Cli.Handlers
{
    public static class BackupsCommandHandler
    {
        public static void Handle()
        {
            try
            {
                var backups = new List<BackupInfo>();
                string backupDir = IoExtensions.BackupOutput;

                if (Directory.Exists(backupDir))
                {
                    // Folder structure: BackupOutput / yyyy.MM.dd / GameName / XUID
                    var dateDirs = Directory.GetDirectories(backupDir);
                    foreach (var dateDir in dateDirs)
                    {
                        var date = Path.GetFileName(dateDir);
                        var gameDirs = Directory.GetDirectories(dateDir);
                        
                        foreach (var gameDir in gameDirs)
                        {
                            var game = Path.GetFileName(gameDir);
                            var xuidDirs = Directory.GetDirectories(gameDir);

                            foreach (var xuidDir in xuidDirs)
                            {
                                var xuid = Path.GetFileName(xuidDir);
                                backups.Add(new BackupInfo
                                {
                                    date = date,
                                    game = game,
                                    xuid = xuid,
                                    path = xuidDir
                                });
                            }
                        }
                    }
                }

                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<IEnumerable<BackupInfo>> { success = true, data = backups }, typeof(ApiResponse<IEnumerable<BackupInfo>>), new AppJsonContext(new JsonSerializerOptions { WriteIndented = true })));
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonSerializer.Serialize(new ApiResponse<IEnumerable<BackupInfo>> { success = false, error = new ErrorDetail { code = "ERROR", message = ex.Message } }, typeof(ApiResponse<IEnumerable<BackupInfo>>), AppJsonContext.Default));

            }
        }
    }
}

