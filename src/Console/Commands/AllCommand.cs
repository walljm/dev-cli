using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public class AllCommand : CommandBase, ICommand
    {
        public override string Name { get { return "all"; } }
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public AllCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var args = (cmd.StartsWith(this.Name) ? cmd : $"{this.Name} {cmd}").Split(' ').Skip(1);
            var cmdtorun = string.Join(' ', args);
            var oldPath = this.stack.AppSettings.Public.ItpieProjectPath;

            // get all the projects
            var projects = Directory.GetDirectories(this.stack.AppSettings.Public.ItpieProjectPath)
                .Where(d => Regex.IsMatch(d, @"^\w") && Directory.Exists(Path.Combine(d, ".git")));

            foreach (var project in projects)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"{cmdtorun} {project}");
                Console.WriteLine("---------------------------------------------");
                Console.ResetColor();
                this.stack.AppSettings.Public.ItpieProjectPath = project;
                var torun = this.context.GetCommand(cmdtorun);
                if (torun != null)
                {
                    await torun.Run(cmdtorun);
                }
                Console.WriteLine();
            }

            this.stack.AppSettings.Public.ItpieProjectPath = oldPath;
            return true;
        }

        public Help[] GetHelp()
        {
            return new Help[] {
                new Help
                {
                    Command = $"{this.Name}",
                    Description = new List<string>
                    {
                        $"Run a command in every project folder under {this.stack.AppSettings.Public.ItpieProjectPath}",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
