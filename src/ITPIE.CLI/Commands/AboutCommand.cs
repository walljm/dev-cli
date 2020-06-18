using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class AboutCommand : ICommand
    {
        public string Name { get { return "about"; } }
        private string[] aliases { get { return new[] { "version", "system" }; } }

        public async Task<bool> Run(string cmd)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine();
            Console.WriteLine("  System Information:");
            Console.WriteLine("  ---------------------------------------");
            Console.WriteLine($"  Version: {fvi.ProductVersion}");
            Console.WriteLine();
            return true;
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = $"about",
                    Description = new List<string>
                    {
                        "Show the current CLI version and other system info.",
                        "Aliases:",
                        "  version | system",
                    }
                }
            };
        }
    }
}
