using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;
using CliWrap;
using CliWrap.EventStream;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class GitCommand : CommandBase, ICommand
    {
        public override string Name { get { return "git"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public GitCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var args = cmd.Split(' ').Skip(1);

            var cli = Cli.Wrap(this.Name)
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory(ctx.GetEnvVariable(Constants.EnvironmentProjectPath))
                .WithArguments(args)
                ;

            await foreach (var cmdEvent in cli.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StandardOutputCommandEvent stdOut:
                        Console.WriteLine(stdOut.Text);
                        break;

                    case StandardErrorCommandEvent stdErr:
                        Console.WriteLine(stdErr.Text);
                        break;
                }
            }

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
                        $"Execute {this.Name} commands in the current environment: {this.stack.Current.GetEnvVariable(Constants.EnvironmentProjectPath)}",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
