﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

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
            // we should always have at least 3 at this point.  two real commands (for 1 pipe)
            //  and a blank string at the end, because of the way that regex split works.
            if (cmds.Count < 3)
            {
                Console.WriteLine("Something went wrong. :(");
                return false;
            }

            cmds.RemoveAt(cmds.Count - 1); // remove the last empty item, because regex.split is weird.

            // capture the original output stream.
            var stdout = Console.Out;

            // capture each commands output and pass it into the next command.
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

            // show the final output to the user
            using (var sr = new StringReader(last.ToString()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    stdout.WriteLine(line);
                }
            }

            // put the stream back the way you found it.
            Console.SetOut(stdout);

            return true;
        }

        public bool Match(string cmd)
        {
            // remove quoted strings.
            var t = Regex.Replace(cmd, @"\"".*?\""", string.Empty);
            return t.Contains(this.Name);
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help{
                    Command = "|",
                    Description = new List<string>{
                        "Allows you to send out from a command into another.",
                        "  Examples:",
                        "   - find device * | in 10.10.10.10",
                        "   - find device * | re \"Cisco|Juniper\"",
                    }
                }
            };
        }
    }
}
