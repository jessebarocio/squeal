using Dapper;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Linq;

namespace Squeal
{
    [Command(Description = "Runs database migrations against the target database.")]
    class UpCommand : SquealCommandBase
    {
        private Squeal Parent { get; set; }

        [Option("-m|--target-migration", Description = "The migration ID to update/rollback the database to")]
        public int? TargetMigration { get; set; }

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console)
        {
            string basePath = Parent.Path;
            var migrationDir = Path.Combine(basePath, "migrations");

            var config = SquealConfig.GetConfig(Parent);

            var connectionString = config.ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
            {
                console.Error.WriteLine("Connection string not set. Use --connection-string option or set ConnectionString property in squeal.json.");
                return -1;
            }

            using (var conn = ConnectionFactory.CreateConnection(config))
            {
                conn.Open();
                // Create metadata table if it doesn't exist
                conn.Execute(@"IF NOT EXISTS(SELECT TOP 1 * FROM sys.tables WHERE NAME = N'__squealmetadata')
BEGIN
    CREATE TABLE __squealmetadata
    (
        MigrationId INT PRIMARY KEY NOT NULL,
        MigrationName NVARCHAR(256) NOT NULL,
        DateApplied DATETIME NOT NULL
    )
END");

                // Get latest applied migration id
                int currentMigrationId = conn.ExecuteScalar<int?>("SELECT TOP 1 MigrationId FROM __squealmetadata ORDER BY DateApplied DESC") ?? 0;
                Console.WriteLine("Database is currently at migration {0}", currentMigrationId);

                var migrationRepo = new MigrationRepository(migrationDir);
                var operationsToApply = migrationRepo.GetOperations(currentMigrationId, TargetMigration);

                if(operationsToApply.Any())
                {
                    Console.WriteLine("Beginning update");
                    using (var trans = conn.BeginTransaction())
                    {
                        foreach (var operation in operationsToApply)
                        {
                            try
                            {
                                Console.Write("Executing {0} {1}...", operation.Type.ToString(), operation.Name);
                                conn.Execute(File.ReadAllText(operation.Path), transaction: trans);
                                if (operation.Type == OperationType.Upgrade)
                                {
                                    conn.Execute("INSERT INTO __squealmetadata (MigrationId, MigrationName, DateApplied) " +
                                        "VALUES (@Id, @Name, @Date)",
                                        new { operation.Id, operation.Name, Date = DateTime.UtcNow }, transaction: trans);
                                }
                                else if (operation.Type == OperationType.Rollback)
                                {
                                    conn.Execute("DELETE FROM __squealmetadata WHERE MigrationId = @Id",
                                        new { operation.Id }, transaction: trans);
                                }
                                Console.WriteLine(" complete");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine();
                                Console.WriteLine("An error occurred executing the operation:");
                                Console.WriteLine("\tMessage:     {0}", e.Message);
                                Console.WriteLine("\tMigration:   {0}", operation.Name);
                                Console.WriteLine("\tScript Path: {0}", operation.Path);
                                Console.WriteLine("Rolling back transaction");
                                trans.Rollback();
                                return -1;
                            }
                        }
                        trans.Commit();
                    }
                }
                else
                {
                    Console.WriteLine("Database is already up to date");
                }
            }

            return 0;
        }
    }
}
