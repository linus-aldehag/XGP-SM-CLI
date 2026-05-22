using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using XgpSaveTools.Records;

namespace XgpSm.Cli.Models
{
    [JsonSerializable(typeof(IEnumerable<GameInfo>))]
    [JsonSerializable(typeof(IEnumerable<ScannedGame>))]
    [JsonSerializable(typeof(ExtractResult))]
    [JsonSerializable(typeof(ErrorResult))]
    [JsonSerializable(typeof(BackupResult))]
    [JsonSerializable(typeof(ReplaceResult))]
    [JsonSerializable(typeof(PlayerDbResponse))]
    [JsonSerializable(typeof(TransferResult))]
    internal partial class AppJsonContext : JsonSerializerContext
    {
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
        public int replaced { get; set; }
        public int removed { get; set; }
    }

    public class TransferResult
    {
        public bool success { get; set; }
        public string targetPath { get; set; } = string.Empty;
    }
}
