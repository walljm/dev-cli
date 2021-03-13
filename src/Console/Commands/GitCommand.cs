using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;
using CliWrap;
using CliWrap.EventStream;

namespace CLI.Commands
{
    public class GitCommand : CommandBase, ICommand
    {
        public override string Name { get { return "git"; } }
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public GitCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var args = cmd.Split(' ').Skip(1);

            var cli = Cli.Wrap(this.Name)
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory(this.stack.AppSettings.Public.ItpieProjectPath)
                .WithArguments(args)
                ;

            await foreach (var cmdEvent in cli.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StandardOutputCommandEvent stdOut:
                        ContextStack.WriteLine(stdOut.Text);
                        break;

                    case StandardErrorCommandEvent stdErr:
                        ContextStack.WriteLine(stdErr.Text);
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
                    Command = this,
                    Description = new List<string>
                    {
                        $"Execute {this.Name} commands in the current environment: {this.stack.AppSettings.Public.ItpieProjectPath}",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
