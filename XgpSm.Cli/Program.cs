using System.CommandLine;
using System.Threading.Tasks;
using XgpSm.Cli.Handlers;

namespace XgpSm.Cli
{
    public static class SharedOptions
    {
        public static readonly Option<string> Package = new("--package", "Target game package name") { IsRequired = true };
        public static readonly Option<string> Xuid = new("--xuid", "Target user XUID") { IsRequired = true };
        public static readonly Option<string> Source = new("--source", "Source directory containing replacement save files") { IsRequired = true };
        public static readonly Option<bool> DryRun = new("--dry-run", "Preview changes without modifying any files") { IsRequired = false };
        public static readonly Option<string> TargetXuid = new("--target-xuid", "Target user XUID to transfer to") { IsRequired = true };
        public static readonly Option<string> SourceXuid = new("--source-xuid", "Source user XUID to transfer from") { IsRequired = true };
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Xbox Game Pass Save Manager (XGP-SM) deterministic CLI")
            {
                CreateListCommand(),
                CreateExportCommand(),
                CreateBackupCommand(),
                CreateBackupsCommand(),
                CreateImportCommand(),
                CreateMigrateCommand(),
                CreateAnalyzeCommand()
            };

            return await rootCommand.InvokeAsync(args);
        }

        private static Command CreateListCommand()
        {
            var cmd = new Command("list", "Scans local XGP packages");
            cmd.SetHandler(async () => await ListCommandHandler.HandleAsync());
            return cmd;
        }

        private static Command CreateExportCommand()
        {
            var cmd = new Command("export", "Export save files into raw PC format (non-Xbox format)")
            {
                SharedOptions.Package,
                SharedOptions.Xuid
            };
            cmd.SetHandler((package, xuid) => ExportCommandHandler.Handle(package, xuid), SharedOptions.Package, SharedOptions.Xuid);
            return cmd;
        }

        private static Command CreateBackupCommand()
        {
            var cmd = new Command("backup", "Creates a timestamped backup of an Xbox Game Pass WGS save folder")
            {
                SharedOptions.Package,
                SharedOptions.Xuid
            };
            cmd.SetHandler((package, xuid) => BackupCommandHandler.Handle(package, xuid), SharedOptions.Package, SharedOptions.Xuid);
            return cmd;
        }

        private static Command CreateBackupsCommand()
        {
            var cmd = new Command("backups", "Lists all locally stored timestamped backups");
            cmd.SetHandler(() => BackupsCommandHandler.Handle());
            return cmd;
        }

        private static Command CreateImportCommand()
        {
            var cmd = new Command("import", "Inject external save data into an Xbox Game Pass WGS container")
            {
                SharedOptions.Package,
                SharedOptions.Xuid,
                SharedOptions.Source,
                SharedOptions.DryRun
            };
            cmd.SetHandler((package, xuid, source, dryRun) => ImportCommandHandler.Handle(package, xuid, source, dryRun), SharedOptions.Package, SharedOptions.Xuid, SharedOptions.Source, SharedOptions.DryRun);
            return cmd;
        }

        private static Command CreateMigrateCommand()
        {
            var cmd = new Command("migrate", "Copy full directory structures between Xbox profiles on disk")
            {
                SharedOptions.Package,
                SharedOptions.SourceXuid,
                SharedOptions.TargetXuid,
                SharedOptions.DryRun
            };
            cmd.SetHandler((package, sourceXuid, targetXuid, dryRun) => MigrateCommandHandler.Handle(package, sourceXuid, targetXuid, dryRun), SharedOptions.Package, SharedOptions.SourceXuid, SharedOptions.TargetXuid, SharedOptions.DryRun);
            return cmd;
        }

        private static Command CreateAnalyzeCommand()
        {
            var cmd = new Command("analyze", "Reads the Magic Bytes of the save container to guess its format")
            {
                SharedOptions.Package,
                SharedOptions.Xuid
            };
            cmd.SetHandler((package, xuid) => AnalyzeCommandHandler.Handle(package, xuid), SharedOptions.Package, SharedOptions.Xuid);
            return cmd;
        }
    }
}