using Dapper;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Squeal
{
    [Command(Description = "Runs a named script against the target database.")]
    class RunCommand : SquealCommandBase
    {
        private Squeal Parent { get; set; }

        [Option("-s|--script", Description = "The name of the script to run")]
        [Required]
        public string ScriptName { get; set; }

        [Option("-p|--parameter", CommandOptionType.MultipleValue,
            Description = "Pass a parameter to the script. Format: PARAM=VALUE, Replaces ${PARAM} in the script.")]
        public string[] Parameters { get; set; }

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console)
        {
            string basePath = Parent.Path;
            var scriptDir = Path.Combine(basePath, "scripts");

            var config = SquealConfig.GetConfig(Parent);

            var connectionString = config.ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
            {
                console.Error.WriteLine("Connection string not set. Use --connection-string option or set ConnectionString property in squeal.json.");
                return -1;
            }

            string scriptPath = Path.Combine(scriptDir, $"{ScriptName}.sql");
            if(!File.Exists(scriptPath))
            {
                console.Error.WriteLine($"Script {ScriptName} does not exist.");
                return -1;
            }

            using (var conn = ConnectionFactory.CreateConnection(config))
            {
                console.WriteLine($"Executing script {ScriptName}...");
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var template = File.ReadAllText(scriptPath);
                        var ps = Parameters.Select(p => p.Split("=")).ToDictionary(k => k[0], v => v[1]);
                        foreach (var key in ps.Keys)
                        {
                            template = template.Replace("${" + key + "}", ps[key]);
                        }
                        var rows = conn.Execute(template, transaction: trans);
                        console.WriteLine($"{rows} rows affected.");
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine("An error occurred executing the script:");
                        Console.WriteLine("\tMessage:     {0}", e.Message);
                        Console.WriteLine("\tScript:      {0}", ScriptName);
                        Console.WriteLine("\tScript Path: {0}", scriptPath);
                        Console.WriteLine("Rolling back transaction");
                        trans.Rollback();
                        return -1;
                    }
                }

            }

            return 0;
        }
    }
}
