using System.CommandLine;
using System.Threading.Tasks;
using XgpSm.Cli.Handlers;

namespace XgpSm.Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Xbox Game Pass Save Manager (XGP-SM) deterministic CLI");

            var jsonOption = new Option<bool>(
                name: "--json",
                description: "Output machine-readable JSON");

            var scanCommand = new Command("scan", "Scans local XGP packages")
            {
                jsonOption
            };

            scanCommand.SetHandler((json) => ScanCommandHandler.Handle(json), jsonOption);

            var packageOption = new Option<string>(
                name: "--package",
                description: "Target game package name") { IsRequired = true };

            var xuidOption = new Option<string>(
                name: "--xuid",
                description: "Target user XUID") { IsRequired = true };

            var extractCommand = new Command("extract", "Extract files from a profile")
            {
                packageOption,
                xuidOption,
                jsonOption
            };

            extractCommand.SetHandler((package, xuid, json) => ExtractCommandHandler.Handle(package, xuid, json), packageOption, xuidOption, jsonOption);

            var backupCommand = new Command("backup", "Creates a timestamped backup of an Xbox Game Pass WGS save folder")
            {
                packageOption,
                xuidOption,
                jsonOption
            };

            backupCommand.SetHandler((package, xuid, json) => BackupCommandHandler.Handle(package, xuid, json), packageOption, xuidOption, jsonOption);

            var sourceOption = new Option<string>(
                name: "--source",
                description: "Source directory containing replacement save files") { IsRequired = true };

            var replaceCommand = new Command("replace", "Inject external save data into an Xbox Game Pass WGS container")
            {
                packageOption,
                xuidOption,
                sourceOption,
                jsonOption
            };

            replaceCommand.SetHandler((package, xuid, source, json) => ReplaceCommandHandler.Handle(package, xuid, source, json), packageOption, xuidOption, sourceOption, jsonOption);

            rootCommand.AddCommand(scanCommand);
            rootCommand.AddCommand(extractCommand);
            rootCommand.AddCommand(backupCommand);
            rootCommand.AddCommand(replaceCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}