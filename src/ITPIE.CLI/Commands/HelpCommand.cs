using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get { return "help"; } }
        private string[] aliases { get { return new[] { "?" }; } }
        private readonly Stack<Context> stack;

        public HelpCommand(Stack<Context> stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Peek();
            WriteHelp(ctx.Commands);
            return true;
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
                    cWidth = cWidth < help.Command.Length ? help.Command.Length : cWidth;
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

            foreach (var command in commands.OrderBy(c => c.Name))
            {
                var helps = command.GetHelp();
                foreach (var help in helps)
                {
                    Console.WriteLine($"{spacer}{help.Command.PadRight(cWidth)}{spacer}{help.Description.First()}");
                    var more = false;
                    foreach (var desc in help.Description.Skip(1))
                    {
                        more = true;
                        Console.WriteLine($"{spacer}{string.Empty.PadRight(cWidth)}{spacer}{desc}");
                    }
                    if (more)
                    {
                        Console.WriteLine();
                    }
                }
            }
            if (printExit)
            {
                Console.WriteLine($"{spacer}{"exit".PadRight(cWidth)}{spacer}Exits the CLI application");
            }
            Console.WriteLine();
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public Help[] GetHelp()
        {
            return new Help[0];
        }
    }
}
