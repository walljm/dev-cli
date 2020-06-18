using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                Console.WriteLine("That term is unsupported. Please use: 'grep', 'in', 'contains', or 're'");
                return false;
            }

            var type = terms[0];
            var query = string.Join(' ', terms.Skip(1)).Trim('"');

            using (var sr = new StringReader(stdIn.ToString()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    switch (type)
                    {
                        case "in":
                        case "grep":
                            HandleContains(query, line);
                            break;

                        case "re":
                            HandleRegex(query, line);
                            break;
                    }
                }
            }
            return true;
        }

        private static void HandleRegex(string query, string line)
        {
            if (Regex.IsMatch(line, query))
            {
                Console.WriteLine(line);
            }
        }

        private static void HandleContains(string query, string line)
        {
            if (line.Contains(query))
            {
                Console.WriteLine(line);
            }
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public string[] GetHelp()
        {
            return new[]{
                $"env | list the current environment"
            };
        }
    }
}
