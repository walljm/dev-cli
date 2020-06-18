using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Console.WriteLine();
            Console.WriteLine($"   Available Commands");
            Console.WriteLine($"   --------------------------------------------------------------------------------");

            foreach (var command in ctx.Commands)
            {
                var helps = command.GetHelp();
                foreach (var help in helps)
                {
                    Console.WriteLine($"   {help}");
                }
            }
            Console.WriteLine("   exit | exits the cli application");
            Console.WriteLine();

            return true;
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public string[] GetHelp()
        {
            return new string[0];
        }
    }
}
