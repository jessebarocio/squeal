using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Squeal
{
    [HelpOption("--help")]
    abstract class SquealCommandBase
    {
        private string AsciiArt =
@"   _____                        _ 
  / ____|                      | |
 | (___   __ _ _   _  ___  __ _| |
  \___ \ / _` | | | |/ _ \/ _` | |
  ____) | (_| | |_| |  __/ (_| | |
 |_____/ \__, |\__,_|\___|\__,_|_|
            | |                   
            |_|                  ";

        protected int OnExecute(CommandLineApplication app, IConsole console)
        {
            try
            {
                console.WriteLine(AsciiArt);
                console.WriteLine();
                return ExecuteCommand(app, console);
            }
            catch (Exception)
            {
                console.WriteLine("Unhandled exception!");
                throw;
            }
        }

        protected abstract int ExecuteCommand(CommandLineApplication app, IConsole console);
    }
}
