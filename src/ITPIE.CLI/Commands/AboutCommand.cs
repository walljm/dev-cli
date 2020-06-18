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
        private readonly Stack<Context> stack;

        public AboutCommand(Stack<Context> stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var fvi = GetProductVersion();
            Console.WriteLine();
            Console.WriteLine("  System Information:");
            Console.WriteLine("  ---------------------------------------------");
            var ctx = this.stack.Peek();
            var kWidth = ctx.Variables.Max(kvp => kvp.Key.Length);

            foreach (var kvp in ctx.Variables.Where(kvp => kvp.Key != Constants.Pass))
            {
                Console.WriteLine($"  {kvp.Key.PadLeft(kWidth)}: {kvp.Value}");
            }
            Console.WriteLine($"  {"Version".PadLeft(kWidth)}: {fvi.ProductVersion}");

            Console.WriteLine();
            return true;
        }

        public static FileVersionInfo GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi;
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
                        "",
                        "Aliases:",
                        "  version | system",
                    }
                }
            };
        }
    }
}
