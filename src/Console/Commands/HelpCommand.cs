using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public class HelpCommand : CommandBase, ICommand
    {
        public override string Name { get { return "help"; } }
        public override string[] Aliases { get { return new[] { "?" }; } }

        public HelpCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public Task<bool> Run(string cmd)
        {
            WriteHelp(this.context.Commands);
            return Task.FromResult(true);
        }

        public static void WriteHelp(List<ICommand> commands, bool printHeader = true, bool printExit = true)
        {
            var spacer = "  ";

            // calculate the column widths.
            var cWidth = 0;
            var dWidth = 0;
            foreach (var command in commands)
            {
                var helps = command.GetHelp();
                foreach (var help in helps)
                {
                    cWidth = cWidth < help.Command.Name.Length ? help.Command.Name.Length : cWidth;
                    foreach (var desc in help.Description)
                    {
                        dWidth = dWidth < desc.Length ? desc.Length : dWidth;
                    }
                }
            }
            Console.WriteLine();
            if (printHeader)
            {
                Console.WriteLine($"{spacer}{"Command".PadRight(cWidth)}{spacer}Description");
                Console.WriteLine($"{spacer}{string.Empty.PadRight(cWidth, '-')}{spacer}{string.Empty.PadRight(dWidth, '-')}");
                Console.WriteLine();
            }

            foreach (var command in commands)
            {
                var helps = command.GetHelp();
                foreach (var help in helps)
                {
                    Console.WriteLine($"{spacer}{help.Command.Name.PadRight(cWidth)}{spacer}{help.Description.First()}");
                    if (help.Command.Aliases.Length > 0)
                    {
                        Console.WriteLine($"{spacer}Aliases: {string.Join(" | ", help.Command.Aliases)}");
                    }

                    foreach (var desc in help.Description.Skip(1))
                    {
                        Console.WriteLine($"{spacer}{string.Empty.PadRight(cWidth)}{spacer}{desc}");
                    }
                    Console.WriteLine();
                }
            }
            if (printExit)
            {
                Console.WriteLine($"{spacer}{"exit".PadRight(cWidth)}{spacer}Exits the CLI application");
            }

            Console.WriteLine();
        }

        public Help[] GetHelp()
        {
            return Array.Empty<Help>();
        }
    }
}
