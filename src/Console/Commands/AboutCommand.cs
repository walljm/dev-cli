using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using CLI.Models;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class AboutCommand : CommandBase, ICommand
    {
        public override string Name { get { return "about"; } }
        public override string[] Aliases { get { return new[] { "version", "system" }; } }

        public AboutCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var fvi = GetProductVersion();
            Console.WriteLine();
            Console.WriteLine("  System Information:");
            Console.WriteLine("  ---------------------------------------------");
            this.stack.AppSettings.Public.PrintSettings();
            Console.WriteLine();
            Console.WriteLine($"  {"Version"}: {fvi.ProductVersion}");

            Console.WriteLine();
            return true;
        }

        public static FileVersionInfo GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi;
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = $"{this.Name}",
                    Description = new List<string>
                    {
                        "Show the current CLI version and other system info.",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
