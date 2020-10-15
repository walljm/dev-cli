using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;
using System.IO;
using System.Text.RegularExpressions;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class AllCommand : CommandBase, ICommand
    {
        public override string Name { get { return "all"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public AllCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var args = (cmd.StartsWith(this.Name) ? cmd : $"{this.Name} {cmd}").Split(' ').Skip(1);
            var cmdtorun = string.Join(' ', args);
            var oldPath = ctx.GetEnvVariable(Constants.EnvironmentProjectPath);

            // get all the projects
            var projects = Directory.GetDirectories(ctx.GetEnvVariable(Constants.EnvironmentProjectPath))
                .Where(d => Regex.IsMatch(d, @"^\w") && Directory.Exists(Path.Combine(d, ".git")));

            foreach (var project in projects)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"{cmdtorun} {project}");
                Console.WriteLine("---------------------------------------------");
                Console.ResetColor();
                ctx.SetEnvVariable(Constants.EnvironmentProjectPath, project);
                var torun = ctx.GetCommand(cmdtorun);
                if (torun  != null)
                {
                    await torun.Run(cmdtorun);
                }
                Console.WriteLine();
            }

            ctx.SetEnvVariable(Constants.EnvironmentProjectPath, oldPath);
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
                        $"Run a command in every project folder under {this.stack.Current.GetEnvVariable(Constants.EnvironmentProjectPath)}",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
