using System;
using System.Threading.Tasks;
using CLI.Models;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class SetCommand : CommandBase, ICommand
    {
        public override string Name { get { return "set"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public SetCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;

            var terms = cmd.Split(' ');
            if (terms.Length < 2)
            {
                Console.WriteLine($"A value is required!");
                return false;
            }

            var varname = terms[1];
            var val = terms[2];
            ctx.SetEnvVariable(varname, val);

            return true;
        }

        public Help[] GetHelp()
        {
            return new Help[]{
            };
        }
    }
}
