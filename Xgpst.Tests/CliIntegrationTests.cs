using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Xunit;
using XgpSm.Cli.Models;

namespace Xgpst.Tests
{
    public class CliIntegrationTests
    {
        private string GetCliPath()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "XgpSm.Cli.dll");
            Assert.True(File.Exists(path), $"CLI dll not found at {path}");
            return path;
        }

        private (string StdOut, string StdErr, int ExitCode) RunCli(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"\"{GetCliPath()}\" {args}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(5000);

            return (output, error, process.ExitCode);
        }

        [Fact]
        public void HelpCommand_ReturnsUsageInformation()
        {
            var (output, error, exitCode) = RunCli("--help");
            
            Assert.Equal(0, exitCode);
            Assert.Contains("Usage:", output);
            Assert.Contains("list", output);
            Assert.Contains("export", output);
            Assert.Contains("migrate", output);
            Assert.Contains("analyze", output);
        }

        [Fact]
        public void InvalidAnalyzeCommand_ReturnsJsonError()
        {
            // Test that running a command against a non-existent package returns our structured JSON ErrorResult
            var (output, error, exitCode) = RunCli("analyze --package FakePackage_123 --xuid 0000000000000000");
            
            // Should still return 0 because it's a handled application error outputting JSON
            Assert.Equal(0, exitCode);
            
            // Verify it parses into our ApiResponse struct
            var errorResult = JsonSerializer.Deserialize(output.Trim(), typeof(ApiResponse<AnalyzeResult>), AppJsonContext.Default) as ApiResponse<AnalyzeResult>;
            
            Assert.NotNull(errorResult);
            Assert.False(errorResult.success);
            Assert.Contains("Game package not found", errorResult.error?.message);
        }

        [Fact]
        public void LiveListCommand_ReturnsValidJson()
        {
            // Execute a live list against the host's actual WGS environment
            var (output, error, exitCode) = RunCli("list");
            
            Assert.Equal(0, exitCode);

            // Verify it's valid JSON (object format now, since it's ApiResponse<T>)
            using (var jsonDoc = JsonDocument.Parse(output.Trim()))
            {
                Assert.Equal(JsonValueKind.Object, jsonDoc.RootElement.ValueKind);
            }
        }
    }
}
