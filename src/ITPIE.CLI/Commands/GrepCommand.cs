using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class GrepCommand : IPipableCommand
    {
        public string Name { get { return "grep"; } }
        private string[] aliases { get { return new[] { "in", "re" }; } }

        public async Task<bool> Run(string cmd)
        {
            return true;
        }

        public async Task<bool> RunWithPipe(string cmd, StringBuilder stdIn)
        {
            var terms = cmd.Trim().Split(" ", StringSplitOptions.None);

            if (terms.Length < 2)
            {
                Console.WriteLine("Please provide an expression to filter with.");
                HelpCommand.WriteHelp(new List<ICommand> { this }, false, false);
                return false;
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

                Console.WriteLine();
                Console.WriteLine($"Matching Lines: {i}, Total Lines: {t}");
                Console.WriteLine();
            }
            return true;
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help{
                    Command = "grep",
                    Description = new List<string>{
                        "Used in conjunction with pipe, it allows you to filter results using regular expressions.",
                        "  The '|' character is supported only if the expresssion is quoted.",
                        "",
                        "Aliases:",
                        "  in | re",
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
