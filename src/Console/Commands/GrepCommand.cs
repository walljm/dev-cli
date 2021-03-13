using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public class GrepCommand : CommandBase, IPipableCommand
    {
        public override string Name { get { return "grep"; } }
        public override string[] Aliases { get { return new[] { "in", "re" }; } }

        public Task<bool> Run(string cmd)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RunWithPipe(string cmd, StringBuilder stdIn)
        {
            var terms = cmd.Trim().Split(" ", StringSplitOptions.None);

            if (terms.Length < 2)
            {
                ContextStack.WriteLine("Please provide an expression to filter with.");
                HelpCommand.WriteHelp(new List<ICommand> { this }, false, false);
                return Task.FromResult(false);
            }

            var query = string.Join(' ', terms.Skip(1)).Trim('"');

            using (var sr = new StringReader(stdIn.ToString()))
            {
                string line;
                var i = 0;
                var t = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    t++;
                    if (Regex.IsMatch(line, query))
                    {
                        Console.WriteLine(line);
                        i++;
                    }
                }

                ContextStack.WriteLine();
                ContextStack.WriteLine($"Matching Lines: {i}, Total Lines: {t}");
                ContextStack.WriteLine();
            }
            return Task.FromResult(true);
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help{
                    Command = this.Name,
                    Description = new List<string>{
                        "Used in conjunction with pipe, it allows you to filter results using regular expressions.",
                        "  The '|' character is supported only if the expresssion is quoted.",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                        "",
                        "Examples:",
                        " - find device * | grep 10.10.10.10",
                        " - find device * | grep \"Cisco|Juniper\"",
                    }
                }
            };
        }
    }
}
