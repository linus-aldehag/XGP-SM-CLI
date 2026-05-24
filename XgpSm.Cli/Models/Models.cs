using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using XgpSaveTools.Records;

namespace XgpSm.Cli.Models
{
    [JsonSerializable(typeof(ApiResponse<IEnumerable<GameInfo>>))]
    [JsonSerializable(typeof(ApiResponse<IEnumerable<ScannedGame>>))]
    [JsonSerializable(typeof(ApiResponse<ExtractResult>))]
    [JsonSerializable(typeof(ApiResponse<BackupResult>))]
    [JsonSerializable(typeof(ApiResponse<ReplaceResult>))]
    [JsonSerializable(typeof(ApiResponse<TransferResult>))]
    [JsonSerializable(typeof(ApiResponse<AnalyzeResult>))]
    [JsonSerializable(typeof(ApiResponse<IEnumerable<BackupInfo>>))]
    [JsonSerializable(typeof(PlayerDbResponse))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }

    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public T? data { get; set; }
        public ErrorDetail? error { get; set; }
    }

    public class ErrorDetail
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string suggestedAction { get; set; } = string.Empty;
    }

    public class ScannedGame
    {
        public string name { get; set; } = string.Empty;
        public string package { get; set; } = string.Empty;
        public List<ProfileInfo> profiles { get; set; } = new();
    }

    public class ProfileInfo
    {
        public string xuid { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public DateTime lastPlayed { get; set; }
        public long saveSize { get; set; }
        public int chunkCount { get; set; }
    }

    public class PlayerDbResponse
    {
        public bool success { get; set; }
        public PlayerDbData? data { get; set; }
    }

    public class PlayerDbData
    {
        public PlayerDbPlayer? player { get; set; }
    }

    public class PlayerDbPlayer
    {
        public string username { get; set; } = string.Empty;
    }

    public class ExtractResult
    {
        public bool success { get; set; }
        public int filesExtracted { get; set; }
    }

    public class ErrorResult
    {
        public string error { get; set; } = string.Empty;
    }

    public class BackupResult
    {
        public string backupPath { get; set; } = string.Empty;
    }

    public class ReplaceResult
    {
        public bool dryRun { get; set; }
        public int replaced { get; set; }
        public int removed { get; set; }
        public List<string> wouldReplace { get; set; } = new();
        public List<string> wouldRemove { get; set; } = new();
    }

    public class TransferResult
    {
        public bool success { get; set; }
        public bool dryRun { get; set; }
        public string targetPath { get; set; } = string.Empty;
    }

    public class AnalyzeResult
    {
        public string largestFileId { get; set; } = string.Empty;
        public long sizeBytes { get; set; }
        public string magicBytesHex { get; set; } = string.Empty;
        public string magicBytesAscii { get; set; } = string.Empty;
        public string guessedFormat { get; set; } = string.Empty;
    }

    public class BackupInfo
    {
        public string date { get; set; } = string.Empty;
        public string game { get; set; } = string.Empty;
        public string xuid { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
    }
}
