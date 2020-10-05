using McMaster.Extensions.CommandLineUtils;
using System.IO;

namespace Squeal
{
    [Subcommand(typeof(UpCommand), typeof(RunCommand))]
    class Squeal : SquealCommandBase
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Squeal>(args);

        [Option("--path <path>", Description = "The base path (defaults to current dir)", Inherited = true)]
        [DirectoryExists]
        public string Path { get; set; } = Directory.GetCurrentDirectory();

        [Option("--connection-string <connString>", Description = "The connection string (overrides squeal.json)", Inherited = true)]
        public string ConnectionString { get; set; }

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();
            return 1;
        }
    }
}
