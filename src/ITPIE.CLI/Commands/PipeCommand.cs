using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITPIE.CLI.Commands
{
    public class PipeCommand : ICommand
    {
        public string Name { get { return "|"; } }
        private readonly Stack<Context> stack;

        public PipeCommand(Stack<Context> stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Peek();

            var cmds = Regex.Split(cmd.Trim(), @"((?:[^|""']|""[^""]*""|'[^']*')+)")
                          .Skip(1)
                          .Where(o => o != this.Name)
                          .Select(o => o.Trim())
                          .ToList()
                          ;
            cmds.RemoveAt(cmds.Count-1); // remove the last empty item.

            var stdout = Console.Out;
            var last = new StringBuilder();

            foreach (var cmdText in cmds)
            {
                var sb = new StringBuilder();
                using (var wr = new StringWriter(sb))
                {
                    Console.SetOut(wr);

                    await ctx.HandlePipeCommand(cmdText, last);
                }
                last = sb;
            }

            using (var sr = new StringReader(last.ToString()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    stdout.WriteLine(line);
                }
            }
            Console.SetOut(stdout);

            return true;
        }

        public bool Match(string cmd)
        {
            // remove quoted strings.
            var t = Regex.Replace(cmd, @"\"".*?\""", string.Empty);
            return t.Contains(this.Name);
        }

        public string[] GetHelp()
        {
            return new[]{
                $"env | list the current environment"
            };
        }
    }
}
