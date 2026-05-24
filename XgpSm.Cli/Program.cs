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

            var listCommand = new Command("list", "Scans local XGP packages");

            listCommand.SetHandler(async () => await ListCommandHandler.HandleAsync());

            var packageOption = new Option<string>(
                name: "--package",
                description: "Target game package name") { IsRequired = true };

            var xuidOption = new Option<string>(
                name: "--xuid",
                description: "Target user XUID") { IsRequired = true };

            var exportCommand = new Command("export", "Export save files into raw PC format (non-Xbox format)")
            {
                packageOption,
                xuidOption
            };

            exportCommand.SetHandler((package, xuid) => ExportCommandHandler.Handle(package, xuid), packageOption, xuidOption);

            var backupCommand = new Command("backup", "Creates a timestamped backup of an Xbox Game Pass WGS save folder")
            {
                packageOption,
                xuidOption
            };

            backupCommand.SetHandler((package, xuid) => BackupCommandHandler.Handle(package, xuid), packageOption, xuidOption);

            var backupsCommand = new Command("backups", "Lists all locally stored timestamped backups");
            backupsCommand.SetHandler(() => BackupsCommandHandler.Handle());

            var sourceOption = new Option<string>(
                name: "--source",
                description: "Source directory containing replacement save files") { IsRequired = true };

            var dryRunOption = new Option<bool>(
                name: "--dry-run",
                description: "Preview changes without modifying any files") { IsRequired = false };

            var importCommand = new Command("import", "Inject external save data into an Xbox Game Pass WGS container")
            {
                packageOption,
                xuidOption,
                sourceOption,
                dryRunOption
            };

            importCommand.SetHandler((package, xuid, source, dryRun) => ImportCommandHandler.Handle(package, xuid, source, dryRun), packageOption, xuidOption, sourceOption, dryRunOption);

            var targetXuidOption = new Option<string>(
                name: "--target-xuid",
                description: "Target user XUID to transfer to") { IsRequired = true };

            var sourceXuidOption = new Option<string>(
                name: "--source-xuid",
                description: "Source user XUID to transfer from") { IsRequired = true };

            var migrateCommand = new Command("migrate", "Copy full directory structures between Xbox profiles on disk")
            {
                packageOption,
                sourceXuidOption,
                targetXuidOption,
                dryRunOption
            };

            migrateCommand.SetHandler((package, sourceXuid, targetXuid, dryRun) => MigrateCommandHandler.Handle(package, sourceXuid, targetXuid, dryRun), packageOption, sourceXuidOption, targetXuidOption, dryRunOption);

            var analyzeCommand = new Command("analyze", "Reads the Magic Bytes of the save container to guess its format")
            {
                packageOption,
                xuidOption
            };

            analyzeCommand.SetHandler((package, xuid) => AnalyzeCommandHandler.Handle(package, xuid), packageOption, xuidOption);

            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(exportCommand);
            rootCommand.AddCommand(backupCommand);
            rootCommand.AddCommand(backupsCommand);
            rootCommand.AddCommand(importCommand);
            rootCommand.AddCommand(migrateCommand);
            rootCommand.AddCommand(analyzeCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}