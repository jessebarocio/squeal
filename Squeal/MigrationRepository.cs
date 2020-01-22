using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Squeal
{
    class MigrationRepository
    {
        private readonly string _migrationDir;

        public MigrationRepository(string migrationDir)
        {
            _migrationDir = migrationDir;
        }

        public IEnumerable<Operation> GetOperations(int currentMigration, int? targetMigration = null)
        {
            // Assume this is an upgrade
            bool isUpgrade = targetMigration.HasValue ? currentMigration <= targetMigration : true;
            Operation[] operations;
            if(isUpgrade)
            {
                operations = Directory.EnumerateFiles(_migrationDir, "*.upgrade.sql")
                    .Select(path => new Operation(path, OperationType.Upgrade))
                    .Where(m => m.Id > currentMigration && (targetMigration.HasValue ? m.Id <= targetMigration.Value : true))
                    .OrderBy(m => m.Id)
                    .ToArray();
            }
            else
            {
                operations = Directory.EnumerateFiles(_migrationDir, "*.rollback.sql")
                    .Select(path => new Operation(path, OperationType.Rollback))
                    .Where(m => m.Id > targetMigration.Value && m.Id <= currentMigration)
                    .OrderByDescending(m => m.Id)
                    .ToArray();
            }
            return operations;
        }
    }

    enum OperationType
    {
        Upgrade,
        Rollback
    }

    class Operation
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public OperationType Type { get; set; }

        public Operation(string path, OperationType operationType)
        {
            var fileNameSplit = System.IO.Path.GetFileName(path).Split("_", 2);
            Path = path;
            Id = Int32.Parse(fileNameSplit[0]);
            Name = fileNameSplit[1].Split(".")[0];
            Type = operationType;
        }
    }
}
