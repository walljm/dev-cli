using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class EnvCommand : ICommand
    {
        public string Name { get { return "env"; } }
        private readonly Stack<Context> stack;

        public EnvCommand(Stack<Context> stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Peek();
            foreach (var kvp in ctx.Variables.Where(kvp => kvp.Key != Constants.Pass))
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            return true;
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name);
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = $"env",
                    Description = new List<string>
                    {
                        "List the current environment variables for the cli."
                    }
                }
            };
        }
    }
}
