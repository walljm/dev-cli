using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class SetCommand : ICommand
    {
        public string Name { get { return "set"; } }
        private readonly Stack<Context> stack;

        public SetCommand(Stack<Context> stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Peek();

            var terms = cmd.Split(' ');
            if (terms.Length < 2)
            {
                Console.WriteLine($"Unknown command: {cmd}");
                return false;
            }

            var varname = terms[1];
            var val = terms[2];

            if (varname == Constants.ItpieUrl)
            {
                ctx.Variables[varname] = val.TrimEnd('/'); // trim trialing slashes because we don't want them.
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
                    Command = $"set {Constants.ItpieUrl} <api url>",
                    Description = new List<string>
                    {
                        "Sets the ITPIE api endpoint url e.g. https://itpie.yourdomain.com/api"
                    }
                }
            };
        }
    }
}
