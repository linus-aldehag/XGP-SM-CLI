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

            scanCommand.SetHandler(async (json) => await ScanCommandHandler.HandleAsync(json), jsonOption);

            var packageOption = new Option<string>(
                name: "--package",
                description: "Target game package name") { IsRequired = true };

            var xuidOption = new Option<string>(
                name: "--xuid",
                description: "Target user XUID") { IsRequired = true };

            var exportCommand = new Command("export", "Export save files into raw PC format (non-Xbox format)")
            {
                packageOption,
                xuidOption,
                jsonOption
            };

            exportCommand.SetHandler((package, xuid, json) => ExportCommandHandler.Handle(package, xuid, json), packageOption, xuidOption, jsonOption);

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

            var targetXuidOption = new Option<string>(
                name: "--target-xuid",
                description: "Target user XUID to transfer to") { IsRequired = true };

            var sourceXuidOption = new Option<string>(
                name: "--source-xuid",
                description: "Source user XUID to transfer from") { IsRequired = true };

            var transferCommand = new Command("transfer", "Moves full directory structures between Xbox profiles on disk")
            {
                packageOption,
                sourceXuidOption,
                targetXuidOption,
                jsonOption
            };

            transferCommand.SetHandler((package, sourceXuid, targetXuid, json) => TransferCommandHandler.Handle(package, sourceXuid, targetXuid, json), packageOption, sourceXuidOption, targetXuidOption, jsonOption);

            rootCommand.AddCommand(scanCommand);
            rootCommand.AddCommand(exportCommand);
            rootCommand.AddCommand(backupCommand);
            rootCommand.AddCommand(replaceCommand);
            rootCommand.AddCommand(transferCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}